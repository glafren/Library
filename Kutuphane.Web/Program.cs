/*
1-)Data katman�na ApplicationDbContext.cs diye bir class a�al�m
2-)Ve onu DbContext 'ten kal�tarak, olmas� gereken constructor' yazal�m
3-) Models katman�m�zdaki modelleri birer DbSet olarak tan�mlayal�m.
bunlar� yaparken, data katman�na efcore kurman�z gerekebilir.
4-)Web katman�nda, bui1der.Services .AddDbCOntext komutuyla dependency injection kullanarak
context'imizi Katman�m�za tan�tal�m.
5-) appsettings.json dosyas�na connection string tan�mlamas� yapal�m, database ad�na "Library" diyelim.
*/

using Kutuphane.Data;
using Kutuphane.Repository.Abstract;
using Kutuphane.Repository.Concrete;
using Kutuphane.Repository.Shared.Abstract;
using Kutuphane.Repository.Shared.Concrete;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//Program�m�z�n login i�lemi gerektirdi�ine dair bir authentication tan�ml�yoruz.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options=> 
{
    options.LoginPath = "/User/Login";
    options.AccessDeniedPath = "/User/UnAuthorized";
});

builder.Services.AddControllersWithViews().AddNewtonsoftJson(options=>options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore); //�oka �ok ili�iki olan tablolarda loopa izin verilmesi i�in newtonsoft.json a ihtiyac�m�z vard�r. (microsoft newtonsoft.json)
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Library")));
//builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
//builder.Services.AddScoped<IPublisherRepository, PublisherRepository>();
//builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=User}/{action=Login}/{id?}");

app.Run();
