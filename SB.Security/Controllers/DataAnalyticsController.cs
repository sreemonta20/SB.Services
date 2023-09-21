using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Security.Helper;

namespace SB.Security.Controllers
{
    [Authorize]
    [Route(ConstantSupplier.CTRLER_ROUTE_PATH_NAME)]
    [ApiController]
    [EnableCors(ConstantSupplier.CORSS_POLICY_NAME)]
    public class DataAnalyticsController : ControllerBase
    {

    }
}
