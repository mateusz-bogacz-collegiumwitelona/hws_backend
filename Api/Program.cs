using Api.Config;
using Api.Middleware;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// services 
builder.Services.AddSwaggerConfiguration();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddServices(builder.Configuration);
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var dbContext =  scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();

    bool isSeederActie = app.Configuration.GetValue<bool>("ISSEEDERACTIVE", false);
    
    if (isSeederActie) SeedData.Seed(dbContext);
}

app.UseSwaggerUIConfiguration();
app.UseExceptionHandler();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();
