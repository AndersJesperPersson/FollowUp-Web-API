namespace SurveyAPI
{
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.IdentityModel.Tokens;
    using Quartz;
    using Quartz.Impl;
    using SurveyAPI.APIBehavior;
    using SurveyAPI.Filter;
    using SurveyAPI.Helpers;
    using SurveyAPI.Jobs;
    using System.Collections.Specialized;
    using System.IdentityModel.Tokens.Jwt;
    using System.Net;
    using System.Net.Mail;
    using System.Text;

    public class Startup
    {
    

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
          
        }

        public IConfiguration Configuration { get; }

  
        // Use this method to add services to the container.
        // More info read: https://go.microsoft.com/fwlink/?LinkID=398940
        // 
        
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
                var frontendURL = Configuration.GetValue<string>("frontend_url");  // ändra för prodd. 
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
            services.AddIdentity<IdentityUser, IdentityRole>()               //För authentication....
                 .AddEntityFrameworkStores<ApplicationDbContext>()
                 .AddDefaultTokenProviders();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(Configuration["keyjwt"])),
                ClockSkew = TimeSpan.Zero
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("IsAdmin", policy => policy.RequireClaim("role","admin"));         // to proctect certain endpoints
            });

           services.AddLogging();          // ska jag ha loggning? 

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

            services.AddTransient<IMailSender, MailSender>();
            services.AddTransient<AutoMailJob>();


            

            services.AddQuartz(q =>
            {
                q.UseMicrosoftDependencyInjectionScopedJobFactory();
                var jobkey = new JobKey("sendMail");
                q.AddJob<AutoMailJob>(options=> options.WithIdentity(jobkey));

                q.AddTrigger(options => options
                .ForJob(jobkey)
                .WithIdentity("sendMail-trigger")
                       .StartNow()
        .WithSimpleSchedule(x => x
            .WithIntervalInSeconds(60)
            .RepeatForever()));



                //.WithDailyTimeIntervalSchedule
                //(x=>
                //    x.WithIntervalInHours(12)
                //    .OnEveryDay()
                //    .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(11,00)
                //)
                //));
                  
            });

            services.AddQuartzHostedService(q=> q.WaitForJobsToComplete = true);

        }



        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Configure the HTTP request pipeline.
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FollowUpAPI v1"));
            }

            //The request handling pipeline is composed as a series of middleware components.
            //Each component performs operations on an HttpContext and either invokes the next middleware in the pipeline or terminates the request.
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
       
    }
}
