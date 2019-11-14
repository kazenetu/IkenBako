using Domain.Domain.Receivers;
using System.Collections.Generic;

namespace Infrastructures
{
  /// <summary>
  /// 送信対象者リポジトリ
  /// </summary>
  public class ReceiverRepository
  {
    /// <summary>
    /// 送信対象者リストを取得
    /// </summary>
    /// <returns>送信対象者メッセージリスト</returns>
    public List<Receiver> GetReceivers()
    {
      var result = new List<Receiver>();
      result.Add(Receiver.Create("Aさん", "aa"));
      result.Add(Receiver.Create("Bさん", "bb"));
      result.Add(Receiver.Create("Cさん", "cc"));

      return result;
    }
  }
}
