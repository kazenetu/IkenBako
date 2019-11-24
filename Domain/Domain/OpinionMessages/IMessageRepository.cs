using Domain.Domain.Receivers;
using System.Collections.Generic;

namespace Domain.Domain.OpinionMessages
{
  /// <summary>
  /// 意見メッセージリポジトリインターフェース
  /// </summary>
  public interface IMessageRepository
  {
    /// <summary>
    /// 保存
    /// </summary>
    /// <param name="message">意見メッセージクラス</param>
    void Save(Message message);

    /// <summary>
    /// 送信対象宛ての意見メッセージ取得
    /// </summary>
    /// <param name="receiverId">送信対象ID</param>
    /// <returns>意見メッセージリスト</returns>
    List<Message> Find(ReceiverId receiverId);

    /// <summary>
    /// すべての意見メッセージ取得
    /// </summary>
    /// <returns>意見メッセージリスト</returns>
    List<Message> FindAll();
  }
}
