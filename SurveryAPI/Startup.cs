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
                var frontendURL = Configuration.GetValue<string>("frontend_url");  // To accept from what url request will be made.  
                options.AddDefaultPolicy(builder =>
                {
                    builder.WithOrigins(frontendURL).AllowAnyMethod().AllowAnyHeader()
                    .WithExposedHeaders(new string[] { "totalAmountOfRecords" });  // to allow the pagination set up. 
                });
            }
            );

            services.AddAutoMapper(typeof(Startup));  // this service enable the function to automap DTO to Models and reverse. 
            services.AddScoped<IFileStorageService, AzureStorageService>();

           
            services.AddControllers(options =>
            {
                options.Filters.Add(typeof(ParseBadRequest));  // Adding my BadRequest handler.
            }).ConfigureApiBehaviorOptions(BadRequestBehavior.Parse);

           services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("ProductionConnection")));
            services.AddIdentity<IdentityUser, IdentityRole>()              
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
                options.AddPolicy("IsAdmin", policy => policy.RequireClaim("role","admin"));      // to proctect certain endpoints
            });

           services.AddLogging();          

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


            //Triggering the job for sending out the mail with the survey. Are set to run every 2 minutes. 

            services.AddQuartz(q =>
            {
                q.UseMicrosoftDependencyInjectionScopedJobFactory();
                var jobkey = new JobKey("sendMail");
                q.AddJob<AutoMailJob>(options=> options.WithIdentity(jobkey));

                q.AddTrigger(options => options
                .ForJob(jobkey)
                .WithIdentity("sendMail-trigger")
                       .WithDailyTimeIntervalSchedule
                       (x =>
                           x.WithIntervalInHours(12)
                           .OnEveryDay()
                           .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(11, 00)
                       )
                       ));



        //               .StartNow()
        //.WithSimpleSchedule(x => x
        //    .WithIntervalInSeconds(120)
        //    .RepeatForever()));


                  
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

            app.UseDeveloperExceptionPage();

            //The request handling pipeline is composed as a series of middleware components.
            //Each component performs operations on an HttpContext and either invokes the next middleware in the pipeline or terminates the request.
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors();

            app.UseAuthentication();

            app.UseAuthorization();

            //Make sure the the controllers are at the last place. 
            app.UseEndpoints(endpoints =>
            { 
            endpoints.MapControllers();
              });

           
        }
       
    }
}
