using BusinessLayer.Constants;
using BusinessLayer.Interfaces.Cashier;
using BusinessLayer.Interfaces.Common;
using BusinessLayer.Interfaces.Doctor;
using BusinessLayer.Interfaces.Notification;
using BusinessLayer.Interfaces.User;
using BusinessLayer.Services.Common;
using BusinessLayer.Services.Doctor;
using BusinessLayer.Services.Notification;
using BusinessLayer.Services.User;
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

namespace HASB_Admin
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
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Admin API - Hospital Appointment Booking", Version = "v1" });
                options.DocumentFilter<KebabCaseDocumentFilter>();

                options.TagActionsBy(api =>
                {
                    var controllerActionDescriptor = api.ActionDescriptor as ControllerActionDescriptor;
                    string controllerName = controllerActionDescriptor.ControllerName;

                    if (api.GroupName != null)
                    {
                        var name = api.GroupName + controllerName.Replace("Controller", "");
                        name = Regex.Replace(name, "([a-z])([A-Z])", "$1 $2");
                        return new[] { name };
                    }

                    if (controllerActionDescriptor != null)
                    {
                        controllerName = Regex.Replace(controllerName, "([a-z])([A-Z])", "$1 $2");
                        return new[] { controllerName };
                    }

                    throw new InvalidOperationException("Unable to determine tag for endpoint.");
                });
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
                  IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["AppSecret:AdminSecret"]))
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
            //admin app
            services.AddTransient<BusinessLayer.Interfaces.User.ICheckupRecordService,
                BusinessLayer.Services.User.CheckupRecordService>();
            services.AddTransient<IDoctorService, DoctorService>();
            services.AddTransient<IPatientService, PatientService>();
            services.AddTransient<BusinessLayer.Interfaces.Doctor.IScheduleService,
                BusinessLayer.Services.Doctor.ScheduleService>();
            services.AddTransient<BusinessLayer.Interfaces.User.IScheduleService,
              BusinessLayer.Services.User.ScheduleService>();
            services.AddTransient<BusinessLayer.Interfaces.Admin.ILoginService,
            BusinessLayer.Services.Admin.LoginService>();
            //services.AddTransient<ILoginService, LoginService>();
            services.AddTransient<BusinessLayer.Interfaces.Doctor.IDepartmentService,
                BusinessLayer.Services.Doctor.DepartmentService>();
            services.AddTransient<BusinessLayer.Interfaces.Doctor.IOperationService,
               BusinessLayer.Services.Doctor.OperationService>();
            services.AddTransient<INumercialOrderService, NumercialOrderService>();

            //services.AddTransient<IVnPayService, VnPayService>();

            services.AddLogging();
            services.AddTransient<ConfigService, ConfigService>();

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
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "HASB_Admin v1"));

            app.UseRewriter(new RewriteOptions().Add(new PascalRule()));

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(options => options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
