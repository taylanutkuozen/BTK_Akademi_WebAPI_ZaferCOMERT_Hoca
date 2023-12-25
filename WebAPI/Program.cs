using Microsoft.EntityFrameworkCore;
using WebAPI.Repositories;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<RepositoryContext>(options=>
options.UseSqlServer(builder.Configuration.GetConnectionString("sqlConnection")));//Generic olarak DbContext inherit olan context class eklememiz gerekiyor. IoC DbContext tanýmý yapmýþ olundu=Bir DbContext ihtiyacýmýz olduðunda bir injection yaptýðýmýz zaman somut haline ulaþmak ve db ye baðlanýrken hata olmayacaktýr. *Register-Container*
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
