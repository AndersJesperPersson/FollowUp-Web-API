namespace SurveyAPI
{
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.EntityFrameworkCore;
    using Quartz;
    using Quartz.Impl;
    using SurveyAPI.APIBehavior;
    using SurveyAPI.Filter;
    using SurveyAPI.Helpers;
    using System.Collections.Specialized;
    using System.Net;
    using System.Net.Mail;

    public class Startup
    {
       private IScheduler _scheduler;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            _scheduler = ConfigureQuartz();
        }

        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c => 
            { 
                c.SwaggerDoc("v1", new() { Title = "FollowUpAPI", Version = "v1" }); 
            });

            services.AddCors(options =>
            {
                var frontendURL = Configuration.GetValue<string>("frontend_url");
                options.AddDefaultPolicy(builder =>
                {
                    builder.WithOrigins(frontendURL).AllowAnyMethod().AllowAnyHeader()
                    .WithExposedHeaders(new string[] { "totalAmountOfRecords" }); // TODO: LÄS PÅ! 
                });
            }
            );

            services.AddAutoMapper(typeof(Startup));  // så jag kan omvandla DTO till models. 
            services.AddScoped<IFileStorageService, AzureStorageService>();

            // Add services to the container.
            services.AddControllers(options =>
            {
                options.Filters.Add(typeof(MyExceptionsFilter));
                options.Filters.Add(typeof(ParseBadRequest));  // Adding my BadRequest handler.
            }).ConfigureApiBehaviorOptions(BadRequestBehavior.Parse);

           services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("Development")));
           services.AddLogging();
           services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

            var from = Configuration.GetSection("Mail")["From"];                      // config sender host. 
            var gmailSender = Configuration.GetSection("Gmail")["Sender"];
            var password = Configuration.GetSection("Gmail")["Password"];
            var port = Convert.ToInt32(Configuration.GetSection("Gmail")["Port"]);

            services.AddFluentEmail(gmailSender, from)
                .AddRazorRenderer()
                .AddSmtpSender(new SmtpClient("smtp.gmail.com")
                {
                    UseDefaultCredentials = false,
                    Port = port,
                    Credentials = new NetworkCredential(gmailSender, password),
                    EnableSsl = true
                });

            services.AddScoped<IMailSender, MailSender>();

            services.AddSingleton(provider => _scheduler);

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Configure the HTTP request pipeline.
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FollowUpAPI v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors();

            app.UseAuthentication();

            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            { 
            endpoints.MapControllers();
              });
        }
        public IScheduler ConfigureQuartz()
        {
            NameValueCollection props = new()
            {
                {
                    "quartz.serializer.type",
                    "binary"
                },
            };
            StdSchedulerFactory factory = new StdSchedulerFactory(props);
            var scheduler = factory.GetScheduler().Result;
            scheduler.Start().Wait();

            return scheduler;
            
        }
    }
}
