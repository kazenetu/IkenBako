using Microsoft.AspNetCore.Mvc;

namespace IkenBako.ViewModels
{
  /// <summary>
  /// ユーザーメンテナンス
  /// </summary>
  public class UserViewModel
  {
    /// <summary>
    /// ユーザーID
    /// </summary>
    /// <remarks>アルファベットで表現すること</remarks>
    public string ID { get; set; } = "";

    /// <summary>
    /// 受信者か否か
    /// </summary>
    public bool IsReceiver { set; get; } = false;

    /// <summary>
    /// 送信表示象名称
    /// </summary>
    public string DisplayName { get; set; } = "";

    /// <summary>
    /// 送信元表示
    /// </summary>
    /// <remarks>送信先に表示するか</remarks>
    public bool DisplayList { get; set; } = false;

    /// <summary>
    /// 管理者権限
    /// </summary>
    /// <remarks>一覧ですべてを選択できるか</remarks>
    public bool IsAdminRole { get; set; } = false;

    /// <summary>
    /// 一覧確認権限
    /// </summary>
    /// <remarks>一覧の表示権限があるか</remarks>
    public bool IsViewListRole { get; set; } = true;

    #region 表示用
    /// <summary>
    /// 一覧表示用受信者か否か
    /// </summary>
    public string ViewIsReceiver {
      get
      {
        return IsReceiver ? "〇" : "×";
      }
    }

    /// <summary>
    /// 一覧表示用送信表示象名称
    /// </summary>
    public string ViewDisplayName
    {
      get
      {
        return IsReceiver ? DisplayName : "-";
      }
    }

    /// <summary>
    /// 一覧表示用送信元表示
    /// </summary>
    /// <remarks>送信先に表示するか</remarks>
    public string ViewDisplayList
    {
      get
      {
        var result = DisplayList ? "〇" : "×";
        return IsReceiver ? result : "-";
      }
    }

    /// <summary>
    /// 一覧表示用管理者権限
    /// </summary>
    /// <remarks>一覧ですべてを選択できるか</remarks>
    public string ViewIsAdminRole
    {
      get
      {
        var result = IsAdminRole ? "〇" : "×";
        return IsReceiver ? result : "-";
      }
    }

    /// <summary>
    /// 一覧確認権限
    /// </summary>
    /// <remarks>一覧の表示権限があるか</remarks>
    public string ViewIsViewListRole
    {
      get
      {
        var result = IsViewListRole ? "〇" : "×";
        return IsReceiver ? result : "-";
      }
    }

    #endregion

  }
}
