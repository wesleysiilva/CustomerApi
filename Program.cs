using Microsoft.EntityFrameworkCore;
using CustomerApi.Data;
using CustomerApi.Repositories;
using CustomerApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Sqlite database for testing
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=customer.db"));

// DI
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICustomerService, CustomerService>();

var app = builder.Build();

// Ensure SQLite migrations applied
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "CustomerApi v1"); c.RoutePrefix = string.Empty; });

app.UseAuthorization();
app.MapControllers();

app.Run();
