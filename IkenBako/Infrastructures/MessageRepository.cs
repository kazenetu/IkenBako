using IkenBako.Models;
using System.Collections.Generic;

namespace IkenBako.Infrastructures
{
  /// <summary>
  /// 意見メッセージリポジトリクラス
  /// </summary>
  public class MessageRepository: IMessageRepository
  {
    /// <summary>
    /// 保存
    /// </summary>
    /// <param name="message">意見メッセージクラス</param>
    /// <returns>保存結果turns>
    public bool Save(Message message)
    {
      return false;
    }

    /// <summary>
    /// 送信対象宛ての意見メッセージ取得
    /// </summary>
    /// <param name="sendTarget">送信対象者クラス</param>
    /// <returns>意見メッセージリスト</returns>
    public List<Message> FindMessage(SendTarget sendTarget)
    {
      var result = new List<Message>();
      return result;
    }
  }
}
