using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NLog;
using Presentation.ActionFilters;
using Repositories.EFCore;
using Services.Contracts;
using WebAPI.Extensions;
var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
LogManager.LoadConfiguration(String.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));
/*LogManager Nlog'tan gelmektedir.Konfig�rasyon ifadesi ve path al�nm�� oldu.*/
builder.Services.AddControllers(config =>
        {
            config.RespectBrowserAcceptHeader = true;
            /*bu ifade bir flag ve bu flag i�erik pazarl��� i�in a����z*/
            config.ReturnHttpNotAcceptable = true;
            /*Bir request'i kabul edip etmedi�imizi client ile payla�mak*/
        })
    .AddCustomCsvFormatter()
    .AddXmlDataContractSerializerFormatters() /*Xml format�nda ��kt� verebilecektir*/
    .AddApplicationPart(typeof(Presentation.AssemblyReference).Assembly)
    /*PresentationLayer taraf�ndan entegre edildi. bu kod ile Controller yap�s�n�n bu projede ��z�lebilmesine olanak sa�lanm�� olundu*/
    .AddNewtonsoftJson();
builder.Services.AddScoped<ValidationFilterAttribute>(); //IoC taraf�ndan projede sa�lanacakt�r.
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
    /*Bir filteri tan�mlayan de�eri tan�mlar ve ModelState invalid oldu�unda BadRequest*/
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();/*Controller yap�lar� bu komut ile ke�fedilir.*/
builder.Services.AddSwaggerGen();
builder.Services.ConfigureSqlContext(builder.Configuration);
builder.Services.ConfigureRepositoryManager();/*RepositoryManager generate edilecek*/
builder.Services.ConfigureServicesManager();
builder.Services.ConfigureLoggerService();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.ConfigureActionFilters();
var app = builder.Build();
var logger=app.Services.GetRequiredService<ILoggerService>();
app.ConfigureExceptionHandler(logger);
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
if(app.Environment.IsProduction())
{
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();