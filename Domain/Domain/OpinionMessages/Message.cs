namespace Domain.Domain.OpinionMessages
{
  /// <summary>
  /// 意見メッセージ
  /// </summary>
  public class Message
  {
    /// <summary>
    /// メッセージ対象
    /// </summary>
    public string SendTo { get; private set; }

    /// <summary>
    /// メッセージ本文
    /// </summary>
    public string Detail { get; private set; }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    private Message()
    {
    }

    /// <summary>
    /// インスタンス生成
    /// </summary>
    /// <param name="sendTo">メッセージ対象</param>
    /// <param name="detail">メッセージ本文</param>
    /// <returns>インスタンス</returns>
    public static Message Create(string sendTo, string detail)
    {
      return new Message()
      {
        SendTo = sendTo,
        Detail = detail
      };
    }
  }
}
