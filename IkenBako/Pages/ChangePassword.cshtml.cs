using Domain.Application;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

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
    [DataType(DataType.Password)]
    [BindProperty]
    public string OldPassword { set; get; }

    /// <summary>
    /// 新パスワード
    /// </summary>
    [DataType(DataType.Password)]
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

      // ユーザーの更新バージョンを取得する
      var dbUser = userService.GetUser(HttpContext.Session.GetString(LoginModel.KEY_LOGIN_ID));
      if(dbUser is null){
        // ユーザーが存在しない場合はindexに戻る
        Response.Redirect("/index");
        return;
      }
      // 更新バージョンをセッションに格納
      HttpContext.Session.SetInt32(LoginModel.KEY_LOGIN_VERSION, dbUser.Version);
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
        if (!userService.EqalsPassword(id, OldPassword))
        {
          errorMessages.Add("旧パスワードが間違っています。");
        }
      }

      // ページ表示時の更新バージョンの取得
      var modifiedMessage = "すでに更新されています。ページを開きなおしてください。";
      if (!HttpContext.Session.Keys.Contains(LoginModel.KEY_LOGIN_VERSION))
      {
          errorMessages.Add(modifiedMessage);
      }
      var version = HttpContext.Session.GetInt32(LoginModel.KEY_LOGIN_VERSION);
      if(!version.HasValue)
      {
          errorMessages.Add(modifiedMessage);
      }

      // パスワード変更
      if (!errorMessages.Any() && version.HasValue)
      {
        if (!userService.ChangePassword(id, NewPassword, version.Value))
        {
          errorMessages.Add(modifiedMessage);
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
  }
}
