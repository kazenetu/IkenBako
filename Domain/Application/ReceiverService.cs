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
    /// <returns>送信対象者メッセージリスト</returns>
    public List<ReceiverModel> GetList()
    {
      var receivers = repository.GetReceivers();
      return receivers.Select(receiver => new ReceiverModel(receiver)).ToList();
    }
  }
}
