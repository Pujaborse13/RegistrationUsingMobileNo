using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ManagerLayer.Interface;
using ManagerLayer.Service;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RepositoryLayer.Context;
using RepositoryLayer.Interface;
using RepositoryLayer.Service;

namespace FundooNotesApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) //Registers services & dependencies
        {
            //Enables controllers for API.
            services.AddControllers();

            //Registers SQL Server database using a connection string.
            services.AddDbContext<FundooDBContext>(options => options.UseSqlServer(Configuration["ConnectionStrings:DBConn"])); 
           
            services.AddTransient<IUserRepo , UserRepo>();
            services.AddTransient<IUserManager, UserManager>();

            services.AddTransient<INotesManager, NotesManager>();
            services.AddTransient<INotesRepo, NotesRepo>();

            services.AddTransient<ICollaboratorRepo, CollaboratorRepo>();
            services.AddTransient<ICollaboratorManager ,CollaboratorManager>();


            //For Label
            services.AddTransient<ILabelRepo, LabelRepo>();
            services.AddTransient<ILabelManager, LabelManager>();

            services.AddTransient<ITimeZoneConvertorRepo,  TimeZoneConvertorRepo>();
            services.AddTransient<ITimeZoneConvertorManager, TimeZoneConvertorManager>();


            

            services.AddTransient<IUserRegistrationRepo, UserRegistrationRepo>();
            services.AddTransient<IUserRegistrationManager, UserRegistrationManager>();



            //Redis Caching : Adds distributed caching using Redis (for improving performance)
            services.AddStackExchangeRedisCache(options => { options.Configuration = Configuration["RedisCacheUrl"]; });


            //Session Management : Enables server-side session state
            services.AddDistributedMemoryCache();
            services.AddSession(x =>
            {
                x.IdleTimeout = TimeSpan.FromMinutes(30);
                x.Cookie.HttpOnly = true;
                x.Cookie.IsEssential = true;
            });




            //Swagger for API Documentation
            services.AddSwaggerGen(
                option =>
                {
                    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Fundo API", Version = "v1" });
                    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        In = ParameterLocation.Header,
                        Description = "Please enter the valid token",
                        Name = "Authorization",
                        Type = SecuritySchemeType.Http,
                        BearerFormat = "JWT",
                        Scheme = "Bearer"

                    });

                    option.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                      { 
                          new OpenApiSecurityScheme
                          {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }

                          },

                        new string[]{ }

                       }
                    });
             });


            //MassTransit & RabbitMQ Setup
            services.AddMassTransit(x =>
            {
                x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    cfg.UseHealthCheck(provider);
                    cfg.Host(new Uri("rabbitmq://localhost"), h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });
                }));

            });
            
            services.AddMassTransitHostedService();



            //JWT Authentication
            services.AddAuthentication(x =>

            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(o =>
            {
                var Key = Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]);

                o.SaveToken = true;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["Jwt:Issue"],
                    ValidAudience = Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Key)

                };
            });


           services.AddHttpClient();

        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseAuthentication();

            app.UseHttpsRedirection();
            app.UseSwagger();
            app.UseSession();

            // This middleware serves the Swagger documentation UI
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Employee API V1");
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }





    }
}
