using Gassy.Helpers;
using Gassy.Services;
using Gassy.Models;
 using Microsoft.EntityFrameworkCore; 



var builder = WebApplication.CreateBuilder(args);

{
    builder.Services.AddCors();
    builder.Services.AddControllers();
    builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
    builder.Services.AddScoped<IAgentService, AgentService>();
    builder.Services.AddScoped<IListingService, ListingService>();
}


//Add Listing DbContext
{
    var host = builder.Configuration.GetConnectionString("DBHOST");
    var port = builder.Configuration.GetConnectionString("DBPORT");
    var password = builder.Configuration.GetConnectionString("MYSQL_PASSWORD");
    var user = builder.Configuration.GetConnectionString("MYSQL_USER");
    var database = builder.Configuration.GetConnectionString("MYSQL_DATABASE");

    builder.Services.AddDbContext<ListingContext>(
        options => options.UseMySQL($"Server={host}; Uid={user}; Pwd={password};Port={port}; Database={database}")
    );
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
