using Domain.Application;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace IkenBako.Pages
{
  public class ChangePasswordModel : PageModel
  {
    private readonly ILogger<ChangePasswordModel> _logger;

    /// <summary>
    /// ユーザーサービス
    /// </summary>
    private readonly UserService userService;

    /// <summary>
    /// 旧パスワード
    /// </summary>
    [BindProperty]
    public string OldPassword { set; get; }

    /// <summary>
    /// 新パスワード
    /// </summary>
    [BindProperty]
    public string NewPassword { set; get; }

    /// コンストラクタ
    /// </summary>
    /// <param name="logger">ログインスタンス</param>
    /// <param name="userService">ユーザーサービス</param>
    public ChangePasswordModel(ILogger<ChangePasswordModel> logger, UserService userService)
    {
      _logger = logger;

      this.userService = userService;
    }

    public void OnGet()
    {
      if (!HttpContext.Session.Keys.Contains(LoginModel.KEY_LOGIN_ID))
      {
        Response.Redirect("/index");
        return;
      }
    }

    public IActionResult OnPost()
    {
      if (!HttpContext.Session.Keys.Contains(LoginModel.KEY_LOGIN_ID))
      {
        return RedirectToPage("/index");
      }

      // ユーザーID取得
      var id = HttpContext.Session.GetString(LoginModel.KEY_LOGIN_ID);

      // 入力チェック
      var errorMessages = new List<string>();
      if (string.IsNullOrEmpty(OldPassword))
      {
        errorMessages.Add("旧Passwordを入力してください。");
      }
      if (string.IsNullOrEmpty(NewPassword))
      {
        errorMessages.Add("新Passwordを入力してください。");
      }
      if (OldPassword == NewPassword)
      {
        errorMessages.Add("旧Passwordと新Passwordが同じです。");
      }

      // 旧パスワードの確認
      if (!errorMessages.Any())
      {
        if (!EqalsPassword(id, OldPassword))
        {
          errorMessages.Add("旧パスワードが間違っています。");
        }
      }

      // パスワード変更
      if (!errorMessages.Any())
      {
        if (!ChangePassword(id, NewPassword))
        {
          errorMessages.Add("パスワード変更が失敗しました。");
        }
      }

      // エラーメッセージがある場合は現在のページのまま
      if (errorMessages.Any())
      {
        ViewData["ErrorMessages"] = errorMessages;
        OldPassword = string.Empty;
        NewPassword = string.Empty;
        return Page();
      }

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

    /// <summary>
    /// パスワード変更
    /// </summary>
    /// <param name="unique_name">ユーザーID</param>
    /// <param name="password">新パスワード</param>
    /// <returns>更新可否</returns>
    private bool ChangePassword(string unique_name, string password)
    {
      // 128ビットのソルトを生成する
      byte[] salt = new byte[128 / 8];
      using (var rng = RandomNumberGenerator.Create())
      {
        rng.GetBytes(salt);
      }
      var saltBase64 = Convert.ToBase64String(salt);

      // 256ビットのサブキーを導出
      string encryptedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
          password: password,
          salt: salt,
          prf: KeyDerivationPrf.HMACSHA1,
          iterationCount: 10000,
          numBytesRequested: 256 / 8));

      // DB更新
      return userService.Save(Domain.Domain.Users.User.Create(unique_name, encryptedPassword, saltBase64));
    }
  }
}