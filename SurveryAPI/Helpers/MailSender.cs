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

        public async void SendPlainGmail(string recipientEmail)
        {
            using (var scope = _serviceProvider.CreateScope())
            {

                var mailer = scope.ServiceProvider.GetRequiredService<IFluentEmail>();
                var email = mailer
                  .To(recipientEmail, "Employee")
                  .Subject("Uppföljning")
                  .Body(MailDesign().ToString());
     
                try
                {
                    await email.SendAsync();
                }
                catch (Exception ex)
                {

                }
            }
        }


        private static StringBuilder MailDesign()
        {
            StringBuilder template = new();
            template.AppendLine("<h3>Hej,</h3>");
            template.AppendLine(@"<p>För sex månader sedan var företagshälsovården delaktiga i ett ärende på din arbetsplats. Eftersom det är viktigt för oss att veta om interventioner var lyckade
                eller om det krävs andra åtgärder för att åstadkomma en förändring så följer vi nu upp hur resultatet blivit.
                För att få en så trovärdig och talande bild som möjligt är det viktigt att du lyckas hitta tid att fylla i denna enkät.  Var god gå in och svara på länken nedan. 
                Alla svar är anonyma och länken är inte kopplad till dig eller din mail. När du svart på enkäten och tryckt på knappen ''skicka in'' så kommer även länken
                inaktiveras så det inte går att svara mer än en gång.
                Känner du oro för att trycka på länkar? Dubbelkolla i så fall med din chef som kan intyga att detta mail ska ha gått ut till berörda medarbetare/p>");
            template.AppendLine("<p> Med vänliga hälsningar</p>");
            template.AppendLine("- Avonova");
            return template;
        }

        public async void SendHtmlGmail(string recipientEmail)
        {
            using (var scope = _serviceProvider.CreateScope())
            {

                var mailer = scope.ServiceProvider.GetRequiredService<IFluentEmail>();
                var email = mailer
                  .To(recipientEmail, "Employee")
                  .Subject("Uppföljning")
                  .UsingTemplateFromFile($"{Directory.GetCurrentDirectory()}/EmailLayout/layout.cshtml", new
                  {
                      Name = "Jesper",
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

