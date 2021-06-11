namespace ServicesApi
{
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.OpenApi.Models;

    using ServicesApi.Filters;
    using ServicesApi.Utilities;

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
            services.AddAutoMapper(typeof(Startup));
            services.AddTransient<IApplicationAzureStorage, ApplicationAzureStorage>();
            services.AddDbContext<ApplicationDbContext>(
                options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
            services.AddCors(
                options =>
                    {
                        var allowOrigins = Configuration.GetValue<string>("AllowOrigins");
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