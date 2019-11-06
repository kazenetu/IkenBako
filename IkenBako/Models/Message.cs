namespace IkenBako.Models
{
  /// <summary>
  /// 意見メッセージ
  /// </summary>
  public class Message
  {
    /// <summary>
    /// メッセージ対象
    /// </summary>
    public string SendTo { get; set; } = "";

    /// <summary>
    /// メッセージ本文
    /// </summary>
    public string Detail { get; set; }
  }
}
