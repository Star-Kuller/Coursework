using Coursework.Database;
using Coursework.Database.Infrastructure;
using Coursework.Interfaces.Database;

var builder = WebApplication.CreateBuilder(args);

Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<DbConnectionFactory>();
builder.Services.AddTransient<IUnitOfWorkFactory, UnitOfWorkFactory>();
var connectionString = builder.Configuration.GetConnectionString("MainDbConnection");

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();
MigrationRunner.RunMigrations(connectionString, logger);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseBrowserLink();
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();