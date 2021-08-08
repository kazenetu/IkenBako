using System.Collections.Generic;

namespace Domain.Domain.Receivers
{
  /// <summary>
  /// 送信対象者リポジトリインターフェース
  /// </summary>
  public interface IReceiverRepository: IRepositoryBase
  {
    /// <summary>
    /// 送信対象者リストを取得
    /// </summary>
    /// <returns>送信対象者メッセージリスト</returns>
    List<Receiver> GetReceivers();

    /// <summary>
    /// 送信担当者を取得
    /// </summary>
    /// <param name="unique_name">ユニーク名</param>
    /// <returns>送信担当者</returns>
    Receiver GetReceiver(string unique_name);
  }
}
