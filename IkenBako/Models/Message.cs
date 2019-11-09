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
    public string SendTo { get; set; } = "";

    /// <summary>
    /// メッセージ本文
    /// </summary>
    public string Detail { get; set; }

    /// <summary>
    /// 保存
    /// </summary>
    /// <param name="messageRepository">意見メッセージ保存リポジトリクラスインスタンス</param>
    /// <returns>保存結果</returns>
    public bool Save(IMessageRepository messageRepository)
    {
      return messageRepository.Save(this);
    }
  }
}
