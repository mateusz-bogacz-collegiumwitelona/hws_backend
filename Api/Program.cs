using Api.Config;
using Api.Middleware;
using Domain.Models;
using Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    String frontendUrl = builder.Configuration["FRONTEND:URL"] ?? "http://localhost:5173";
    options.AddPolicy("AllowFront", policy =>
    {
        policy.WithOrigins(frontendUrl)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddRouting(options => options.LowercaseUrls = true);

// services 
builder.Services.AddSwaggerConfiguration();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddServices(builder.Configuration);
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var dbContext =  scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    
    dbContext.Database.Migrate();
    
    bool isSeederActie = app.Configuration.GetValue<bool>("ISSEEDERACTIVE", false);
    
    if (isSeederActie) 
    {
        var seeder = new SeedData(userManager);
        await seeder.Seed(dbContext);
    }
}

app.UseSwaggerUIConfiguration();

//app.UseHttpsRedirection();
app.UseExceptionHandler();

app.UseRouting();
app.UseCors("AllowFront");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers().RequireAuthorization();

app.Run();
