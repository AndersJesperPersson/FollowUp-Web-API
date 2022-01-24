namespace SurveyAPI.Helpers
{
    public interface IMailSender
    {
        public void SendPlainGmail(string recipientEmail);
        public void SendHtmlGmail(string recipientEmail);

    }
}
