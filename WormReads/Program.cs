using Microsoft.EntityFrameworkCore;
using WormReads.Data;
using WormReads.DataAccess.Repository.Category_Repository;
using WormReads.DataAccess.Repository.Product_Repository;
using WormReads.DataAccess.Repository.Unit_Of_Work;
using Microsoft.AspNetCore.Identity;
using WormReads.Application;
using Microsoft.AspNetCore.Identity.UI.Services;
using WormReads.DataAccess.Repository.Company_Repository;
using WormReads.DataAccess.Repository.Shopping_Cart_Repository;
using WormReads.DataAccess.Repository.User_Rpository;
using WormReads.DataAccess.Repository.Order_Header_Repository;
using WormReads.Models;
using WormReads.DataAccess.Repository.Order_Details_Repository;
using Stripe;

namespace WormReads
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<AppDbContext>
                (o => o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddIdentity<IdentityUser, IdentityRole>().
                AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();//because AddIdentity doesnt add token providers such as for email confirmation

            builder.Services.ConfigureApplicationCookie(options => //configure login path because we are using areas
            {
                options.LoginPath = $"/Identity/Account/Login";
                options.LogoutPath = $"/Identity/Account/Logout";
                options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
            });

            builder.Services.AddRazorPages();//for identity to work

            builder.Services.AddScoped<ICategoryRepository,CategoryRepository>();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IShoppingCartRepository, ShoppingCartRepository>();
            builder.Services.AddScoped<IUserRpository, UserRepository>();
            builder.Services.AddScoped<IOrderHeaderRepository, OrderHeaderRepository>();
            builder.Services.AddScoped<IOrderDetailsRepository,OrderDetailsRepository>();
            builder.Services.AddScoped<IEmailSender, EmailSender>(); //registered fake email sender to override default one which throws exception


            builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe")); //mapping stripe section into stripe class

            //configurations for consuming sessions
            builder.Services.AddDistributedMemoryCache(); //This is required before you can use Session.
            builder.Services.AddSession(o =>
            {
                o.IdleTimeout = TimeSpan.FromMinutes(100);
                o.Cookie.IsEssential = true; //This session cookie is essential for the site to function.
                o.Cookie.HttpOnly = true; //Good security practice 
            });

            builder.Services.AddAuthentication().AddFacebook(options =>
            {
                options.AppId = "1416005980096492";
                options.AppSecret = "1986907b6020ab26470c300555186680";
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession(); //to use sessions
            app.MapStaticAssets();

            StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();//configuring stripe 

            app.MapRazorPages(); //for identity to work

            app.MapControllerRoute(
                name: "default",
                pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
