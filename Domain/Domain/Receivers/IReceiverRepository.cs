using System.Collections.Generic;

namespace Domain.Domain.Receivers
{
  /// <summary>
  /// 受信者リポジトリインターフェース
  /// </summary>
  public interface IReceiverRepository: IRepositoryBase
  {
    /// <summary>
    /// 受信者リストを取得
    /// </summary>
    /// <returns>受信者メッセージリスト</returns>
    List<Receiver> GetReceivers();

    /// <summary>
    /// 受信者を取得
    /// </summary>
    /// <param name="unique_name">ユニーク名</param>
    /// <returns>受信者</returns>
    Receiver GetReceiver(string unique_name);
  }
}
