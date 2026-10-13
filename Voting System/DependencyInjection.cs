using Hangfire;
using FluentValidation.AspNetCore;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;
using Voting_System.Authentication;
using Voting_System.Authentication.Filters;
using Voting_System.Errors;
using Voting_System.Persistence;
using Voting_System.Settings;
////

namespace Voting_System;

public static class DependencyInjection 
{
    public static IServiceCollection AddDependencies(this IServiceCollection services
                                                     ,IConfiguration configuration)
    {

        
        var ConnectionString = configuration.GetConnectionString("DefaultConnection") ??
            throw new InvalidOperationException
                          ("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<ApplicationDbContext>(
            option => option.UseSqlServer(ConnectionString)
        );
        services.AddControllers();
        services.AddAuthConfig(configuration);
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services
            .AddSwaggerServices()
            .AddMapsterConf()
            .AddValidationConfig();
       
        services.AddScoped<IVoteSubmissionService, VoteSubmissionService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IQuestionService, QuestionService>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IEmailSender, EmailService>();
        services.AddScoped<IVoteService, VoteService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IRoleService, RoleService>();


        //services.AddScoped<IConfiguration, >();


        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
        services.AddBackgroundJobsConfig(configuration);
        services.AddHttpContextAccessor();

        services.Configure<MailSettings>(configuration.GetSection(nameof(MailSettings)));





        return services;
    }
    private static IServiceCollection AddSwaggerServices(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        return services;
    }
    private static IServiceCollection AddMapsterConf(this IServiceCollection services)
    {
        var Mappingconfig = TypeAdapterConfig.GlobalSettings;
        Mappingconfig.Scan(Assembly.GetExecutingAssembly());
        services.AddSingleton<IMapper>(new Mapper(Mappingconfig));

        return services;
    }
    private static IServiceCollection AddValidationConfig(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation()
                .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
       
        return services;
    }
    private static IServiceCollection AddAuthConfig(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IJwtProvider, JwtProvider>();


        services.AddTransient<IAuthorizationHandler,PermissionAuthorizationHandler>();
        services.AddTransient<IAuthorizationPolicyProvider,PermissionAuthorizationPolicyProvider>();

        // services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.AddOptions<JwtOptions>()
                .BindConfiguration(JwtOptions.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

        services.AddCors(
            options=>options.AddPolicy("MyPolicy",builder=>              
                               builder.AllowAnyMethod()
                                      .AllowAnyHeader()
                                      .AllowAnyOrigin()
               )
            );

        var JwtSettings=configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>();

        services.AddAuthentication(options=>
            {  options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
               options.DefaultChallengeScheme= JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                o.SaveToken = true;
                o.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,

                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSettings?.Key!)),
                    ValidIssuer = JwtSettings?.issuer,
                    ValidAudience = JwtSettings?.audience
                };
            });

        services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequiredLength = 8;
            options.User.RequireUniqueEmail = true;
          //  options.SignIn.RequireConfirmedEmail = true;
        });

        services.AddIdentity<ApplicatonUser, ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
        return services;
    }


    private static IServiceCollection AddBackgroundJobsConfig(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddHangfire(config => config
                                        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                                        .UseSimpleAssemblyNameTypeSerializer()
                                        .UseRecommendedSerializerSettings()
                                        .UseSqlServerStorage(configuration.GetConnectionString("HangfireConnection")));

        services.AddHangfireServer();
        return services;
    }

}
