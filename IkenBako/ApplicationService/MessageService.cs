using IkenBako.Infrastructures;
using IkenBako.Models;
using System.Collections.Generic;

namespace IkenBako.ApplicationService
{
  /// <summary>
  /// 意見メッセージ用サービス
  /// </summary>
  public static class MessageService
  {
    /// <summary>
    /// 意見メッセージの保存
    /// </summary>
    /// <param name="message">意見メッセージクラスインスタンス</param>
    public static void Save(Message message)
    {
      using (var repository = new MessageRepository())
      {
        Save(message, repository);
      }
    }

    /// <summary>
    /// 意見メッセージの保存
    /// </summary>
    /// <param name="message">意見メッセージクラスインスタンス</param>
    /// <param name="repository">意見メッセージリポジトリ</param>
    public static void Save(Message message, IMessageRepository repository)
    {
      message.Save(repository);
    }

    /// <summary>
    /// 送信対象宛ての意見メッセージ取得
    /// </summary>
    /// <param name="sendTarget">送信対象者クラス</param>
    /// <returns>意見メッセージリスト</returns>
    public static List<Message> FindMessage(SendTarget sendTarget)
    {
      using (var repository = new MessageRepository())
      {
        return FindMessage(sendTarget, repository);
      }
    }

    /// <summary>
    /// 送信対象宛ての意見メッセージ取得
    /// </summary>
    /// <param name="sendTarget">送信対象者クラス</param>
    /// <returns>意見メッセージリスト</returns>
    /// <param name="repository">意見メッセージリポジトリ</param>
    public static List<Message> FindMessage(SendTarget sendTarget, IMessageRepository repository)
    {
      return repository.FindMessage(sendTarget);
    }

  }
}
