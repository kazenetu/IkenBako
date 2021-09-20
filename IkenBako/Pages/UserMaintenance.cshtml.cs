using Domain.Application;
using Domain.Domain.Users;
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
    private const string KEY_USER_LIST = "KEY_USER_LIST";
    private const int VERSION_NONE = -1;

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
    /// パスワード設定を実施するか否か
    /// </summary>
    [BindProperty]
    public bool EditIsSetPassword { get; set; } = false;

    /// <summary>
    /// パスワード
    /// </summary>
    [BindProperty]
    public string EditPassword { get; set; } = "";

    /// <summary>
    /// 編集中のユーザーマスタのバージョン
    /// </summary>
    [BindProperty]
    public int EditTargetUserVersion { set; get; } = VERSION_NONE;

    /// <summary>
    /// 編集中の受信者マスタのバージョン
    /// </summary>
    [BindProperty]
    public int EditTargetReceiverVersion { set; get; } = VERSION_NONE;

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
      var userMaintenanceModels = userService.GetList().Select(user =>
      {
        return new UserViewModel() {
          ID = user.ID,
          IsReceiver = user.Receiver != null,
          DisplayName = user.Receiver != null ? user.Receiver.DisplayName : string.Empty,
          DisplayList = user.Receiver != null && user.Receiver.DisplayList,
          IsAdminRole = user.Receiver != null && user.Receiver.IsAdminRole
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
    public IActionResult OnPostRemove()
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
        var errorMessages = new List<string>();

        // JSON文字列をデシアライズ
        var json = RemoveItemsJson.Replace("\\", string.Empty).Replace("\"{", "{").Replace("}\"","}");
        var jsonDictionary = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(json);

        // dataキーが存在するか
        if (!jsonDictionary.ContainsKey("data"))
        {
          errorMessages.Add("削除対象を選択してください。");
        }

        // UserIDリストに変換
        var userIds = jsonDictionary["data"].Select(id=>new UserId(id)).ToList();

        // 削除
        if (!userService.RemoveAll(userIds))
        {
          errorMessages.Add("削除に失敗しました。");
        }

        // エラーがある場合は終了
        if (errorMessages.Any())
        {
          ViewData["ErrorMessages"] = errorMessages;
          return Page();
        }
      }

      // 再表示
      return RedirectToPage();
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
      EditPassword = string.Empty;
      EditIsSetPassword = false;
      EditTarget.ID = string.Empty;
      EditTarget.IsReceiver = false;
      EditTarget.DisplayName = string.Empty;
      EditTarget.DisplayList = false;
      EditTarget.IsAdminRole = false;
      EditTargetUserVersion = VERSION_NONE;
      EditTargetReceiverVersion = VERSION_NONE;
    }

    /// <summary>
    /// 保存
    /// </summary>
    public IActionResult OnPostSave()
    {
      // TODO:ページの権限チェック

      // 一覧復元
      if (HttpContext.Session.Keys.Contains(KEY_USER_LIST))
      {
        Users.Clear();
        var bytes = HttpContext.Session.Get(KEY_USER_LIST);
        Users.AddRange(JsonSerializer.Deserialize<List<UserViewModel>>(bytes));
      }

      // 入力チェック
      var errorMessages = new List<string>();
      if (IsEdit)
      {
        // 既存ユーザー
        if(string.IsNullOrEmpty(EditTarget.ID) || EditTargetUserVersion == VERSION_NONE)
        {
          errorMessages.Add("ユーザーが正しく取得されませんでした。");
        }
        else
        {
          // バージョンチェック
          var userModel = userService.GetUser(EditTarget.ID);
          if(EditTargetUserVersion != userModel.Version)
          {
            errorMessages.Add("すでに更新されています。一覧の編集ボタンから再度編集してください。");
          }

          var receiverModel = receiverService.GetReceiver(EditTarget.ID);
          if (receiverModel != null && EditTargetReceiverVersion != receiverModel.Version)
          {
            errorMessages.Add("すでに更新されています。一覧の編集ボタンから再度編集してください。");
          }
        }
      }
      else
      {
        // 新規登録
        if(string.IsNullOrEmpty(EditTarget.ID))
        {
          errorMessages.Add("IDを入力してください。");
        }
        else
        {
          // 既存にユーザーと同名か確認
          if(userService.GetUser(EditTarget.ID) != null)
          {
            errorMessages.Add("IDを変更してください。すでにユーザーが存在します。");
          }
        }

        // パスワード
        if (!EditIsSetPassword || string.IsNullOrEmpty(EditPassword))
        {
          errorMessages.Add("パスワードは必須です。");
        }
      }
      // 共通
      if (receiverService.IsAllReceiverId(EditTarget.ID))
      {
        errorMessages.Add($"このIDは使えません。");
      }
      if (EditIsSetPassword && string.IsNullOrEmpty(EditPassword))
      {
        errorMessages.Add("パスワードを入力してください。");
      }
      if (EditTarget.IsReceiver)
      {
        if (string.IsNullOrEmpty(EditTarget.DisplayName))
        {
          errorMessages.Add("受信者名を入力してください。");
        }
      }
      if (errorMessages.Any())
      {
        ViewData["ErrorMessages"] = errorMessages;
        return Page();
      }

      // 保存処理
      string newPassword = null;
      if (EditIsSetPassword)
      {
        newPassword = EditPassword;
      }
      var dbResult = userService.Save(EditTarget.ID, EditTargetUserVersion, 
                                      EditTarget.IsReceiver, EditTarget.DisplayName, EditTarget.DisplayList, EditTarget.IsAdminRole, EditTargetReceiverVersion, 
                                      newPassword);

      if (dbResult)
      {
        // 登録成功時は一覧の再表示
        return RedirectToPage();
      }

      // 失敗時はエラーメッセージを追加して表示
      errorMessages.Add("保存に失敗しました。");
      ViewData["ErrorMessages"] = errorMessages;
      return Page();

    }
  }
}
