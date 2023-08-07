using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi.Models;

namespace ApiB
{
    public class Program
    {
        // Update the following:
        //   ClientID
        //   Domain e.g. domain.onmicrosoft.com
        //   Tenant
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);
            IdentityModelEventSource.ShowPII = true;
            // Add services to the container.

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddMicrosoftIdentityWebApi(
                jwtOptions =>
                {
                    jwtOptions.Events = new JwtBearerEvents()
                    {
                        OnTokenValidated = ctx =>
                        {
                            var jwtSecurityToken = ctx.SecurityToken as JwtSecurityToken;
                            var raw = jwtSecurityToken.RawData;
                            return Task.CompletedTask;
                        }
                    };
                }, idOptions =>
                {
                    idOptions.ClientId = "a251cf10-30ff-4b76-81a1-xxxxxxxxxxxx";
                    idOptions.Domain = "MyDomain.onmicrosoft.com";
                    idOptions.TenantId = "9b3ea83d-ef72-4f7e-bf5c-xxxxxxxxxxxx";
                    idOptions.Instance = "https://login.microsoftonline.com/";
                   
                });

            builder.Services.AddControllers(mvcOptions =>
            {
                mvcOptions.Filters.Add(new AuthorizeFilter());
            });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthentication(); 
            
            app.MapControllers();

            app.Run();
        }
    }
}