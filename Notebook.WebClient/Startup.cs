using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Notebook.Database;
using Notebook.DTO.Mapping;
using Notebook.WebClient.Services;
using System;
using System.IO;
using System.Reflection;

namespace Notebook.WebClient
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
            services.AddControllersWithViews();
            var schema = Configuration.GetValue<string>("SchemaName");

            services.AddDbContext<NotebookDbContext>(opt =>
                opt.UseNpgsql(Configuration.GetConnectionString("NotebookConnection"), x =>
                        x.MigrationsHistoryTable("_migrations_history", schema))
                    .UseSnakeCaseNamingConvention());

            services.AddScoped<ContactService>();
            services.AddScoped<NotebookService>();
            //*services.AddAutoMapper(typeof(Startup));
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            // Register the Swagger generator
            services.AddSwaggerGen(c =>
            {
                // https://localhost:5001/swagger/v1/swagger.json  open API specification available
                c.SwaggerDoc("v1", new OpenApiInfo //part of Url
                {
                    Version = "v1",
                    Title = "API for work with Notebook",
                    Description = "This app represent two option which can be used like contact notebook and notes for daily deals",
                    Contact = new OpenApiContact()
                    {
                        Email = "MissValeriV@mail.ru",
                        Name = "Valeriia Vaganova",
                        Url = new Uri("https://www.facebook.com/valeriia.vaganova.9/")
                    }
                });

                //c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                //{
                //    Description = @"JWT Authorization header using the Bearer scheme." + Environment.NewLine +
                //                  "Enter 'Bearer' [space] and then your token in the text input below." + Environment.NewLine +
                //                  "Example: 'Bearer 12345abcdef'",
                //    Name = "Authorization",
                //    In = ParameterLocation.Header,
                //    Type = SecuritySchemeType.ApiKey,
                //    Scheme = "Bearer"
                //});

                //c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                //{
                //    {
                //        new OpenApiSecurityScheme
                //        {
                //            Reference = new OpenApiReference
                //            {
                //                Type = ReferenceType.SecurityScheme,
                //                Id = "Bearer"
                //            },
                //            Scheme = "oauth2",
                //            Name = "Bearer",
                //            In = ParameterLocation.Header,

                //        },
                //        new List<string>()
                //    }
                //});

                var xmlCommentsFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFile);
                var xmlDtoPath = Path.Combine(AppContext.BaseDirectory, "Notebook.DTO.xml");

                c.IncludeXmlComments(xmlCommentsFullPath);
                c.IncludeXmlComments(xmlDtoPath);
            });

            services.AddSwaggerGenNewtonsoftSupport();


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint
            app.UseSwagger();

            // Specify the Swagger endpoint
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"v1/swagger.json", $"Notebook v1");
                c.DisplayRequestDuration();
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
