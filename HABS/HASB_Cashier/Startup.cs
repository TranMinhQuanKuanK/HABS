using BusinessLayer.Constants;
using BusinessLayer.Interfaces.Cashier;
using BusinessLayer.Interfaces.Doctor;
using BusinessLayer.Interfaces.Notification;
using BusinessLayer.Services.Cashier;
using BusinessLayer.Services.Common;
using BusinessLayer.Services.Doctor;
using BusinessLayer.Services.Notification;
using DataAccessLayer.Models;
using DataAcessLayer;
using DataAcessLayer.Interfaces;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Utilities;
using ILoginService = BusinessLayer.Interfaces.Cashier.ILoginService;
using LoginService = BusinessLayer.Services.Cashier.LoginService;

namespace HASB_Cashier
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
            services.AddDistributedMemoryCache();
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = "redis-16974.c89.us-east-1-3.ec2.cloud.redislabs.com:16974,password=bbMlurx8k7lOY1Jv4YqYZJ8VGXY8xhgs,ssl=False,abortConnect=False";
                options.InstanceName = "HospitalRedisCache";
            });
            services.AddRouting(option =>
            {
                option.ConstraintMap["slugify"] = typeof(SlugifyParameterTransformer);
            });
            services.AddDbContext<HospitalAppointmentBookingContext>(
               options => options.UseSqlServer(Configuration.GetConnectionString("HospitalCloud")));

            services.AddControllers();
            services.AddControllers(options =>
            {
                options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
            });

            services.AddSwaggerGen(options =>
            {
                options.EnableAnnotations();
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Cashier API - Hospital Appointment Booking", Version = "v1" });
                options.DocumentFilter<KebabCaseDocumentFilter>();
                var jwtSecurityScheme = new OpenApiSecurityScheme
                {
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Name = "JWT Authentication",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",

                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };

                options.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                            {
                                { jwtSecurityScheme, Array.Empty<string>() }
                            });

                options.DocInclusionPredicate((name, api) => true);
            });
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
          .AddJwtBearer(options =>
          {
              options.TokenValidationParameters = new TokenValidationParameters
              {
                  ValidateIssuer = true,
                  ValidateAudience = true,
                  ValidateLifetime = true,
                  ValidateIssuerSigningKey = true,
                  ValidIssuer = "http://localhost:2000",
                  ValidAudience = "http://localhost:2000",
                  IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["AppSecret:CashierSecret"]))
              };
          });

            services.AddSingleton(FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile(Configuration["Firebase:Admin"]),
            })
           );

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            //config
            services.AddSingleton<ConfigService, ConfigService>();
            services.AddSingleton<BaseConfig, BaseConfig>();
            //cashier app
            services.AddTransient<IBillService, BillService>();
            services.AddTransient<ILoginService, LoginService>();
            services.AddTransient<IScheduleService, ScheduleService>();
            services.AddTransient<INotificationService, NotificationService>();
            services.AddTransient<BusinessLayer.Interfaces.User.ICheckupRecordService, BusinessLayer.Services.User.CheckupRecordService>();
            services.AddTransient<BusinessLayer.Interfaces.User.IScheduleService, BusinessLayer.Services.User.ScheduleService>();
            services.AddTransient<BusinessLayer.Interfaces.Doctor.IDepartmentService, BusinessLayer.Services.Doctor.DepartmentService>();
            services.AddTransient<BusinessLayer.Interfaces.Doctor.IOperationService, BusinessLayer.Services.Doctor.OperationService>();
            services.AddTransient<BusinessLayer.Interfaces.Common.INumercialOrderService, BusinessLayer.Services.Common.NumercialOrderService>();


            //Firebase messaging
            services.AddTransient<INotificationService, NotificationService>();
            services.AddTransient<BusinessLayer.Interfaces.Notification.IFCMTokenService,
               BusinessLayer.Services.Notification.FCMTokenService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.ApplicationServices.GetService<ConfigService>();
            app.ApplicationServices.GetService<BaseConfig>();

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "HASB_Cashier v1"));

            app.UseRewriter(new RewriteOptions().Add(new PascalRule()));

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(options => options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

            app.UseAuthentication();
            //if (!env.IsDevelopment())
            //{
                app.UseAuthorization();
            //}

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
