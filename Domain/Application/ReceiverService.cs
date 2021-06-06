using Domain.Application.Models;
using Domain.Domain.Receivers;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Application
{
  /// <summary>
  /// 送信対象者
  /// </summary>
  public class ReceiverService
  {
    private readonly IReceiverRepository repository;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="repository">送信対象者リポジトリ</param>
    public ReceiverService(IReceiverRepository repository)
    {
      this.repository = repository;
    }

    /// <summary>
    /// 送信対象者リストを取得
    /// </summary>
    /// <param name="isAddAllItem">すべての対象を追加</param>
    /// <returns>送信対象者メッセージリスト</returns>
    public List<ReceiverModel> GetList(bool isAddAllItem = false)
    {
      var receivers = repository.GetReceivers();

      // すべての対象を追加
      if (isAddAllItem)
      {
        receivers.Insert(0, Receiver.Create("すべて", ReceiverId.AllReceiverId));
      }

      return receivers.Select(receiver => new ReceiverModel(receiver)).ToList();
    }

    /// <summary>
    /// 送信担当者を取得
    /// </summary>
    /// <param name="unique_name">ユニーク名</param>
    /// <returns>送信担当者</returns>
    public ReceiverModel GetReceiver(string unique_name)
    {
      return new ReceiverModel(repository.GetReceiver(unique_name));
    }

    /// <summary>
    /// すべての送信対象か否か
    /// </summary>
    /// <param name="id">確認対象のID</param>
    /// <returns>すべての送信対象か否か</returns>
    public bool IsAllReceiverId(string id)
    {
      return id == ReceiverId.AllReceiverId;
    }
  }
}
