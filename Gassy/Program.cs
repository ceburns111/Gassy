using Gassy.Helpers;
using Gassy.Services;


var builder = WebApplication.CreateBuilder(args);

{
    var services = builder.Services;
    services.AddCors();
    services.AddControllers();
    services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
    services.AddScoped<IAgentService, AgentService>();
    

}

var app = builder.Build();

{
     app.UseCors(x => x
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());

     app.UseMiddleware<JwtMiddleware>();

     app.MapControllers();
}


    app.Run();
