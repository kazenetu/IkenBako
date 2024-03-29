using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Domain.Application;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Antiforgery;
using System.Text;

namespace IkenBako.Pages
{
  public class LoginModel : PageModel
  {
    public const string KEY_LOGIN_ID = "KEY_LOGIN_ID";
    public const string KEY_LOGIN_VERSION = "KEY_LOGIN_VERSION";

    public const string KEY_RECEIVER = "KEY_RECEIVER";
    public const string KEY_ADMIN = "KEY_ADMIN";

    private readonly ILogger<LoginModel> _logger;

    /// <summary>
    /// ユーザーサービス
    /// </summary>
    private readonly UserService userService;

    /// <summary>
    /// 送信者サービス
    /// </summary>
    private readonly ReceiverService receiverService;

    /// <summary>
    /// 確認者
    /// </summary>
    [BindProperty]
    public string Target { set; get; }

    /// <summary>
    /// パスワード
    /// </summary>
    [BindProperty]
    [DataType(DataType.Password)]
    public string Password { set; get; }

    /// コンストラクタ
    /// </summary>
    /// <param name="logger">ログインスタンス</param>
    /// <param name="userService">ユーザーサービス</param>
    /// <param name="receiverService">送信者サービス</param>
    public LoginModel(ILogger<LoginModel> logger, UserService userService,ReceiverService receiverService)
    {
      _logger = logger;

      this.userService = userService;
      this.receiverService = receiverService;
    }

    public void OnGet()
    {
      if (HttpContext.Session.Keys.Contains(LoginModel.KEY_LOGIN_ID))
       {
         Response.Redirect("/index");
         return;
       }
    }

    public IActionResult OnPost()
    {
      HttpContext.Session.Clear();

      var errorMessages = new List<string>();
      if(string.IsNullOrEmpty(Target))
      {
        errorMessages.Add("IDを入力してください。");
      }
      if (string.IsNullOrEmpty(Password))
      {
        errorMessages.Add("パスワードを入力してください。");
      }

      if (!errorMessages.Any())
      {
        if (!userService.EqalsPassword(Target, Password))
        {
          errorMessages.Add("IDまたはパスワードが間違っています。");
        }
      }

      if (errorMessages.Any())
      {
        ViewData["ErrorMessages"] = errorMessages;
        Target = string.Empty;
        Password = string.Empty;
        return Page();
      }

      // Cookie削除(のちほど自動的に再取得)
      foreach (var key in Request.Cookies.Keys)
      {
        // CSRF対策用Cookieは削除しない
        if (key.StartsWith(".AspNetCore.Antiforgery."))
        {
          continue;
        }
        Response.Cookies.Delete(key);
      }
 
      // CSRF対策用トークンの取得
      var antiforgery = (IAntiforgery)HttpContext.RequestServices.GetService(typeof(IAntiforgery));
      var tokenSet = antiforgery.GetAndStoreTokens(HttpContext);
 
      // 一時表示用HTML作成(OnPostRedirectIndexを呼ぶHTML)
      var contentResult = new StringBuilder();
      contentResult.AppendLine($"<html><body><div style=\"display:none;\">");
      contentResult.AppendLine($"<form method=\"POST\" action=\"/Login/RedirectIndex\">");
      contentResult.AppendLine($"<input type=\"hidden\" name=\"loginID\" value=\"{Target}\">");
      contentResult.AppendLine($"<input type=\"hidden\" name=\"{tokenSet.FormFieldName}\" value=\"{tokenSet.RequestToken}\">");
      contentResult.AppendLine($"<input type=\"submit\" id=\"submit\">");
      contentResult.AppendLine($"</form></div>");
      contentResult.AppendLine($"<script>");
      contentResult.AppendLine("window.onload = function() {");
      contentResult.AppendLine("document.getElementById('submit').click();");
      contentResult.AppendLine("}");
      contentResult.AppendLine($"</script>");
      contentResult.AppendLine($"</body></html>");
 
      // HTMLとして出力
      return Content(contentResult.ToString(), "text/html; charset=utf-8");
    }

    /// <summary>
    /// ログインIDをセッションに設定とindexにリダイレクト
    /// </summary>
    /// <param name="loginID">ログインID(form内要素から取得)</param>
    /// <returns>遷移先</returns>
    public IActionResult OnPostRedirectIndex(string loginID)
    {
      if (!string.IsNullOrEmpty(loginID))
      {
        HttpContext.Session.SetString(KEY_LOGIN_ID, loginID);

        // 一覧表示権限・管理者権限のチェック
        var receiver = receiverService.GetReceiver(loginID);
        if(receiver != null){
          // 一覧確認権限がある場合のみ設定
          if(receiver.IsViewListRole){
            HttpContext.Session.SetString(KEY_RECEIVER, receiver.ID);
          }

          // 管理者権限がある場合のみ設定
          if(receiver.IsAdminRole)
            HttpContext.Session.SetString(KEY_ADMIN, receiver.IsAdminRole.ToString());
        }

        return RedirectToPage("/index");
      }
 
      // IDがない場合はLoginページに戻す
      return Redirect("/Login");
    }
  }
}
