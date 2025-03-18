using System.Reflection;
using Domain.Models.Email;
using Domain.Services.ApiCommunication;
using Domain.Services.DataProcessing;
using Domain.Services.Email;
using Domain.Services.EmailNotification;
using Domain.Services.RocketLaunch;
using Infrastructure.Data;
using Infrastructure.Repositories.ApiCallTrackingRepository;
using Infrastructure.Repositories.EmailRepository;
using Infrastructure.Repositories.RocketLaunch;
using Microsoft.EntityFrameworkCore;
using RocketLauncherNotifier.BackgroundServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var dataStoreConfiguration = builder.Configuration["DataStore"];
var emailConfig = builder.Configuration.GetSection("EmailConfig").Get<EmailConfig>()!;
var initialEmails = emailConfig.ToAddresses.ToArray();

if (string.IsNullOrEmpty(dataStoreConfiguration))
{
    throw new ArgumentException("Please provide a data store configuration.");
}

if (dataStoreConfiguration.ToLower() == "inmemory")
{
    builder.Services.AddSingleton<IEmailRepository>(
        builder.Configuration["ExtractEmailsFrom"] == "Config"
            ? new InMemoryEmailRepository(initialEmails)
            : new InMemoryEmailRepository()
    );

    builder.Services.AddSingleton<IRocketLaunchRepository, InMemoryRocketLaunchRepository>();
    builder.Services.AddSingleton<IApiCallTrackingRepository, InMemoryApiCallTrackingRepository>();

    // Register services as singletons
    builder.Services.AddSingleton<RocketLaunchService>();
    builder.Services.AddSingleton<EmailService>();
    builder.Services.AddSingleton<IDataProcessingService, DataProcessingService>();
    builder.Services.AddSingleton<EmailNotificationService>();
}
else if (dataStoreConfiguration.ToLower() == "sqlite")
{
    builder.Services.AddDbContext<RocketLaunchDbContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
    );

    if (builder.Configuration["ExtractEmailsFrom"] == "Config")
    {
        builder.Services.AddScoped<IEmailRepository>(provider =>
        {
            var context = provider.GetRequiredService<RocketLaunchDbContext>();
            return new SqLiteEmailRepository(context, initialEmails);
        });
    }
    else
    {
        builder.Services.AddScoped<IEmailRepository, SqLiteEmailRepository>();
    }

    builder.Services.AddScoped<IRocketLaunchRepository, SqLiteRocketLaunchRepository>();
    builder.Services.AddScoped<IApiCallTrackingRepository, SqLiteApiCallTrackingRepository>();

    // Register services as scoped to match repository lifetime
    builder.Services.AddScoped<RocketLaunchService>();
    builder.Services.AddScoped<EmailService>();
    builder.Services.AddScoped<IDataProcessingService, DataProcessingService>();
    builder.Services.AddScoped<EmailNotificationService>();
}
else
{
    throw new ArgumentException("Please provide a valid data store configuration.");
}

// These can remain singletons as they don't depend on scoped services
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IApiCommunicationService, ApiCommunicationService>();
builder.Services.AddSingleton(emailConfig);

builder.Services.AddHostedService<RocketLaunchBackgroundService>();
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Get the XML documentation file path
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

namespace RocketLauncherNotifier
{
    public partial class Program { }
}
