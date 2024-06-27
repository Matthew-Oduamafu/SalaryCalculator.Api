using System.Reflection;
using FluentValidation;
using FluentValidation.AspNetCore;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using SalaryCalculator.Api;
using SalaryCalculator.Api.Extensions;
using SalaryCalculator.Api.Extensions.EndpointsExtensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDatabaseConfiguration(builder.Configuration);
builder.Services.AddRepositories();
builder.Services.AddServices();
builder.Services.AddHttpContextAccessor();

builder.Services.AddControllerConfiguration();
builder.Services.AddConfigurations();

builder.Services.AddCors(options => options
    .AddPolicy(CommonConstants.CorsPolicyName, policy => policy
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod()));


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerDocumentation();

builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddFluentValidationRulesToSwagger();

/***
 * Authentication and Authorization
 */
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

var app = builder.Build();

{
    var logger = app.Logger;
    await app.MigrateDatabaseAsync();
// Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
        app.UseDeveloperExceptionPage();
    else
        app.UseCustomExceptionHandler(logger);

    app.UseSwaggerDocumentation();

    app.UseCors(CommonConstants.CorsPolicyName);
    app.UseRouting();
    app.UseHttpsRedirection();
    
    app.UseAuthentication();
    app.UseAuthorization();
    
    app.MapSalaryEndpoints();

    await app.RunAsync();
}