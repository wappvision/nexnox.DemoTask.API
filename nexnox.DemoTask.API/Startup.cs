using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using nexnox.DemoTask.API.Data;
using System.Text;

namespace nexnox.DemoTask.API
{
    public class Startup
    {
        public const string SHA_JWT_KEY = "secretkeysecretkeysecretkeysecretkey";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(o => o.UseInMemoryDatabase(databaseName: "nexnox.DemoTask.API"));

            services.AddTransient<DbInitializer>();

            services
                .AddIdentityCore<IdentityUser>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            services
                .AddAuthentication(o =>
                {
                    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    o.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(o =>
                {
                    o.RequireHttpsMetadata = false;
                    o.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateAudience = false,
                        ValidateIssuer = false,
                        ValidateLifetime = false,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SHA_JWT_KEY))
                    };
                });


            var tokenProvider = new ShaJwtTokenProvider<IdentityUser>("nexnox", "demo", SHA_JWT_KEY);
            services.AddSingleton<ITokenProvider<IdentityUser>>(tokenProvider);

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Read", policy => policy.RequireClaim("Permission", "ReadPost"));
                options.AddPolicy("Write", policy => policy.RequireClaim("Permission", "WritePost"));
            });

            services.Configure<IdentityOptions>(options => {
                options.User.RequireUniqueEmail = true;
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 3;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
            });
            
            services.AddCors();
            
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, DbInitializer dbInitializer)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(o => o
                .AllowAnyHeader()
                .AllowAnyOrigin()
                .AllowAnyMethod()
            );
            app.UseAuthentication();
            app.UseMvc();

            dbInitializer.Seed().Wait();
        }
    }
}
