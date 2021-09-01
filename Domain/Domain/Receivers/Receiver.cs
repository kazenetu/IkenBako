using System;

namespace Domain.Domain.Receivers
{
  /// <summary>
  /// 受信者マスタ
  /// </summary>
  public class Receiver
  {
    /// <summary>
    /// 受信者表示象名称
    /// </summary>
    public string DisplayName { get; private set; }

    /// <summary>
    /// 受信者ID
    /// </summary>
    /// <remarks>アルファベットで表現すること</remarks>
    public ReceiverId ID { get; private set; }

    /// <summary>
    /// 送信先表示
    /// </summary>
    /// <remarks>送信先に表示するか</remarks>
    public bool DisplayList { get; private set; } = true;

    /// <summary>
    /// 管理者権限
    /// </summary>
    /// <remarks>一覧ですべてを選択できるか</remarks>
    public bool IsAdminRole { get; private set; } = false;

    /// <summary>
    /// 更新バージョン
    /// </summary>
    public int Version{ get; private set; } = 1;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="displayName">受信者表示名</param>
    /// <param name="id">受信者ID</param>
    /// <param name="version">更新バージョン</param>
    private Receiver(string displayName, ReceiverId id, int version)
    {
      DisplayName = displayName;
      ID = id;
      Version = version;
    }

    /// <summary>
    /// 受信者インスタンスの作成
    /// </summary>
    /// <param name="displayName">受信者表示名</param>
    /// <param name="id">受信者ID</param>
    /// <param name="version">更新バージョン</param>
    public static Receiver Create(string displayName, string id, int version = 1)
    {
      return new Receiver(displayName, new ReceiverId(id), version);
    }

    /// <summary>
    /// 受信者インスタンスの作成
    /// </summary>
    /// <param name="displayName">受信者表示名</param>
    /// <param name="id">受信者ID</param>
    /// <param name="displayList">送信先に表示するか</param>
    /// <param name="isAdminRole">一覧ですべてを選択できるか</param>
    /// <param name="version">更新バージョン</param>
    public static Receiver Create(string displayName, string id, bool displayList, bool isAdminRole, int version = 1)
    {
      return new Receiver(displayName, new ReceiverId(id), version){DisplayList = displayList, IsAdminRole= isAdminRole};
    }
  }
}
