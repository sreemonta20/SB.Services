using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SBERP.Security.Models.Response;
using SBERP.Security.Service;
using System.Collections;
using System.Data;
using System.Linq.Dynamic.Core;
using System.Reflection;

namespace SBERP.Security.Helper
{
    public static class Utilities
    {
        public static T CreateObjecItemFromRow<T>(DataRow row) where T : new()
        {
            T item = new();

            // set the item
            SetItemFromRow(item, row);

            // return 
            return item;
        }
        public static List<T> CreateListFromTable<T>(DataTable tbl) where T : new()
        {
            // define return list
            List<T> lst = new();

            // go through each row
            foreach (DataRow r in tbl.Rows)
            {
                // add to the list
                lst.Add(CreateItemFromRow<T>(r));
            }

            // return the list
            return lst;
        }
        public static T CreateItemFromRow<T>(DataRow row) where T : new()
        {
            // create a new object
            T item = new();

            // set the item
            SetItemFromRow(item, row);

            // return 
            return item;
        }

        public static void SetItemFromRow<T>(T item, DataRow row) where T : new()
        {
            // go through each column
            foreach (DataColumn c in row.Table.Columns)
            {
                // find the property for the column
                PropertyInfo? p = item?.GetType().GetProperty(c.ColumnName);

                // if exists, set the value
                if (p != null && row[c] != DBNull.Value)
                {
                    p.SetValue(item, row[c], null);
                }
            }
        }

        public static PagedResult<T> GetPaged<T>(this IQueryable<T> query,int page, int pageSize) where T : class
        {
            var result = new PagedResult<T>();
            result.CurrentPage = page;
            result.PageSize = pageSize;
            result.RowCount = query.Count();
            


            var pageCount = (double)result.RowCount / pageSize;
            result.PageCount = (int)Math.Ceiling(pageCount);

            var skip = (page - 1) * pageSize;
            result.Queryable = query.Skip(skip).Take(pageSize).AsQueryable();

            return result;
        }

        public static async Task<PagingResult<T>> GetPagingResult<T>(this IQueryable<T> query, int page, int pageSize) where T : class
        {
            PagingResult<T> result = new()
            {
                CurrentPage = page,
                PageSize = pageSize,
                RowCount = query.Count()
            };



            double pageCount = (double)result.RowCount / pageSize;
            result.PageCount = (int)Math.Ceiling(pageCount);

            int skip = (page - 1) * pageSize;
            result.Items = await query.Skip(skip).Take(pageSize).ToListAsync();

            return result;
        }

        public static PagingResult<T> GetPagingResult<T>(this List<T> query, int page, int pageSize) where T : class
        {
            PagingResult<T> result = new()
            {
                CurrentPage = page,
                PageSize = pageSize,
                RowCount = query.Count()
            };



            double pageCount = (double)result.RowCount / pageSize;
            result.PageCount = (int)Math.Ceiling(pageCount);
            result.Items = query;

            return result;
        }

        public static List<T> ConvertDataTable<T>(DataTable dt) where T : new()
        {
            List<T> data = new();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }
        
        private static T GetItem<T>(DataRow row) where T : new()
        {
            T obj = new T();
            foreach (DataColumn column in row.Table.Columns)
            {
                PropertyInfo? prop = obj.GetType().GetProperty(column.ColumnName);
                if (prop != null && row[column] != DBNull.Value)
                {
                    object value = row[column];

                    if (prop.PropertyType == typeof(string))
                    {
                        prop.SetValue(obj, value.ToString(), null);
                    }
                    else if (prop.PropertyType == typeof(Guid?) || prop.PropertyType == typeof(Guid))
                    {
                        Guid guidValue;
                        if (Guid.TryParse(value.ToString(), out guidValue))
                        {
                            prop.SetValue(obj, guidValue, null);
                        }
                        else
                        {
                            prop.SetValue(obj, null, null);
                        }
                    }
                    else if (prop.PropertyType == typeof(bool?))
                    {
                        bool boolValue;
                        if (bool.TryParse(value.ToString(), out boolValue))
                        {
                            prop.SetValue(obj, boolValue, null);
                        }
                        else
                        {
                            prop.SetValue(obj, null, null);
                        }
                    }
                    else if (prop.PropertyType == typeof(int?))
                    {
                        int intValue;
                        if (int.TryParse(value.ToString(), out intValue))
                        {
                            prop.SetValue(obj, intValue, null);
                        }
                        else
                        {
                            prop.SetValue(obj, null, null);
                        }
                    }
                    else if (prop.PropertyType == typeof(DateTime?))
                    {
                        DateTime dateTimeValue;
                        if (DateTime.TryParse(value.ToString(), out dateTimeValue))
                        {
                            prop.SetValue(obj, dateTimeValue, null);
                        }
                        else
                        {
                            prop.SetValue(obj, null, null);
                        }
                    }
                    else
                    {
                        prop.SetValue(obj, Convert.ChangeType(value, prop.PropertyType), null);
                    }
                }
            }
            return obj;
        }
        public static bool IsNullOrEmpty(this IEnumerable enumerable)
        {
            if (enumerable is null) return true;

            foreach (var element in enumerable)
            {
                //If we make it here, it means there are elements, and we return false
                return false;
            }

            return true;
        }

        public static bool IsNull<T>(T obj)
        {
            return obj == null;
        }

        public static bool IsNull<T>(List<T> list)
        {
            return list == null || list.Count == 0;
        }

        public static bool IsNotNull<T>(T obj)
        {
            return obj != null;
        }

        public static bool IsNotNull<T>(List<T> list)
        {
            return list == null || list.Count == 0;
        }

        
        public static DataResponse FailedResponse<T>(T data, ISecurityLogService securityLogService, int responseCode,
                                             DataResponse? response = null, string message = "", string logMessage = "")
        {
            var finalResponse = response != null
                ? new DataResponse
                {
                    Success = response.Success,
                    Message = string.IsNullOrWhiteSpace(message) ? response.Message : message,
                    MessageType = response.MessageType,
                    ResponseCode = response.ResponseCode,
                    Result = data
                }
                : new DataResponse
                {
                    Success = false,
                    Message = string.IsNullOrWhiteSpace(message) ? ConstantSupplier.FAILED_MSG : message,
                    MessageType = Enum.EnumResponseType.Error,
                    ResponseCode = responseCode,
                    Result = null
                };

            if (!string.IsNullOrWhiteSpace(logMessage))
            {
                securityLogService.LogError($"{logMessage}: {JsonConvert.SerializeObject(finalResponse, Formatting.Indented)}");
            }

            return finalResponse;
        }
        public static string ConvertJObjectToJsonString(object inputValue, string itemName)
        {
            JObject? oJObject = JObject.Parse(inputValue.ToString());
            // Optionally, parse the Items property separately if it is a string containing JSON
            string oItems = oJObject[itemName].ToString();
            JArray oJArray = JArray.Parse(oItems);
            oJObject[itemName] = oJArray;

            string jsonString = oJObject.ToString();
            return jsonString;
        }

        public static bool IsNullOrEmptyOrWhiteSpace(string? value)
        {
            // Convert null to an empty string to prevent NullReferenceException
            string safeValue = value ?? string.Empty;

            // Return true if the string is empty or whitespace, otherwise false
            return string.IsNullOrWhiteSpace(safeValue);
        }

        /// <summary>
        /// Returns SQL query based on the provided query identifier
        /// </summary>
        /// <param name="queryIdentifier">The identifier for the SQL query</param>
        /// <returns>SQL query string</returns>
        /// <exception cref="ArgumentException">Thrown when query identifier is not found</exception>
        public static string GetSqlQuery(string queryIdentifier)
        {
            return queryIdentifier switch
            {
                ConstantSupplier.GET_ACTIVE_ROLES => @"SELECT Id, RoleName, Description, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate, IsActive 
                                FROM [dbo].[AppUserRoles] 
                                WHERE IsActive = 1 
                                ORDER BY ID DESC",

                ConstantSupplier.GET_ACTIVE_MENUS_BY_ROLE_ID => @"SELECT m.Id, m.Name, p.Name AS ParentName, rm.IsView, rm.IsCreate, rm.IsUpdate, rm.IsDelete
                                           FROM [dbo].[AppUserMenus] m
                                           LEFT JOIN [dbo].[AppUserMenus] p ON m.ParentId = p.Id
                                           LEFT JOIN [dbo].[AppUserRoleMenus] rm ON m.Id = rm.AppUserMenuId AND rm.AppUserRoleId = @RoleId AND rm.IsActive = 1
                                           WHERE m.IsActive = 1 AND m.IsComponent = 1",


                _ => throw new ArgumentException($"Query identifier '{queryIdentifier}' not found.", nameof(queryIdentifier))
            };
        }
    }
}
