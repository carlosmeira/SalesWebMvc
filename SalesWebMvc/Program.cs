using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using SalesWebMvc.Data;

var builder = WebApplication.CreateBuilder(args);
//builder.Services.AddDbContext<SalesWebMvcContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("SalesWebMvcContext") ?? throw new InvalidOperationException("Connection string 'SalesWebMvcContext' not found.")));
builder.Services.AddDbContext<SalesWebMvcContext>(options => options.UseMySql(builder.Configuration.GetConnectionString("SalesWebMvcContext"), ServerVersion.Create(8, 0, 0, ServerType.MySql), builder => builder.MigrationsAssembly("SalesWebMvc")));
builder.Services.AddScoped<SeedingService>();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    
} else
{
    using (var scope = app.Services.CreateScope())
    {
        var scoopedContext = scope.ServiceProvider.GetRequiredService<SeedingService>();
        scoopedContext.Seed();
    }
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
