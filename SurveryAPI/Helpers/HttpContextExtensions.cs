namespace SurveyAPI.Helpers
{
    using Microsoft.EntityFrameworkCore;

    public static class HttpContextExtensions
    {
        //TODO: step through DEbugga för att förstå detta
        public async static Task InsertParametersPagninationsInHeader<T>(this HttpContext context, IQueryable<T> queryable)
        {
            if(context == null) { throw new ArgumentNullException(nameof(context)); }

            double count = await queryable.CountAsync();
            context.Response.Headers.Add("totalAmountOfRecords", count.ToString());
        }
    }
}
