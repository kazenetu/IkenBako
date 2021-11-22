using Domain.Application;
using Domain.Domain.Users;
using IkenBako.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
    private const string KEY_PAGE_INDEX = "KEY_PAGE_INDEX";
    private const string KEY_CREATED_ID = "KEY_EDITED_ID";
    private const int PAGE_SIZE = 4;
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
    /// 表示用ユーザーリスト
    /// </summary>
    public PaginatedList<UserViewModel> DisplayUsers  { get; private set; } 

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
    public IActionResult OnGet()
    {
      // ページの閲覧権限チェック
      if (!CanUsePage())
      {
        return RedirectToPage("/Login");
      }

      // ユーザー一覧取得
      var userMaintenanceModels = userService.GetList().Select(user =>
      {
        return new UserViewModel() {
          ID = user.ID,
          IsReceiver = user.Receiver != null,
          DisplayName = user.Receiver != null ? user.Receiver.DisplayName : string.Empty,
          DisplayList = user.Receiver != null && user.Receiver.DisplayList,
          IsAdminRole = user.Receiver != null && user.Receiver.IsAdminRole,
          IsViewListRole = user.Receiver != null && user.Receiver.IsViewListRole,
          Disabled = user.Disabled
        };
      });

      // セッションに一覧を格納
      var userList = userMaintenanceModels.ToList();
      HttpContext.Session.Set(KEY_USER_LIST, JsonSerializer.SerializeToUtf8Bytes(userList));

      // 新規作成している場合は対象ページを開く
      if (HttpContext.Session.Keys.Contains(KEY_CREATED_ID))
      {
        var createdId = HttpContext.Session.GetString(KEY_CREATED_ID);

        // リスト内に対象IDが存在するか確認
        var targetIndex = -1;
        for (var i = 0; i < userList.Count; i++)
        {
          if (userList[i].ID == createdId)
          {
            targetIndex = i;
            break;
          }
        }

        // 新規作成IDを削除
        HttpContext.Session.Remove(KEY_CREATED_ID);

        // リスト内に新規作成が存在する場合はインデックスからページ数を設定する
        if (targetIndex >= 0)
        {
          HttpContext.Session.SetInt32(KEY_PAGE_INDEX, (targetIndex / PAGE_SIZE) + 1); // ページ数は1ベースのため+1する
        }
      }


      Users.Clear();
      Users.AddRange(userList);

      // ゼロ件の場合はエラー
      if (!Users.Any())
      {
        ViewData["Message"] = "ユーザーはありません。";
      }

      DisplayUsers = GetDisplayUsers(userList);

      return Page();
    }

    /// <summary>
    /// ページ変更
    /// </summary>
    /// <param name="pageIndex">変更後のページインデックス</param>
    public IActionResult OnPostPageChange(string pageIndex)
    {
      // ページの閲覧権限チェック
      if (!CanUsePage())
      {
        return RedirectToPage("/Login");
      }

      // 一覧復元
      if (HttpContext.Session.Keys.Contains(KEY_USER_LIST))
      {
        Users.Clear();
        var bytes = HttpContext.Session.Get(KEY_USER_LIST);
        Users.AddRange(JsonSerializer.Deserialize<List<UserViewModel>>(bytes));

        var pageIndexValue = 1;
        if(!int.TryParse(pageIndex, out pageIndexValue))
        {
          pageIndexValue = 1;
        }

        DisplayUsers = GetDisplayUsers(Users, pageIndexValue);
      }

      // 再表示
      return RedirectToPage();
    }

    /// <summary>
    /// ユーザー削除
    /// </summary>
    public IActionResult OnPostRemove()
    {
      // ページの閲覧権限チェック
      if (!CanUsePage())
      {
        return RedirectToPage("/Login");
      }

      // 一覧復元
      if (HttpContext.Session.Keys.Contains(KEY_USER_LIST))
      {
        Users.Clear();
        var bytes = HttpContext.Session.Get(KEY_USER_LIST);
        Users.AddRange(JsonSerializer.Deserialize<List<UserViewModel>>(bytes));
        DisplayUsers = GetDisplayUsers(Users);
      }

      var errorMessages = new List<string>();
      if(!string.IsNullOrEmpty(RemoveItemsJson))
      {
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
      }
      else
      {
        errorMessages.Add("削除対象を選択してください。");
      }
      
      // エラーがある場合は終了
      if (errorMessages.Any())
      {
        ViewData["ErrorMessages"] = errorMessages;
        return Page();
      }

      // 再表示
      return RedirectToPage();
    }

    /// <summary>
    /// ユーザー一覧から編集クリック
    /// </summary>
    /// <param name="id">ユーザーID</param>
    public IActionResult OnPostEdit(string id)
    {
      // ページの閲覧権限チェック
      if (!CanUsePage())
      {
        return RedirectToPage("/Login");
      }

      // 一覧復元
      if (HttpContext.Session.Keys.Contains(KEY_USER_LIST))
      {
        Users.Clear();
        var bytes = HttpContext.Session.Get(KEY_USER_LIST);
        Users.AddRange(JsonSerializer.Deserialize<List<UserViewModel>>(bytes));
        DisplayUsers = GetDisplayUsers(Users);
      }

      if (string.IsNullOrEmpty(id))
      {
        return Page();
      }

      // ユーザーを反映
      IsEdit = true;
      var editTarget = userService.GetUser(id);
      var receiver = receiverService.GetReceiver(id);
      EditTarget.ID = editTarget.ID;
      EditTarget.Disabled = editTarget.Disabled;
      EditTargetUserVersion = editTarget.Version;
      EditTargetReceiverVersion = -1;
      if (receiver != null)
      {
        EditTarget.IsReceiver = true;
        EditTarget.DisplayName = receiver.DisplayName;
        EditTarget.DisplayList = receiver.DisplayList;
        EditTarget.IsAdminRole= receiver.IsAdminRole;
        EditTarget.IsViewListRole= receiver.IsViewListRole;
        EditTargetReceiverVersion = receiver.Version;
      }

      // 表示
      return Page();
    }

    /// <summary>
    /// URLでIDの指定
    /// </summary>
    /// <param name="id">ユーザーID</param>
    public IActionResult OnGetEdit(string id)
    {
      // ページの閲覧権限チェック
      if (!CanUsePage())
      {
        return RedirectToPage("/Login");
      }

      // URLでIDを指定した場合は強制的にGetメソッドを呼ぶ
      return RedirectToPage();
    }

    /// <summary>
    /// 編集項目クリア
    /// </summary>
    public IActionResult OnPostClear()
    {
      // ページの閲覧権限チェック
      if (!CanUsePage())
      {
        return RedirectToPage("/Login");
      }

      // 一覧復元
      if (HttpContext.Session.Keys.Contains(KEY_USER_LIST))
      {
        Users.Clear();
        var bytes = HttpContext.Session.Get(KEY_USER_LIST);
        Users.AddRange(JsonSerializer.Deserialize<List<UserViewModel>>(bytes));
        DisplayUsers = GetDisplayUsers(Users);
      }

      // 編集項目をクリア
      IsEdit = false;
      EditPassword = string.Empty;
      EditIsSetPassword = false;
      EditTarget.ID = string.Empty;
      EditTarget.Disabled = false;
      EditTarget.IsReceiver = false;
      EditTarget.DisplayName = string.Empty;
      EditTarget.DisplayList = false;
      EditTarget.IsAdminRole = false;
      EditTarget.IsViewListRole= true;
      EditTargetUserVersion = VERSION_NONE;
      EditTargetReceiverVersion = VERSION_NONE;

      // 表示
      return RedirectToPage();
    }

    /// <summary>
    /// 保存
    /// </summary>
    public IActionResult OnPostSave()
    {
      // ページの閲覧権限チェック
      if (!CanUsePage())
      {
        return RedirectToPage("/Login");
      }

      // 一覧復元
      if (HttpContext.Session.Keys.Contains(KEY_USER_LIST))
      {
        Users.Clear();
        var bytes = HttpContext.Session.Get(KEY_USER_LIST);
        Users.AddRange(JsonSerializer.Deserialize<List<UserViewModel>>(bytes));
        DisplayUsers = GetDisplayUsers(Users);
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
                                      EditTarget.IsReceiver, EditTarget.DisplayName, EditTarget.DisplayList, EditTarget.IsAdminRole, EditTarget.IsViewListRole, EditTargetReceiverVersion, 
                                      newPassword, EditTarget.Disabled);

      if (dbResult)
      {
        // 新規作成の場合は専用セッションにユーザーIDを格納
        if (!IsEdit)
        {
          HttpContext.Session.SetString(KEY_CREATED_ID, EditTarget.ID); 
        }

        // 登録成功時は一覧の再表示
        return RedirectToPage();
      }

      // 失敗時はエラーメッセージを追加して表示
      errorMessages.Add("保存に失敗しました。");
      ViewData["ErrorMessages"] = errorMessages;
      return Page();

    }

    /// <summary>
    /// 本ページが利用可能か
    /// </summary>
    /// <returns>利用可能/不可</returns>
    private bool CanUsePage()
    {
      if (!HttpContext.Session.Keys.Contains(LoginModel.KEY_ADMIN))
      {
        return false;
      }

      return true;
    }

    /// <summary>
    /// 表示用ユーザー一覧の取得
    /// </summary>
    /// <param name="srcList">ユーザー一覧</param>
    /// <param name="changedIndex">変更後のページインデックス</param>
    /// <returns>表示用ユーザー一覧</returns>
    private PaginatedList<UserViewModel> GetDisplayUsers(List<UserViewModel> srcList, int changedIndex = 0)
    {
      // ページインデックスをセッションから取得
      var pageIndex = 1;
      if (HttpContext.Session.Keys.Contains(KEY_PAGE_INDEX))
      {
        var sessionPageIndex = HttpContext.Session.GetInt32(KEY_PAGE_INDEX);
        if (sessionPageIndex.HasValue)
          pageIndex = sessionPageIndex.Value;
      }

      if (changedIndex != 0)
      {
        // 変更後のページインデックスを反映
        pageIndex = changedIndex;
      }

      // ページング機能付きリストを生成
      var result = PaginatedList<UserViewModel>.Create(srcList.AsQueryable(), pageIndex, PAGE_SIZE);

      if (changedIndex != 0)
      {
        // 正しく訂正された変更後のページインデックスをセッションに格納
        HttpContext.Session.SetInt32(KEY_PAGE_INDEX, result.PageIndex);
      }

      return result;
    }
  }
}
