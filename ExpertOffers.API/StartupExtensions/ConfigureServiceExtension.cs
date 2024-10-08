using ExpertOffers.API.FileServices;
using ExpertOffers.Core.Domain.IdentityEntities;
using ExpertOffers.Core.DTOS.AuthenticationDTO;
using ExpertOffers.Core.IUnitOfWorkConfig;
using ExpertOffers.Core.MappingProfile;
using ExpertOffers.Core.Services;
using ExpertOffers.Core.ServicesContract;
using ExpertOffers.Infrastructure.Data;
using ExpertOffers.Infrastructure.UnitOfWorkConfig;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace ExpertOffers.API.StartupExtensions
{
    public static class ConfigureServiceExtension
    {
        public static IServiceCollection ServiceConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSignalR();
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });
            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 5;
                options.Tokens.EmailConfirmationTokenProvider = "Default";
            })
               .AddEntityFrameworkStores<ApplicationDbContext>()
               .AddDefaultTokenProviders()
               .AddUserStore<UserStore<ApplicationUser, ApplicationRole, ApplicationDbContext, Guid>>()
               .AddRoleStore<RoleStore<ApplicationRole, ApplicationDbContext, Guid>>();
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
               .AddJwtBearer(o =>
               {
                   o.RequireHttpsMetadata = false;
                   o.SaveToken = false;
                   o.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidateIssuerSigningKey = true,
                       ValidateIssuer = true,
                       ValidateAudience = true,
                       ValidateLifetime = true,
                       ValidIssuer = configuration["JWT:Issuer"],
                       ValidAudience = configuration["JWT:Audience"],
                       IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"])),
                       ClockSkew = TimeSpan.Zero
                   };
               });
            services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromHours(1);
            });
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod();
                });
            });
            services.Configure<JwtDTO>(configuration.GetSection("JWT"));
            services.AddAutoMapper(typeof(CountryConfig));
            services.AddScoped<IAuthenticationServices, AuthenticationServices>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IFileServices, FileService>();
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddScoped<ICityServices, CityServices>();
            services.AddScoped<ICountryServices, CountryServices>();
            services.AddScoped<IClientServices, ClientServices>();
            services.AddScoped<ICompanyServices, CompanyServices>();
            services.AddScoped<IBranchServices, BranchServices>();
            services.AddScoped<IIndustrialServices, IndustrialServices>();
            services.AddScoped<IGenreOfferServices, GenreOfferServices>();
            services.AddScoped<IOfferServices, OfferServices>();
            services.AddScoped<IGenreCouponServices, GenreCouponServices>();
            services.AddScoped<ICouponServices, CouponServices>();
            services.AddScoped<IFavoriteServices, FavoriteServices>();
            services.AddScoped<IBulletinGenreServices, BulletinGenreServices>();
            services.AddScoped<IBulletinServices, BulletinServices>();
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Expert Offers APP", Version = "v1" });
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "api.xml"));
            });

            //services.AddAuthentication().AddFacebook(opt =>
            //{
            //    opt.AppId = configuration["Authentication:Facebook:AppId"];
            //    opt.AppSecret = configuration["Authentication:Facebook:AppSecret"];
            //    opt.Scope.Add("email");
            //    opt.Fields.Add("name");
            //    opt.Fields.Add("email");
            //    opt.SaveTokens = true;
            //});

            return services;
        }
    }
}
