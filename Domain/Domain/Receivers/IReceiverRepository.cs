using System.Collections.Generic;

namespace Domain.Domain.Receivers
{
  /// <summary>
  /// 送信対象者リポジトリインターフェース
  /// </summary>
  public interface IReceiverRepository
  {
    /// <summary>
    /// 送信対象者リストを取得
    /// </summary>
    /// <returns>送信対象者メッセージリスト</returns>
    List<Receiver> GetReceivers();
  }
}
