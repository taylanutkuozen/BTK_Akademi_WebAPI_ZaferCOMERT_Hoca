using Microsoft.EntityFrameworkCore;
using NLog;
using Repositories.EFCore;
using WebAPI.Extensions;
var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
LogManager.LoadConfiguration(String.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));
/*LogManager Nlog'tan gelmektedir.Konfigürasyon ifadesi ve path alýnmýþ oldu.*/
builder.Services.AddControllers()
    .AddApplicationPart(typeof(Presentation.AssemblyReference).Assembly)
    /*PresentationLayer tarafýndan entegre edildi. bu kod ile Controller yapýsýnýn bu projede çözülebilmesine olanak saðlanmýþ olundu*/
    .AddNewtonsoftJson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();/*Controller yapýlarý bu komut ile keþfedilir.*/
builder.Services.AddSwaggerGen();
builder.Services.ConfigureSqlContext(builder.Configuration);
builder.Services.ConfigureRepositoryManager();/*RepositoryManager generate edilecek*/
builder.Services.ConfigureServicesManager();
builder.Services.ConfigureLoggerService();
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