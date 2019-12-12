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
    /// 送信者サービス
    /// </summary>
    private readonly ReceiverService receiverService;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="repository">意見メッセージリポジトリ</param>
    public MessageService(IMessageRepository repository, ReceiverService receiverService)
    {
      this.repository = repository;
      this.receiverService = receiverService;
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
    public List<DisplayMessageModel> Find(string receiverId)
    {
      var messages = repository.Find(new ReceiverId(receiverId));
      var receivers = receiverService.GetList();
      return messages.Select(message => new DisplayMessageModel(message, GetSendTargetName(message.SendTo.Value))).ToList();

      // 送信対象者の名称を取得
      string GetSendTargetName(string sendToId)
      {
        var result = receivers.Where(receiver => receiver.ID == sendToId);
        if (result.Any())
        {
          return result.First().DisplayName;
        }
        return string.Empty;
      }
    }

  }
}
