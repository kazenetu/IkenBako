using Domain.Application;
using Domain.Domain.OpinionMessages;
using Domain.Domain.Receivers;
using Domain.Domain.Users;
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

      services.AddSession();

      // DI設定
      // リポジトリ
      services.AddSingleton<IMessageRepository, MessageRepository>();
      services.AddSingleton<IReceiverRepository, ReceiverRepository>();
      services.AddSingleton<IUserRepository, UserRepository>();

      // Configを専用Modelに設定
      var dbRoot = Configuration.GetSection("DB");

#if DEBUG
      var dbConnectionStrings = dbRoot.GetSection("ConnectionStrings");
      var dbTarget = dbRoot.GetSection("Target");
      if (dbConnectionStrings.Value is null || dbTarget.Value is null)
      {
        // デバッグ時のみ未設定の場合はSQLiteを選択
        dbConnectionStrings.GetSection("sqlite").Value = "Resource/Test.db";
        dbTarget.Value = "sqlite";
      }
#endif

      services.Configure<DatabaseConfigModel>(dbRoot);
      services.Configure<SettingConfigModel>(Configuration.GetSection("Setting"));

      // ApplicationService
      services.AddTransient<MessageService>();
      services.AddTransient<ReceiverService>();
      services.AddTransient<UserService>();
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

      app.UseSession();

      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapRazorPages();
      });
    }
  }
}
