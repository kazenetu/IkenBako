using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace IkenBako.Pages
{
  public class SendSuccessModel : PageModel
  {
    private readonly ILogger<SendSuccessModel> _logger;

    /// <summary>
    /// 設定情報
    /// </summary>
    private SettingConfigModel config;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="logger">ログインスタンス</param>
    /// <param name="config">設定情報</param>
    public SendSuccessModel(ILogger<SendSuccessModel> logger, IOptions<SettingConfigModel> config)
    {
      _logger = logger;
      this.config = config.Value;
    }

    public void OnGet()
    {
      if (config.AllLogin && !HttpContext.Session.Keys.Contains(LoginModel.KEY_LOGIN_ID))
      {
        Response.Redirect("/Login");
        return;
      }
    }
  }
}