using Eshopping.Repository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
//connet database
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration["ConnectionStrings:ConnectedDb"]);
}
);
// Add services to the container.
builder.Services.AddControllersWithViews();
//dki session:
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(option =>
{
    option.IdleTimeout = TimeSpan.FromMinutes(30);
    option.Cookie.IsEssential = true;
}
);

var app = builder.Build();

//đường dẫn đễn trang 404 notfound : khi lỗi thường chrome sẽ hiển thị 1 trang mặc định, ta muốn khi lỗi nó sẽ đi đến trang báo lỗi do tra custom 
app.UseStatusCodePagesWithRedirects("/Home/Error?statuscode={0}");

app.UseSession();
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

app.UseAuthorization();
//Đki map này cho backend: ten map là areas, mặc định "controller=Product"
//ta sẽ để trang backend lên trên: do mặc định nó chạy frontend đầu tiên , mà ta muốn backend chạy trc 
app.MapControllerRoute(
    name: "Areas",
    pattern: "{area:exists}/{controller=Product}/{action=Index}/{id?}");
//ta sẽ đki các map cho category,brand :khi click vào đó nó sẽ chuyển từ trang index mặc định sang các trang này 
app.MapControllerRoute(
    name: "category",
    pattern: "/category/{Slug?}",
    defaults: new {controller="Category",action="Index"});
app.MapControllerRoute(
    name: "brand",
    pattern: "/brand/{Slug?}",
    defaults: new {controller="Brand",action="Index"});

//map này của FE: những map route mới ta phải viết trên route mặc định này 
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

//seeding data:tao du lieu trong sql server
var context=app.Services.CreateScope().ServiceProvider.GetRequiredService<DataContext>();
SeedData.SeedingData(context);
app.Run();
