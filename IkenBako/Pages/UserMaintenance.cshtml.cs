using Domain.Application;
using IkenBako.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace IkenBako.Pages
{
  public class UserMaintenanceModel : PageModel
  {
    public const string KEY_USER_LIST = "KEY_USER_LIST";

    private readonly ILogger<UserMaintenanceModel> _logger;

    /// <summary>
    /// ユーザーサービス
    /// </summary>
    private readonly UserService userService;

    /// <summary>
    /// 送信者サービス
    /// </summary>
    private readonly ReceiverService receiverService;

    /// <summary>
    /// ユーザーリスト
    /// </summary>
    public List<UserViewModel> Users { get; private set; } = new List<UserViewModel>();

    /// <summary>
    /// 編集ユーザー情報
    /// </summary>
    [BindProperty]
    public UserViewModel EditTarget { set; get; } = new UserViewModel();

    /// <summary>
    /// 編集か否か
    /// </summary>
    [BindProperty]
    public bool IsEdit { set; get; } = false;

    /// <summary>
    /// 編集中のユーザーマスタのバージョン
    /// </summary>
    [BindProperty]
    public int EditTargetUserVersion { set; get; }

    /// <summary>
    /// 編集中の受信者マスタのバージョン
    /// </summary>
    [BindProperty]
    public int EditTargetReceiverVersion { set; get; }

    /// <summary>
    /// 変更/新規ボタンのボタン名
    /// </summary>
    public string SaveButtonText { get { return IsEdit ? "変更" : "新規作成"; } }

    /// <summary>
    /// 削除対象ユーザーリスト用JSON
    /// </summary>
    [BindProperty]
    public string RemoveItemsJson { set; get; }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="logger">ログインスタンス</param>
    /// <param name="userService">ユーザーメッセージサービス</param>
    /// <param name="receiverService">送信者サービス</param>
    public UserMaintenanceModel(ILogger<UserMaintenanceModel> logger, UserService userService, ReceiverService receiverService)
    {
      _logger = logger;
      this.userService = userService;
      this.receiverService = receiverService;
    }

    /// <summary>
    /// ページ表示
    /// </summary>
    public void OnGet()
    {
      // TODO:ページの権限チェック

      // ユーザー一覧取得
      var users = userService.GetList();
      var receivers = receiverService.GetList();
      var userMaintenanceModels = users.Select(user =>
      {
        var receiver = receivers.FirstOrDefault(receiver=>receiver.ID == user.ID);
        return new UserViewModel() {
          ID = user.ID,
          IsReceiver = receiver != null,
          DisplayName = receiver != null ? receiver.DisplayName : string.Empty,
          DisplayList = receiver != null && receiver.DisplayList,
          IsAdminRole = receiver != null && receiver.IsAdminRole
        };
      });

      // セッションに一覧を格納
      var userList = userMaintenanceModels.ToList();
      HttpContext.Session.Set(KEY_USER_LIST, JsonSerializer.SerializeToUtf8Bytes(userList));

      Users.Clear();
      Users.AddRange(userList);

      // ゼロ件の場合はエラー
      if (!Users.Any())
      {
        ViewData["Message"] = "ユーザーはありません。";
        return;
      }
    }

    /// <summary>
    /// ユーザー削除
    /// </summary>
    public void OnPostRemove()
    {
      // TODO:ページの権限チェック

      // 一覧復元
      if (HttpContext.Session.Keys.Contains(KEY_USER_LIST))
      {
        Users.Clear();
        var bytes = HttpContext.Session.Get(KEY_USER_LIST);
        Users.AddRange(JsonSerializer.Deserialize<List<UserViewModel>>(bytes));
      }

      if(!string.IsNullOrEmpty(RemoveItemsJson))
      {
        var json = this.RemoveItemsJson.Replace("\\", string.Empty).Replace("\"{", "{").Replace("}\"","}");
        var removeItems = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(json);

        // TODO 削除
      }
    }

    /// <summary>
    /// ユーザー一覧から編集クリック
    /// </summary>
    /// <param name="id">ユーザーID</param>
    public void OnPostEdit(string id)
    {
      // TODO:ページの権限チェック

      // 一覧復元
      if (HttpContext.Session.Keys.Contains(KEY_USER_LIST))
      {
        Users.Clear();
        var bytes = HttpContext.Session.Get(KEY_USER_LIST);
        Users.AddRange(JsonSerializer.Deserialize<List<UserViewModel>>(bytes));
      }

      if (string.IsNullOrEmpty(id))
      {
        return;
      }

      // ユーザーを反映
      IsEdit = true;
      var editTarget = userService.GetUser(id);
      var receiver = receiverService.GetReceiver(id);
      EditTarget.ID = editTarget.ID;
      EditTargetUserVersion = editTarget.Version;
      EditTargetReceiverVersion = -1;
      if (receiver != null)
      {
        EditTarget.IsReceiver = true;
        EditTarget.DisplayName = receiver.DisplayName;
        EditTarget.DisplayList = receiver.DisplayList;
        EditTarget.IsAdminRole= receiver.IsAdminRole;
        EditTargetReceiverVersion = receiver.Version;
      }
    }

    /// <summary>
    /// 編集項目クリア
    /// </summary>
    public void OnPostClear()
    {
      // TODO:ページの権限チェック

      // 一覧復元
      if (HttpContext.Session.Keys.Contains(KEY_USER_LIST))
      {
        Users.Clear();
        var bytes = HttpContext.Session.Get(KEY_USER_LIST);
        Users.AddRange(JsonSerializer.Deserialize<List<UserViewModel>>(bytes));
      }

      // 編集項目をクリア
      IsEdit = false;
      EditTarget.ID = string.Empty;
      EditTarget.IsReceiver = false;
      EditTarget.DisplayName = string.Empty;
      EditTarget.DisplayList = false;
      EditTarget.IsAdminRole = false;
      EditTargetUserVersion = -1;
      EditTargetReceiverVersion = -1;
    }

  }
}
