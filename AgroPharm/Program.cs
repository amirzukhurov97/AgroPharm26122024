using AgroPharm.Infrastructure;
using AgroPharm.Interfaces;
using AgroPharm.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton(pro=>new MarketRepository(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddSingleton (pro=>new BuyProductRepo(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddSingleton (pro=>new SellProductRepo(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddSingleton (pro=>new ReturnOutRepo(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddSingleton (pro=>new ReturnInRepo(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddSingleton(propa=>new ProductRepo(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddSingleton(propa=>new CustomerRepo(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddSingleton(propa=>new OrganizationRepo(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddSingleton(propa=>new CurrencyRepo(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddSingleton<AppDbContext>();
builder.Services.AddHttpClient();
builder.Services.AddAntiforgery(options => {
    options.HeaderName = "X-CSRF-TOKEN";
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
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
