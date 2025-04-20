using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SBERP.Security.Helper;

namespace SBERP.Security.Controllers.v1
{
    /// <summary>
    /// This API Controller will be containing all the data of the different business models in graphical representation.
    /// </summary>
    //[ApiVersion("1.0")] // Specify the version
    [Authorize]
    [Route(ConstantSupplier.CTRLER_ROUTE_PATH_NAME_VERSION_ONE)]
    [ApiController]
    [EnableCors(ConstantSupplier.CORSS_POLICY_NAME)]
    public class DataAnalyticsController : ControllerBase
    {

    }
}
