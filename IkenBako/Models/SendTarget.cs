namespace IkenBako.Models
{
  /// <summary>
  /// 送信対象
  /// </summary>
  public class SendTarget
  {
    /// <summary>
    /// 送信表示象名称
    /// </summary>
    public string DisplayName { get; private set; }

    /// <summary>
    /// 送信対象ID
    /// </summary>
    /// <remarks>アルファベットで表現すること</remarks>
    public string ID { get; private set; }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="displayName">送信対象表示名</param>
    /// <param name="id">送信対象ID</param>
    private SendTarget(string displayName, string id)
    {
      DisplayName = displayName;
      ID = id;
    }

    /// <summary>
    /// 送信対象インスタンスの作成
    /// </summary>
    /// <param name="displayName">送信対象表示名</param>
    /// <param name="id">送信対象ID</param>
    public static SendTarget Create(string displayName, string id)
    {
      return new SendTarget(displayName, id);
    }
  }
}
