namespace SurveyAPI.Helpers
{
    using SurveyAPI.DTO;

    public static class IQueryableExtensions
    {
        /// <summary>
        /// using a skip for skip several reords so we can return results in batches. 
        /// Take allows us to only return an x amount of records from our table. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="paginationDTO"></param>
        /// <returns></returns>
        public static IQueryable<T> Paginate<T>(this IQueryable<T> queryable, PaginationDTO paginationDTO)
        {
            return queryable
                .Skip((paginationDTO.Page - 1) * paginationDTO.RecordsPerPage)
                .Take(paginationDTO.RecordsPerPage);
        }
    }
}
