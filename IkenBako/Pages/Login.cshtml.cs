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

    private const string KEY_ERROR_ID_OR_PASS = "KEY_ERROR_ID_OR_PASS";

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

    /// <summary>
    /// エラーメッセージ
    /// </summary>
    public List<string> ErrorMessages {get;private set;} = null;

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

      if (HttpContext.Session.Keys.Contains(KEY_ERROR_ID_OR_PASS))
       {
          ErrorMessages = new List<string>();
          ErrorMessages.Add(HttpContext.Session.GetString(KEY_ERROR_ID_OR_PASS));
          HttpContext.Session.Remove(KEY_ERROR_ID_OR_PASS);
       }
    }

    public IActionResult OnPost()
    {
      var errorMessage = string.Empty;
      if(string.IsNullOrEmpty(errorMessage) && string.IsNullOrEmpty(Target))
      {
        errorMessage = "IDが入力されていません。";
      }
      if(string.IsNullOrEmpty(errorMessage) && string.IsNullOrEmpty(Password))
      {
        errorMessage = "パスワードが入力されていません。";
      }

      if(string.IsNullOrEmpty(errorMessage) && !EqalsPassword(Target, Password))
      {
        errorMessage = "IDまたはパスワードが間違っています。";
        HttpContext.Session.SetString(KEY_ERROR_ID_OR_PASS,errorMessage);
        return RedirectToPage("/Login");
      }

      if(string.IsNullOrEmpty(errorMessage))
      {
        HttpContext.Session.SetString(KEY_LOGIN_ID, Target);
        return RedirectToPage("/index");
      }

      ErrorMessages = new List<string>();
      ErrorMessages.Add(errorMessage);

      return Page();
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
      if(target is null){
        return false;
      }

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
