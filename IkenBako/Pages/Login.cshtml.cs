using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Application;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace IkenBako.Pages
{
  public class LoginModel : PageModel
  {
    public const string KEY_LOGIN_ID = "KEY_LOGIN_ID";
    public const string KEY_LOGIN_VERSION = "KEY_LOGIN_VERSION";

    public const string KEY_RECEIVER = "KEY_RECEIVER";

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

      HttpContext.Session.SetString(KEY_LOGIN_ID, Target);

      // 一覧表示権限のチェック
      var receiver = receiverService.GetReceiver(Target);
      if(receiver != null){
        HttpContext.Session.SetString(KEY_RECEIVER, receiver.ID);
      }

      return RedirectToPage("/index");
    }
  }
}
