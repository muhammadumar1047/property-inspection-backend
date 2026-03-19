using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PropertyInspection.API.Authorization;
using PropertyInspection.API.Middleware;
using PropertyInspection.Application.IServices;
using PropertyInspection.Application.IServices.Notification.Hubs;
using PropertyInspection.Application.Services;
using PropertyInspection.Application.Mapping;
using PropertyInspection.Core.Interfaces.Repositories;
using PropertyInspection.Core.Interfaces.UnitOfWork;
using PropertyInspection.Infrastructure.Auth;
using PropertyInspection.Infrastructure.Data;
using PropertyInspection.Infrastructure.Data.Seeder;
using PropertyInspection.Infrastructure.Repositories;
using PropertyInspection.Infrastructure.UnitOfWork;
using PropertyInspection.Shared;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 8;
    options.SignIn.RequireConfirmedEmail = true;
    options.Tokens.AuthenticatorIssuer = "PropertyInspection.SaaS";
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});

builder.Services.AddAuthorization();
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

builder.Services.Configure<AwsSettings>(builder.Configuration.GetSection("AWS"));
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

builder.Services.AddScoped<TenantContext>();
builder.Services.AddScoped<ITenantContext>(sp => sp.GetRequiredService<TenantContext>());
builder.Services.AddScoped<ITenantAgencyResolver, TenantAgencyResolver>();
builder.Services.AddScoped<IPermissionCacheService, PermissionCacheService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IUserAuthService, IdentityAuthService>();
builder.Services.AddScoped<IAgencyRoleProvisioningService, RoleSeeder>();
var awsOptions = builder.Configuration.GetAWSOptions();
var awsSettings = builder.Configuration.GetSection("AWS").Get<AwsSettings>();
if (!string.IsNullOrWhiteSpace(awsSettings?.Region))
{
    awsOptions.Region = RegionEndpoint.GetBySystemName(awsSettings.Region);
}
if (!string.IsNullOrWhiteSpace(awsSettings?.AccessKey) &&
    !string.IsNullOrWhiteSpace(awsSettings?.SecretKey))
{
    awsOptions.Credentials = new BasicAWSCredentials(awsSettings.AccessKey, awsSettings.SecretKey);
}
builder.Services.AddDefaultAWSOptions(awsOptions);
builder.Services.AddAWSService<IAmazonS3>();

builder.Services.AddScoped<IAgencyService, AgencyService>();
builder.Services.AddScoped<IAgencyWhitelabelService, AgencyWhitelabelService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
builder.Services.AddScoped<IInspectionService, InspectionService>();
builder.Services.AddScoped<IPropertyService, PropertyService>();
builder.Services.AddScoped<IPropertyLayoutService, PropertyLayoutService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IReportSyncService, ReportSyncService>();
builder.Services.AddScoped<IReportTemplateService, ReportTemplateService>();
builder.Services.AddScoped<IMobileReportTemplateService, MobileReportTemplateService>();
builder.Services.AddScoped<IS3Service, S3Service>();
builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddScoped<ILookupService, LookupService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IMobileDashboardService, MobileDashboardService>();
builder.Services.AddScoped<IMobileProfileService, ProfileService>();
builder.Services.AddScoped<IMobileInspectionService, MobileInspectionService>();

builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:3000") // frontend origin
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials(); // must if frontend uses withCredentials
    });
});


builder.Services.AddSignalR();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var db = services.GetRequiredService<AppDbContext>();
    var roleSeeder = services.GetRequiredService<IAgencyRoleProvisioningService>();

    await PermissionSeeder.SeedAsync(db);
    await roleSeeder.EnsureDefaultRolesForAllAgenciesAsync();
    await IdentitySeeder.SeedSuperAdminAsync(services);
}
app.MapHub<NotificationHub>("/notificationHub");
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("AllowFrontend");
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<TenantContextMiddleware>();

app.MapControllers();
app.MapGet("/health", () => Results.Ok("ok"));

app.Run();
