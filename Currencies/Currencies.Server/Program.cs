using Currencies.Core.Interface;
using Currencies.DataAccess;
using Currencies.DataRepositories;
using Currencies.DataServices;
using Currencies.Server.HostedService;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(c =>
{
    c.UseSqlServer(builder.Configuration.GetConnectionString("main"), b => b.MigrationsAssembly(typeof(Program).Assembly.FullName));
});
builder.Services.AddTransient<ICurrencyRepository, CurrencyRepository>();
builder.Services.AddTransient<IExchangeRateRepository, ExchangeRateRepository>();
builder.Services.AddHttpClient<IExchangesDataService, NBPDataService>(client =>
{
    client.BaseAddress = new Uri("http://api.nbp.pl/api/exchangerates/");
});
builder.Services.AddHostedService<UpdateCurrenciesService>();
var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
}

    app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
