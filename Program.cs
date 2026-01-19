using Microsoft.EntityFrameworkCore;
using CustomerApi.Data;
using CustomerApi.Repositories;
using CustomerApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// In-memory Sqlite database for testing with migrations support
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=customer.db"));

// DI
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICustomerService, CustomerService>();

var app = builder.Build();

// Ensure in-memory SQLite is opened and migrations applied
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
app.UseAuthorization();
app.MapControllers();

app.Run();
