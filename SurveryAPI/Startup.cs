namespace SurveyAPI
{
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.EntityFrameworkCore;
    using SurveyAPI.APIBehavior;
    using SurveyAPI.Filter;

    public class Startup
    {
       

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
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
                    builder.WithOrigins(frontendURL).AllowAnyMethod().AllowAnyHeader();
                });
            }
            );

            services.AddAutoMapper(typeof(Startup));

            // Add services to the container.
            services.AddControllers(options =>
            {
                options.Filters.Add(typeof(MyExceptionsFilter));
                options.Filters.Add(typeof(ParseBadRequest));  // Adding my BadRequest handler.
            }).ConfigureApiBehaviorOptions(BadRequestBehavior.Parse);

           services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("Development")));
           services.AddLogging();
           services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

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

    }
}
