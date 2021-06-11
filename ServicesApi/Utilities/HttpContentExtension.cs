namespace ServicesApi.Utilities
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;

    public static class HttpContentExtension
    {
        public async static Task AddParametersHeader<T>(this HttpContext httpContext, IQueryable<T> queryable)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            long quantityRecords = await queryable.CountAsync();
            httpContext.Response.Headers.Add("totalRecords", quantityRecords.ToString());
        }
    }
}