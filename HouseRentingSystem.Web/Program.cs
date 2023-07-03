namespace HouseRentingSystem.Web
{
    
   
    using Microsoft.EntityFrameworkCore;
    using Data;
    using HouseRentingSystem.Data.Models;

    using HouseRentingSystem.Web.Infrastructure.Extensions;
    using HouseRenting.Services.Data;
    using HouseRenting.Services.Data.Interfaces;

    public class Program
    {
        public static async Task Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            
            string connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            builder.Services.AddDbContext<HouseRentingDbContext>(options =>
                options.UseSqlServer(connectionString));


            builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = builder.Configuration.GetValue<bool>("Identity:SignIn:RequireConfirmedAccount");
                options.Password.RequireLowercase = builder.Configuration.GetValue<bool>("Identity:Password:RequireLowercase"); ;
                options.Password.RequireUppercase = builder.Configuration.GetValue<bool>("Identity:Password:RequireUppercase"); ;
                options.Password.RequireNonAlphanumeric = builder.Configuration.GetValue<bool>("Identity:Password:RequireNonAlphanumeric"); ;
                options.Password.RequiredLength = builder.Configuration.GetValue<int>("Identity:Password:RequiredLength"); ;
            } )
                .AddEntityFrameworkStores<HouseRentingDbContext>();

            builder.Services.AddApplicationServices(typeof(IHouseService));

            builder.Services.AddControllersWithViews();

         

            WebApplication app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
               
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapDefaultControllerRoute();
            app.MapRazorPages();
                
           await app.RunAsync();
        }
    }
}