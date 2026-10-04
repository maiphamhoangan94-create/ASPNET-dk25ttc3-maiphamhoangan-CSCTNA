using ASPNET_dk25ttc3_maiphamhoangan_CSCTNA.Models;
using ASPNET_dk25ttc3_maiphamhoangan_CSCTNA.Services;
using Microsoft.EntityFrameworkCore;

var trinhTaoWeb = WebApplication.CreateBuilder(args);

var chuoiKetNoi = trinhTaoWeb.Configuration.GetConnectionString("DefaultConnection");

trinhTaoWeb.Services.AddDbContext<AnmphDbContext>(cauHinh =>
    cauHinh.UseSqlServer(chuoiKetNoi));

// Add services to the container.
trinhTaoWeb.Services.AddControllersWithViews();
trinhTaoWeb.Services.AddScoped<AdoNetReportService>();

trinhTaoWeb.Services.AddSession(cauHinh =>
{
    cauHinh.IdleTimeout = TimeSpan.FromMinutes(30);
    cauHinh.Cookie.HttpOnly = true;
    cauHinh.Cookie.IsEssential = true;
});

var ungDung = trinhTaoWeb.Build();

// Configure the HTTP request pipeline.
if (!ungDung.Environment.IsDevelopment())
{
    ungDung.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    ungDung.UseHsts();
}

ungDung.UseHttpsRedirection();
ungDung.UseRouting();

ungDung.UseSession();

ungDung.UseAuthorization();

ungDung.MapStaticAssets();

ungDung.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

ungDung.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

ungDung.MapControllers();

ungDung.Run();
