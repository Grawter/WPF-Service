using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Web_Service.Models;

namespace Web_Service
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ParkingdbMainContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                endpoints.MapGet("/", async context =>
                {
                    System.DateTime current_date = System.DateTime.Now;

                    if (current_date.DayOfWeek == System.DayOfWeek.Monday && current_date.Hour == 10)
                    {
                        context.Response.Redirect("http://localhost:7285/api/Data/Update");
                    }
                    else
                    {
                        await context.Response.WriteAsync(@"<script>
                                                            onload = function () {setTimeout ('location.reload (true)', 60*1000*60)}
                                                            </script>");

                        await context.Response.WriteAsync(
                        "<b>Show current data:</b><br> <a href='http://localhost:7285/api/Data/Show'>Click<a>" + "<br>" +
                        "<b>Re-create data(<u>Warning: Costly operation</u>):</b><br> <a href='http://localhost:7285/api/Data/Create'>Click</a>" + "<br>" +
                        "<b>Update data:</b><br> <a href='http://localhost:7285/api/Data/Update'>Click</a>" + "<br>" +
                        "<b>Export data to Json:</b><br> <a href='http://localhost:7285/api/Data/Export'>Click</a>" + "<br>");

                        await context.Response.WriteAsync("<br><b>Today: </b>" + current_date.DayOfWeek.ToString() + "<br>" + current_date.ToShortDateString() + "<br>");

                        await context.Response.WriteAsync("<br><span style='font-size: 1.4em;'>The database is updated <b>automatically</b> every Monday at 10:00</span>");
                    }
                });
            });
        }
    }
}