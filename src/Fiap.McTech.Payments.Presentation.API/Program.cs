using Fiap.McTech.Payments.CrossCutting.IoC;
using Fiap.McTech.Payments.CrossCutting.IoC.Infra.Context;
using Fiap.McTech.Payments.CrossCutting.IoC.Mappers;
using Fiap.McTech.Payments.Presentation.API.Configurations;
using Fiap.McTech.Payments.Presentation.API.Handlers;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddCommandLine(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddJwtBearerAuthentication();
builder.Services.AddSwagger();

builder.Services.Configure<Fiap.McTech.Payments.ExternalService.WebAPI.MercadoPago.MercadoPagoConfig>(
    builder.Configuration.GetSection("MercadoPagoConfig")
);

// AutoMapper configuration
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.RegisterMappings();
builder.Services.RegisterServices(builder.Configuration);

// Cors configuration
builder.Services.AddCors(options =>
{
    var allowOrigins = builder.Configuration.GetValue<string>("ALLOW_ORIGINS") ?? "*";
    options.AddPolicy("CorsConfig", builder => builder.WithOrigins(allowOrigins.Split(';')).AllowAnyMethod().AllowAnyHeader());
});


var app = builder.Build();

using var scope = app.Services.CreateScope();
scope.McTechDatabaseInitialize();

// Configure the HTTP request pipeline.
/* if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
} */

//app.UseHttpsRedirection();
//app.UseAuthorization();
//app.MapControllers();
//app.Run();

app.UseSwaggerV3();
app.UseCors("CorsConfig");
app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseAuth();

app.MapControllers();

await app.RunAsync();
