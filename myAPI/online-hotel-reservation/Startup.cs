using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using online_hotel_reservation.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace online_hotel_reservation
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddDbContext<ApplicationDbContext>(opt => opt.UseInMemoryDatabase("OnlineHotelResevation"));
            // services.AddIdentity<IdentityUser,IdentityRole>()
            //         .AddEntityFrameworkStores<ApplicationDbContext>()
            //         .AddDefaultTokenProviders();
            services.AddAuthentication(x => {
                                        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                                        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
                    {
                        options.SaveToken= true;
                        options.TokenValidationParameters = new TokenValidationParameters
                        {

                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("this-is-my-secret-key")),
                            ValidateIssuer = false,
                            ValidateAudience = false
                        };
                    });
            // services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            // .AddJwtBearer(options =>
            // {
            //     options.TokenValidationParameters = new TokenValidationParameters
            //     {
            //         ValidateIssuerSigningKey = true,
            //         IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII
            //             .GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
            //         ValidateIssuer = false,
            //         ValidateAudience = false
            //     };
            // });
            // services.AddSwaggerGen(c => {
            //         c.SwaggerDoc("v1", new OpenApiInfo { Title = "JWTToken_Auth_API", Version = "v1"});
            //         c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme() {
            //                 Name = "Authorization",
            //                 Type = SecuritySchemeType.ApiKey,
            //                 Scheme = "Bearer",
            //                 BearerFormat = "JWT",
            //                 In = ParameterLocation.Header,
            //                 Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
            //         });
            //         c.AddSecurityRequirement(new OpenApiSecurityRequirement {
            //             {
            //                 new OpenApiSecurityScheme {
            //                     Reference = new OpenApiReference {
            //                         Type = ReferenceType.SecurityScheme,
            //                             Id = "Bearer"
            //                     }
            //                 },
            //                 new string[] {}
            //             }
            //         });
            //     });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "online_hotel_reservation", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme{
                    Description ="JWT Authorization",
                    Name ="Authorization",
                    In= ParameterLocation.Header,
                    Type= SecuritySchemeType.ApiKey,
                    Scheme="Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type= ReferenceType.SecurityScheme,
                                Id="Bearer"
                            },
                            Scheme="oauth2",
                            Name="Bearer",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "online_hotel_reservation v1"));
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
