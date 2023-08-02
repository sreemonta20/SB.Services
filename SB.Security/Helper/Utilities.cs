using Microsoft.EntityFrameworkCore;
using SB.Security.Models.Response;
using System.Collections;
using System.Data;
using System.Linq.Dynamic.Core;
using System.Reflection;

namespace SB.Security.Helper
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

        public static List<T> ConvertDataTable<T>(DataTable dt)
        {
            List<T> data = new();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }
        private static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                    {
                        var type = Nullable.GetUnderlyingType(pro.PropertyType) ?? pro.PropertyType;
                        if ((type.Name == "DateTime") && dr[column.ColumnName].Equals(DBNull.Value))
                        {
                            continue;
                        }
                     
                        pro.SetValue(obj, dr[column.ColumnName].Equals(DBNull.Value) ? string.Empty : dr[column.ColumnName], null);
                    }
                    else
                    {
                        continue;
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
    }
}
