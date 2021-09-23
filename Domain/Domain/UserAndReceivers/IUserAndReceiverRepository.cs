using Domain.Domain.Users;
using System.Collections.Generic;

namespace Domain.Domain.UserAndReceivers
{
  /// <summary>
  /// ユーザーと受信者の集約Entityインターフェース
  /// </summary>
  public interface IUserAndReceiverRepository : IRepositoryBase
  {
    /// <summary>
    /// ユーザーを取得
    /// </summary>
    /// <param name="userId">ユーザーID</param>
    /// <returns>ユーザー</returns>
    UserAndReceiver GetUserAndReceiver(UserId userId);

    /// <summary>
    /// ユーザーリストを取得
    /// </summary>
    /// <returns>ユーザーリスト</returns>
    List<UserAndReceiver> GetUserAndReceivers();

    /// <summary>
    /// ユーザーの保存
    /// </summary>
    /// <param name="target">ユーザーと受信者の集約Entity</param>
    /// <returns>保存成否</returns>
    bool Save(UserAndReceiver target);

    /// <summary>
    /// ユーザーの削除
    /// </summary>
    /// <param name="userId">ユーザーID</param>
    /// <returns>削除成否</returns>
    bool Remove(UserId userId);
  }
}
