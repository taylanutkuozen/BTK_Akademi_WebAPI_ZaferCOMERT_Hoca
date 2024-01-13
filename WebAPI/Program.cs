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
/*LogManager Nlog'tan gelmektedir.Konfigürasyon ifadesi ve path alýnmýþ oldu.*/
builder.Services.AddControllers(config =>
        {
            config.RespectBrowserAcceptHeader = true;
            /*bu ifade bir flag ve bu flag içerik pazarlýðý için açýðýz*/
            config.ReturnHttpNotAcceptable = true;
            /*Bir request'i kabul edip etmediðimizi client ile paylaþmak*/
        })
    .AddCustomCsvFormatter()
    .AddXmlDataContractSerializerFormatters() /*Xml formatýnda çýktý verebilecektir*/
    .AddApplicationPart(typeof(Presentation.AssemblyReference).Assembly)
    /*PresentationLayer tarafýndan entegre edildi. bu kod ile Controller yapýsýnýn bu projede çözülebilmesine olanak saðlanmýþ olundu*/
    .AddNewtonsoftJson();
builder.Services.AddScoped<ValidationFilterAttribute>(); //IoC tarafýndan projede saðlanacaktýr.
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
    /*Bir filteri tanýmlayan deðeri tanýmlar ve ModelState invalid olduðunda BadRequest*/
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();/*Controller yapýlarý bu komut ile keþfedilir.*/
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