using Domain.Domain.OpinionMessages;

namespace Domain.Application.Models
{
  /// <summary>
  /// 表示用意見メッセージモデル
  /// </summary>
  public class DisplayMessageModel : MessageModel
  {
    /// <summary>
    /// 送信対象者名称
    /// </summary>
    public string SendTargetName { get; private set; }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="source">意見メッセージインスタンス</param>
    /// <param name="sendTargetName">送信対象者名称</param>
    public DisplayMessageModel(Message source, string sendTargetName) : base(source)
    {
      SendTargetName = sendTargetName;
    }
  }
}
