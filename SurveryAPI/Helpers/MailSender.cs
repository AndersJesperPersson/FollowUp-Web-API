namespace SurveyAPI.Helpers
{
    using FluentEmail.Core;
    using FluentEmail.Razor;
    using FluentEmail.Smtp;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Mail;
    using System.Text;
    public class MailSender : IMailSender
    {
        private readonly IServiceProvider _serviceProvider;
        public MailSender(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

       
        public async void SendHtmlGmail(string recipientEmail, string id)
        {
            using (var scope = _serviceProvider.CreateScope())
            {

                var mailer = scope.ServiceProvider.GetRequiredService<IFluentEmail>();
                var email = mailer
                  .To(recipientEmail, "Employee")
                  .Subject("Uppföljning")
                  .UsingTemplateFromFile($"{Directory.GetCurrentDirectory()}/EmailLayout/layout.cshtml", new
                  {
                      Name = recipientEmail,
                      Id = $"http://localhost:3000/surveys/answer/{id}"
                  });

                try
                {
                    await email.SendAsync();
                }
                catch (Exception ex)
                {

                }
            }
        }
    }

}

