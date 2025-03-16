//
// using Microsoft.OpenApi.Models;
//
// namespace Pantry.Module.Scanner;
//
// public class Program
// {
//     public static void Main(string[] args)
//     {
//         var builder = WebApplication.CreateBuilder(args);
//
//         // Add services to the container.
//         builder.Services.AddAuthorization();
//
//         // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//         builder.Services.AddEndpointsApiExplorer();
//         builder.Services.AddSwaggerGen(conf =>
//         {
//             conf.AddSecurityDefinition("UserEMail", new OpenApiSecurityScheme
//             {
//                 Description = "EMail of the user",
//                 Name = "UserEMail",
//                 In = ParameterLocation.Header,
//                 Type = SecuritySchemeType.ApiKey,
//                 Scheme = "UserEMailScheme",
//             });
//             var scheme = new OpenApiSecurityScheme
//             {
//                 Reference = new OpenApiReference
//                 {
//                     Type = ReferenceType.SecurityScheme,
//                     Id = "UserEMail"
//                 },
//                 In = ParameterLocation.Header,
//             };
//             var requirement = new OpenApiSecurityRequirement
//             {
//                 { scheme, Array.Empty<string>() }
//             };
//             conf.AddSecurityRequirement(requirement);
//         });
//
//         var app = builder.Build();
//
//         // Configure the HTTP request pipeline.
//         if (app.Environment.IsDevelopment())
//         {
//             app.UseSwagger();
//             app.UseSwaggerUI();
//         }
//
//         app.UseHttpsRedirection();
//
//         app.UseAuthorization();
//
//
//
//         app.Run();
//     }
// }
