using System;

namespace Domain.Domain.Receivers
{
  /// <summary>
  /// 送信対象
  /// </summary>
  public class Receiver
  {
    /// <summary>
    /// 送信表示象名称
    /// </summary>
    public string DisplayName { get; private set; }

    /// <summary>
    /// 送信対象ID
    /// </summary>
    /// <remarks>アルファベットで表現すること</remarks>
    public ReceiverId ID { get; private set; }

    /// <summary>
    /// 送信元表示
    /// </summary>
    /// <remarks>送信先に表示するか</remarks>
    public bool DisplayList { get; private set; } = true;

    /// <summary>
    /// 管理者権限
    /// </summary>
    /// <remarks>一覧ですべてを選択できるか</remarks>
    public bool IsAdminRole { get; private set; } = false;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="displayName">送信対象表示名</param>
    /// <param name="id">送信対象ID</param>
    private Receiver(string displayName, ReceiverId id)
    {
      DisplayName = displayName;
      ID = id;
    }

    /// <summary>
    /// 送信対象インスタンスの作成
    /// </summary>
    /// <param name="displayName">送信対象表示名</param>
    /// <param name="id">送信対象ID</param>
    public static Receiver Create(string displayName, string id)
    {
      return new Receiver(displayName, new ReceiverId(id));
    }

    /// <summary>
    /// 送信対象インスタンスの作成
    /// </summary>
    /// <param name="displayName">送信対象表示名</param>
    /// <param name="id">送信対象ID</param>
    /// <param name="displayList">送信先に表示するか</param>
    /// <param name="isAdminRole">一覧ですべてを選択できるか</param>
    public static Receiver Create(string displayName, string id, bool displayList, bool isAdminRole)
    {
      return new Receiver(displayName, new ReceiverId(id)){DisplayList = displayList, IsAdminRole= isAdminRole};
    }
  }
}
