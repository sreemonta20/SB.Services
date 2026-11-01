using System.Reflection;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging.Abstractions;
using SBERP.Shared.Authorization;
using Xunit;

namespace SBERP.Services.Tests;

public class ScreenPermissionCodecTests
{
    [Fact]
    public void Encode_normalises_route_links_and_adds_up_rights_of_several_rows()
    {
        string claim = ScreenPermissions.Encode(
        [
            ("/business/HumanResources/corehr/employee/", ScreenAction.View),
            ("/business/humanresources/corehr/employee", ScreenAction.Update),
            ("/business/home", ScreenAction.View),
            ("/business/nothing", ScreenAction.FromHttpMethod)
        ]);

        Assert.Equal("home=V;humanresources/corehr/employee=VU", claim);
    }

    [Fact]
    public void Decode_reverses_encode()
    {
        var rights = ScreenPermissions.Decode("home=V;security/appuserrole=VCUD");

        Assert.Equal(ScreenAction.View, rights["home"]);
        Assert.Equal(ScreenAction.View | ScreenAction.Create | ScreenAction.Update | ScreenAction.Delete, rights["security/appuserrole"]);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void No_claim_value_means_no_rights(string? claim) => Assert.Empty(ScreenPermissions.Decode(claim));

    [Fact]
    public void Own_profile_ignores_case_and_rejects_other_ids()
    {
        var id = Guid.NewGuid().ToString();
        var user = new ClaimsPrincipal(new ClaimsIdentity([new Claim(SberpClaims.ProfileId, id)], "test"));

        Assert.True(user.IsOwnProfile(id.ToUpperInvariant()));
        Assert.False(user.IsOwnProfile(Guid.NewGuid().ToString()));
        Assert.False(user.IsOwnProfile(null));
    }
}

public class ScreenPermissionFilterTests
{
    private static AuthorizationFilterContext Run(string verb, string? permissions, params object[] metadata)
    {
        Claim[] claims = permissions == null ? [] : [new Claim(SberpClaims.Permissions, permissions)];
        var http = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity(claims, "test")) };
        http.Request.Method = verb;
        var descriptor = new ActionDescriptor { EndpointMetadata = metadata.ToList() };
        var context = new AuthorizationFilterContext(new ActionContext(http, new RouteData(), descriptor), new List<IFilterMetadata>());
        new ScreenPermissionFilter(NullLogger<ScreenPermissionFilter>.Instance).OnAuthorization(context);
        return context;
    }

    private static int? Status(AuthorizationFilterContext c) => c.Result switch
    {
        null => null,
        ObjectResult o => o.StatusCode,
        StatusCodeResult s => s.StatusCode,
        _ => -1
    };

    [Fact]
    public void Endpoint_without_a_rule_is_refused() => Assert.Equal(403, Status(Run("GET", "home=VCUD")));

    [Fact]
    public void Anonymous_and_authenticated_only_endpoints_pass()
    {
        Assert.Null(Status(Run("GET", null, new AllowAnonymousAttribute())));
        Assert.Null(Status(Run("GET", "", new AuthenticatedOnlyAttribute())));
    }

    [Theory]
    [InlineData("GET", "home=V", null)]
    [InlineData("GET", "home=CUD", 403)]
    [InlineData("POST", "home=V", 403)]
    [InlineData("POST", "home=C", null)]
    [InlineData("PUT", "home=U", null)]
    [InlineData("DELETE", "home=VCU", 403)]
    [InlineData("DELETE", "home=D", null)]
    [InlineData("GET", "", 403)]
    public void Http_verb_decides_the_right_needed(string verb, string permissions, int? expected) =>
        Assert.Equal(expected, Status(Run(verb, permissions, new RequireScreenAttribute(Screens.Home))));

    [Fact]
    public void Rule_on_the_action_replaces_the_rule_on_the_controller()
    {
        object controller = new RequireScreenAttribute(Screens.Employee);

        Assert.Null(Status(Run("GET", "", controller, new AuthenticatedOnlyAttribute())));
        Assert.Null(Status(Run("GET", "home=V", controller, new RequireScreenAttribute(Screens.Home))));
        Assert.Equal(403, Status(Run("GET", "humanresources/corehr/employee=V", controller, new RequireScreenAttribute(Screens.Home))));
    }

    [Fact]
    public void Any_of_several_screens_is_enough() =>
        Assert.Null(Status(Run("GET", "home=V", new RequireScreenAttribute(Screens.Employee, Screens.Home))));

    [Fact]
    public void Token_from_before_the_claim_existed_is_sent_to_renew_not_refused()
    {
        var context = Run("GET", null, new RequireScreenAttribute(Screens.Home));

        Assert.IsType<UnauthorizedResult>(context.Result);
        Assert.Equal("true", context.HttpContext.Response.Headers["Token-Expired"].ToString());
    }
}

public class AuditActorFilterTests
{
    private sealed class Request
    {
        public string? CreateUpdateBy { get; set; }
        public string? Name { get; set; }
    }

    private static Request Run(string? profileId)
    {
        var request = new Request { CreateUpdateBy = "from-the-browser", Name = "n" };
        Claim[] claims = profileId == null ? [] : [new Claim(SberpClaims.ProfileId, profileId)];
        var http = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity(claims, "test")) };
        var context = new ActionExecutingContext(new ActionContext(http, new RouteData(), new ActionDescriptor()),
            new List<IFilterMetadata>(), new Dictionary<string, object?> { ["request"] = request }, new object());
        new AuditActorFilter().OnActionExecuting(context);
        return request;
    }

    [Fact]
    public void Actor_comes_from_the_token()
    {
        Request request = Run("token-profile");

        Assert.Equal("token-profile", request.CreateUpdateBy);
        Assert.Equal("n", request.Name);
    }

    [Fact]
    public void Anonymous_call_keeps_the_value() => Assert.Equal("from-the-browser", Run(null).CreateUpdateBy);
}

/// <summary>Fails the test run when someone adds an endpoint without saying who may call it.</summary>
public class EveryEndpointHasARuleTests
{
    [Theory]
    [InlineData("SBERP.Security")]
    [InlineData("SBERP.HumanResources")]
    public void All_actions_declare_a_rule(string assemblyName)
    {
        var controllers = Assembly.Load(assemblyName).GetTypes()
            .Where(t => typeof(ControllerBase).IsAssignableFrom(t) && !t.IsAbstract).ToList();
        Assert.NotEmpty(controllers);

        var missing = (
            from controller in controllers
            from action in controller.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            where action.GetCustomAttributes<HttpMethodAttribute>().Any()
            where !HasRule(action) && !HasRule(controller)
            select $"{controller.Name}.{action.Name}").ToList();

        Assert.True(missing.Count == 0, "No permission rule: " + string.Join(", ", missing));
    }

    private static bool HasRule(MemberInfo member) =>
        member.GetCustomAttribute<RequireScreenAttribute>() != null
        || member.GetCustomAttribute<AuthenticatedOnlyAttribute>() != null
        || member.GetCustomAttribute<AllowAnonymousAttribute>() != null;
}
