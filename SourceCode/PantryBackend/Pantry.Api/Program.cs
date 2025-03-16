using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Pantry.Api.Authentication;
using Pantry.Module.Authentication.UserServices;
using Pantry.Module.Recipe.Database.Contexts;
using Pantry.Module.Recipe.Extensions;
using Scalar.AspNetCore;
using Wolverine;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Host.UseWolverine(o =>
{
    o.Durability.Mode = DurabilityMode.MediatorOnly;
});

List<Assembly> mediatrAssemblies = [typeof(Program).Assembly];
builder.AddRecipeModule(mediatrAssemblies);

builder.Services.AddMediatR(c => { c.RegisterServicesFromAssemblies(mediatrAssemblies.ToArray()); });
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<IHeaderEMailService, HeaderEMailService>();

builder.Services.AddPantryAuthentication(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

// app.UseHttpsRedirection();
// app.UseAuthentication();
// app.UseAuthorization();

//app.MapGroup("/").AuthenticationEndpoints();

app.MapRecipeEndpoints();


app.Run();

public partial class Program { }
