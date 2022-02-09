namespace SurveyAPI.Helpers
{
    public interface IMailSender
    {
        public void SendHtmlGmail(string recipientEmail, string id);

    }
}
