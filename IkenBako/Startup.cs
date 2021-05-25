using Domain.Application;
using Domain.Domain.OpinionMessages;
using Domain.Domain.Receivers;
using Infrastructures;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IkenBako
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddRazorPages();

      // DI設定
      // リポジトリ
      services.AddSingleton<IMessageRepository, MessageRepository>();
      services.AddSingleton<IReceiverRepository, ReceiverRepository>();

      // Configを専用Modelに設定
      services.Configure<DatabaseConfigModel>(Configuration.GetSection("DB"));

      // ApplicationService
      services.AddTransient<MessageService>();
      services.AddTransient<ReceiverService>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseExceptionHandler("/Error");
      }

      app.UseStaticFiles();

      app.UseRouting();

      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapRazorPages();
      });
    }
  }
}
