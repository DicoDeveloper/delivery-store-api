using Application.ViaCep.Services;
using Infrastructure.Data;
using Infrastructure.Services;
using Infrastructure.Setup.Configs;
using Infrastructure.Setup.Installers;
using Microsoft.OpenApi.Models;
using Web.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.AddLogging();

builder.Services.InstallRepositories()
                .AddDbContext(builder.Configuration["ConnectionStrings:DefaultConnection"] ?? "")
                .InstallMediatr();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Minimal API", Version = "v1" })
);
builder.Services.AddHttpClient<IViaCepApiService, ViaCepApiService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ViaCEP:BaseUrl"]!);
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
        builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
});

var app = builder.Build();

app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Minimal API v1");
    c.DisplayRequestDuration();
});
await app.InitialiseDatabaseAsync();

app.UseRouting();

app.MapProductsEndpoint();
app.MapSalesEndpoint();

app.Run();

public partial class Program
{ }