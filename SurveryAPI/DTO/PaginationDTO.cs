namespace SurveyAPI.DTO
{
    public class PaginationDTO
    {
        public int Page { get; set; } = 1;

        private int recordsPerPage = 10;
        private readonly int maxRecordsPerPage = 30;


        // make sure the client can´t get more then recordsPerPage (10 right now)
        public int RecordsPerPage
        {
            get { return recordsPerPage; }
            set { recordsPerPage = (value > maxRecordsPerPage) ? maxRecordsPerPage: value; }
        }
    }
}
