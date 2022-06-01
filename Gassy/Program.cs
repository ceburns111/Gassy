using Gassy.Helpers;
using Gassy.Services;
using Gassy.Models.ReverbModels;
 using Microsoft.EntityFrameworkCore; 




var builder = WebApplication.CreateBuilder(args);

{
    builder.Services.AddCors();
    builder.Services.AddControllers();
    builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
    builder.Services.AddScoped<IAgentService, AgentService>();
    builder.Services.AddScoped<IReverbListingService, ReverbListingService>();
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
