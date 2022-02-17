namespace SurveyAPI.Helpers
{
    using Microsoft.EntityFrameworkCore;

    public static class HttpContextExtensions
    {
        //An extension to httpContext so a number for pagination can be sent in through the header in the http-request from UI. 
        public async static Task InsertParametersPaginationsInHeader<T>(this HttpContext context, IQueryable<T> queryable)
        {
            if(context == null) { throw new ArgumentNullException(nameof(context)); }

            double count = await queryable.CountAsync();
            context.Response.Headers.Add("totalAmountOfRecords", count.ToString());
        }
    }
}
