using CBTClinic.Application.Interfaces.Repositories;
using CBTClinic.Domain.Entities;
using CBTClinic.Infrastructure.Data;
using CBTClinic.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBTClinic.Infrastructure.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            // Database
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection")));
            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.AddScoped<IEmailService, EmailService>();



            // JWT
            var key = Encoding.UTF8.GetBytes(config["JWT:Key"]);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = config["JWT:Issuer"],
                    ValidAudience = config["JWT:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine("JWT Authentication Failed: " + context.Exception.Message);
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            Console.WriteLine("Token has expired.");
                        }
                        return Task.CompletedTask;
                    },
                    OnChallenge = async context =>
                    {
                        context.HandleResponse();

                        context.Response.ContentType = "application/json";
                        context.Response.StatusCode = 401;

                        string message = context.Error switch
                        {
                            "invalid_token" => "Invalid token.",
                            "expired_token" => "Token has expired.",
                            _ => "Unauthorized: Missing or invalid token."
                        };

                        var errorObj = new
                        {
                            error = message,
                            details = context.ErrorDescription
                        };

                        var json = JsonConvert.SerializeObject(errorObj);
                        await context.Response.WriteAsync(json);
                    }
                };
            });

            return services;
        }
    }
}
