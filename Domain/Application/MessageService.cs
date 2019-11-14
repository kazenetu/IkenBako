using Domain.Application.Models;
using Domain.Domain.OpinionMessages;
using Domain.Domain.Receivers;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Application
{
  /// <summary>
  /// 意見メッセージ用サービス
  /// </summary>
  public class MessageService
  {
    private readonly IMessageRepository repository;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="repository">意見メッセージリポジトリ</param>
    public MessageService(IMessageRepository repository)
    {
      this.repository = repository;
    }

    /// <summary>
    /// 意見メッセージの保存
    /// </summary>
    /// <param name="sendTo">送信対象者</param>
    /// <param name="detail">メッセージ本文</param>
    public void Save(string sendTo, string detail)
    {
      repository.Save(Message.Create(sendTo, detail));
    }

    /// <summary>
    /// 送信対象宛ての意見メッセージ取得
    /// </summary>
    /// <param name="receiverId">送信対象ID</param>
    /// <returns>意見メッセージリスト</returns>
    public List<MessageModel> FindMessage(string receiverId)
    {
      var messages = repository.FindMessage(new ReceiverId(receiverId));
      return messages.Select(message => new MessageModel(message)).ToList();
    }

  }
}
