using SBERP.Security.Helper;
using SBERP.Security.Models.Base;
using System.Text.Json.Serialization;

namespace SBERP.Security.Models.Response
{
    public class InitialDataResponse
    {
        [JsonConverter(typeof(NestedJsonConverter<AppUserRoleMenuInitialData>))]
        public List<AppUserRoleMenuInitialData>? userRolesList { get; set; }

        [JsonConverter(typeof(NestedJsonConverter<AppUserRoleMenuInitialData>))]
        public List<AppUserRoleMenuInitialData>? parentMenuList { get; set; }

        [JsonConverter(typeof(NestedJsonConverter<AppUserRoleMenuInitialData>))]
        public List<AppUserRoleMenuInitialData>? cssClassList { get; set; }

        [JsonConverter(typeof(NestedJsonConverter<AppUserRoleMenuInitialData>))]
        public List<AppUserRoleMenuInitialData>? routeLinkList { get; set; }

        [JsonConverter(typeof(NestedJsonConverter<AppUserRoleMenuInitialData>))]
        public List<AppUserRoleMenuInitialData>? routeLinkClassList { get; set; }

        [JsonConverter(typeof(NestedJsonConverter<AppUserRoleMenuInitialData>))]
        public List<AppUserRoleMenuInitialData>? iconList { get; set; }

        [JsonConverter(typeof(NestedJsonConverter<AppUserRoleMenuInitialData>))]
        public List<AppUserRoleMenuInitialData>? dropdownIconList { get; set; }

        public int? nextMenuSlNo { get; set; }
    }
}
