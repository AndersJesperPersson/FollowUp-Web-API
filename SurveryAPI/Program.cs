namespace SurveyAPI;

public class Program
{

    // Entry point, building the host and set alot of deafult settings. 
    // Goes on to Start up class. 
    public static void Main(string [] args)
    {
        CreateHostBuilder(args).Build().Run();
    }
    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder =>
        {
 
            webBuilder.UseStartup<Startup>();
        });
}