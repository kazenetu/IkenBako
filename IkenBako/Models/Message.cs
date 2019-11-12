using IkenBako.Infrastructures;

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
    /// 保存
    /// </summary>
    /// <param name="messageRepository">意見メッセージ保存リポジトリクラスインスタンス</param>
    public void Save(IMessageRepository messageRepository)
    {
      messageRepository.Save(this);
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
