namespace IkenBako.Models
{
  /// <summary>
  /// 送信対象
  /// </summary>
  public class SendTarget
  {
    /// <summary>
    /// 送信対象名
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="name">送信対象名</param>
    private SendTarget(string name)
    {
      Name = name;
    }

    /// <summary>
    /// 送信対象インスタンスの作成
    /// </summary>
    /// <param name="name">送信対象名</param>
    /// <returns>送信対象インスタンス</returns>
    public static SendTarget Create(string name)
    {
      return new SendTarget(name);
    }
  }
}
