using IkenBako.Models;
using System.Collections.Generic;
namespace IkenBako.Infrastructures
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
    /// <param name="sendTarget">送信対象者クラス</param>
    /// <returns>意見メッセージリスト</returns>
    List<Message> FindMessage(SendTarget sendTarget);
  }
}
