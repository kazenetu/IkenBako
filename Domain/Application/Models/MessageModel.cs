using Domain.Domain.OpinionMessages;

namespace Domain.Application.Models
{
  /// <summary>
  /// 意見メッセージ
  /// </summary>
  public class MessageModel
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
    /// <param name="source">意見メッセージインスタンス</param>
    public MessageModel(Message source)
    {
      SendTo = source.SendTo;
      Detail = source.Detail;
    }
  }
}
