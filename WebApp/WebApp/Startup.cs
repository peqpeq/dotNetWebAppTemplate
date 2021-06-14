using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Contracts.DAL.App;
using Contracts.DAL.Base;
using DAL.App.EF;
using DAL.App.EF.Helpers;
using Domain.App.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebApp
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

            // Database context
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(
                    Configuration.GetConnectionString("LocalPostgre")));
            services.AddDatabaseDeveloperPageExceptionFilter();

            // Identity
            services.AddIdentity<AppUser, AppRole>()
                .AddDefaultUI()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            // add as a scoped dependency
            services.AddScoped<IAppUnitOfWork, AppUnitOfWork>();
            services.AddScoped<IUserNameProvider, UserNameProvider>();
            
            
            services.AddControllersWithViews();

            //API thing
            services.AddCors(options =>
            {
                options.AddPolicy("CorsAllowAll",
                    builder =>
                    {
                        builder.AllowAnyOrigin();
                        builder.AllowAnyHeader();
                        builder.AllowAnyMethod();
                    });
            });
            // =============== JWT support ===============
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // => remove default claims
            services
                .AddAuthentication()
                .AddCookie(options => { options.SlidingExpiration = true; })
                .AddJwtBearer(cfg =>
                {
                    cfg.RequireHttpsMetadata = false;
                    cfg.SaveToken = true;
                    cfg.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = Configuration["JWT:Issuer"],
                        ValidAudience = Configuration["JWT:Issuer"],
                        IssuerSigningKey =
                            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:SigningKey"])),
                        ClockSkew = TimeSpan.Zero // remove delay of token when expire
                    };
                });
            
            // Version control
            services.AddApiVersioning(options => { options.ReportApiVersions = true;});
            services.AddVersionedApiExplorer(options => options.GroupNameFormat = "'v'VVV");
            
            // Add swagger
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            services.AddSwaggerGen();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            // Database updates
            UpdateDatabase(app, env, Configuration);
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            
            //API thing
            app.UseCors("CorsAllowAll");
            
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            // Swagger
            app.UseSwagger();
            app.UseSwaggerUI(
                options =>
                {
                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                            description.GroupName.ToUpperInvariant());
                    }
                });


            
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "area",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}");
                endpoints.MapRazorPages();
            });

        }
        
        
        //Copied, go figure out
        private static void UpdateDatabase(IApplicationBuilder app, IWebHostEnvironment env,
            IConfiguration Configuration)
        {
            // give me the scoped services (everyhting created by it will be closed at the end of service scope life).
            using var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope();

            using var ctx = serviceScope.ServiceProvider.GetService<AppDbContext>();
            using var userManager = serviceScope.ServiceProvider.GetService<UserManager<AppUser>>();
            using var roleManager = serviceScope.ServiceProvider.GetService<RoleManager<AppRole>>();


            if (Configuration["AppDataInitialization:DropDatabase"] == "True")
            {
               DataInitializers.DeleteDatabase(ctx);
            }

            if (Configuration["AppDataInitialization:MigrateDatabase"] == "True")
            {
                DataInitializers.MigrateDatabase(ctx);
            }

            if (Configuration["AppDataInitialization:SeedIdentity"] == "True")
            {
                DataInitializers.SeedIdentity(userManager, roleManager);
            }

            if (Configuration.GetValue<bool>("AppDataInitialization:SeedData"))
            {
                DataInitializers.SeedData(ctx);
            }
        }

    }
}