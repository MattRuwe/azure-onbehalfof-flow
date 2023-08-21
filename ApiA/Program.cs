using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace ApiA
{
    // Update the following:
    //   ClientID
    //   Domain e.g. domain.onmicrosoft.com
    //   TenantId
    //   ClientSecret
    //   Scope
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);
            
            // Add services to the container.
            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApi(
                jwtOptions =>
                {
                    
                }, idOptions =>
                {
                    idOptions.ClientId = "742bec59-5ad6-4c85-b043-xxxxxxxxxxxx";
                    idOptions.Domain = "omaha.dev";
                    idOptions.TenantId = "a47078d6-821c-4b28-a3a5-xxxxxxxxxxxx";
                    idOptions.Instance = "https://login.microsoftonline.com/";
                })
                .EnableTokenAcquisitionToCallDownstreamApi(options =>
                {
                    options.ClientSecret = "6SE8Q~xxxxxxxxxxxx";
                    options.EnablePiiLogging = true;
                })
                .AddDownstreamApi("ApiB", options =>
                {
                    options.Scopes = new[] { "api://4c6cdc5e-4438-4b05-9441-xxxxxxxxxxxx/user_impersonation" };
                    options.BaseUrl = "http://localhost:5165/";
                })
                .AddInMemoryTokenCaches();


            builder.Services.AddControllers(mvcOptions =>
            {
                mvcOptions.Filters.Add(new AuthorizeFilter());
            });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyHeader();
                    policy.AllowAnyMethod();
                    policy.AllowAnyOrigin();
                });
            });
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthentication();
            app.UseCors();
            app.MapControllers();
            IdentityModelEventSource.ShowPII = true;
            app.Run();
        }
    }
}