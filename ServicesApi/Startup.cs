namespace ServicesApi
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Text;

    using AutoMapper;

    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.OpenApi.Models;

    using NetTopologySuite;
    using NetTopologySuite.Geometries;

    using ServicesApi.Filters;
    using ServicesApi.Utilities;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(typeof(Startup));
            services.AddSingleton(provider =>
                        new MapperConfiguration(config =>
                                {
                                    var geometryFactory = provider.GetRequiredService<GeometryFactory>();
                                    config.AddProfile(new AutoMapperProfiles(geometryFactory));
                                }).CreateMapper());

            services.AddSingleton<GeometryFactory>(NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326));
            services.AddTransient<ITokenGenerator, TokenGenerator>();
            services.AddTransient<IApplicationAzureStorage, ApplicationAzureStorage>();

            services.AddHttpContextAccessor();
            services.AddDbContext<ApplicationDbContext>(
                options => options.UseSqlServer(
                    this.Configuration.GetConnectionString("DefaultConnection"),
                    sqlServer => sqlServer.UseNetTopologySuite()));
            services.AddCors(
                options =>
                    {
                        var allowOrigins = this.Configuration.GetValue<string>("AllowOrigins");
                        options.AddDefaultPolicy(
                            builder =>
                                {
                                    builder
                                        .WithOrigins(allowOrigins)
                                        .AllowAnyMethod()
                                        .AllowAnyHeader()
                                        .WithExposedHeaders(new string[] { "totalRecords" });
                                });
                    });

            services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.Configuration.GetValue<string>("JWT_SECRET_KEY"))),
                        ClockSkew = TimeSpan.Zero
                    });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("IsAdministrator", policy => policy.RequireClaim("role", "Administrator"));
            });

            services.AddControllers(
                options =>
                    {
                        options.Filters.Add(typeof(CustomExceptionFilter));
                    });
            services.AddSwaggerGen(
                c =>
                    {
                        c.SwaggerDoc("v1", new OpenApiInfo { Title = "ServicesApi", Version = "v1" });
                    });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ServicesApi v1"));
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseCors();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(
                endpoints =>
                    {
                        endpoints.MapControllers();
                    });
        }
    }
}