using Hangfire;
using Serilog;
using HangfireBasicAuthenticationFilter;
using Microsoft.AspNetCore.Identity;
using Voting_System;
using Voting_System.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDependencies(builder.Configuration);

builder.Host.UseSerilog((context, configuration) =>configuration.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseSerilogRequestLogging();
app.UseHttpsRedirection();

app.UseHangfireDashboard("/jobs", new DashboardOptions
{
    Authorization = [
        new HangfireCustomBasicAuthenticationFilter{
            User=app.Configuration.GetValue<string>("HangfireSettings:Username"),
            Pass=app.Configuration.GetValue<string>("HangfireSettings:Password")
        }
        ]
  //  IsReadOnlyFunc = true
});
var scopeFactory=app.Services.GetRequiredService<IServiceScopeFactory>();
using var scope = scopeFactory.CreateScope();
var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
RecurringJob.AddOrUpdate("SendNewVoteNotification", () => notificationService.SendNewVoteNotification(null), Cron.Daily);
app.UseCors("MyPolicy");
app.UseAuthentication();

app.UseAuthorization();


app.MapControllers();

app.UseExceptionHandler();

app.Run();
