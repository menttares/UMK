using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Physical;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using УМК.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => {
        options.LoginPath = "/LogIn/AuthAdmin";
    });
builder.Services.AddAuthorization();

// получаем строку подключения из файла конфигурации
string connection = builder.Configuration.GetConnectionString("DefaultConnection");

// добавляем контекст ApplicationContext в качестве сервиса в приложение
builder.Services.AddDbContext<Database>(options => options.UseSqlServer(connection));

// Configure the HTTP request pipeline.
if (builder.Environment.IsDevelopment())
{
    //app.UseBrowserLink();
    builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation(options =>
    {
        options.FileProviders.Add(new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Views")));
    });
}

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseHsts();
app.UseRouting();
app.UseExceptionHandler("/Home/Error");

app.UseAuthentication();   // добавление middleware аутентификации 
app.UseAuthorization();   // добавление middleware авторизации 

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Main}/{action=Index}/{id?}");
app.Run();
