using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Application;
using IkenBako.ViewModels;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace IkenBako.Pages
{
  public class LoginModel : PageModel
  {
    public const string KEY_LOGIN_ID = "KEY_LOGIN_ID";

    private readonly ILogger<LoginModel> _logger;

    /// <summary>
    /// ユーザーサービス
    /// </summary>
    private readonly UserService userService;

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
    /// <param name="receiverService">送信者サービス</param>
    public LoginModel(ILogger<LoginModel> logger, UserService userService)
    {
      _logger = logger;

      this.userService = userService;
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
      if(!EqalsPassword(Target, Password))
      {
        ViewData["ErrorMessages"] = "IDまたはパスワードが間違っています。";
        Target = string.Empty;
        Password = string.Empty;
        return RedirectToPage("/Login");
      }
      HttpContext.Session.SetString(KEY_LOGIN_ID, Target);

      return RedirectToPage("/index");
    }

    /// <summary>
    /// ユーザーログインチェック
    /// </summary>
    /// <param name="unique_name">ID</param>
    /// <param name="password">パスワード</param>
    /// <returns></returns>
    private bool EqalsPassword(string unique_name, string password)
    {
      var target = userService.GetUser(unique_name);

      var salt = Convert.FromBase64String(target.Salt);

      string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
        password: password,
        salt: salt,
        prf: KeyDerivationPrf.HMACSHA1,
        iterationCount: 10000,
        numBytesRequested: 256 / 8));

      return hashed == target.Password;
    }
  }
}
