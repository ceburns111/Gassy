

using Gassy.Helpers;
using Gassy.Services;
using Gassy.Authorization;
using System.Text.Json.Serialization;


var builder = WebApplication.CreateBuilder(args);

{
    var services = builder.Services;
    var env = builder.Environment;
    builder.Services.AddCors();
    builder.Services.AddControllers().AddJsonOptions(x => {
        x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); 
    });
    builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
    services.AddScoped<IJwtUtils, JwtUtils>();
    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<IListingService, ListingService>();
    builder.Services.AddScoped<IWishlistService, WishlistService>();
}

var app = builder.Build();

{
    app.UseCors(x => x
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());

    
    app.UseMiddleware<ErrorHandlerMiddleware>(); 
    app.UseMiddleware<JwtMiddleware>();
    app.MapControllers();
}


app.Run();
