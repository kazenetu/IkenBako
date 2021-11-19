using Domain.Domain.Receivers;
using Domain.Domain.Users;

namespace Domain.Domain.UserAndReceivers
{
  /// <summary>
  /// ユーザーと受信者の集約Entity
  /// </summary>
  public class UserAndReceiver : User
  {
    /// <summary>
    /// ユーザーの受信者エンティティ
    /// </summary>
    /// <remarks>存在しない場合はnull</remarks>
    public Receiver UserReceiver { get; private set; } = null;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="user">ユーザーエンティティ</param>
    /// <param name="receiver">受信者エンティティ</param>
    /// <remarks>受信者エンティティはnullを許容</remarks>
    private UserAndReceiver(User user, Receiver receiver) : base(user.ID, user.Password, user.Salt, user.Disabled ,user.Version)
    {
      UserReceiver = receiver;
    }

    /// <summary>
    /// インスタンスの作成
    /// </summary>
    /// <param name="user">ユーザーエンティティ</param>
    /// <param name="receiver">受信者エンティティ</param>
    /// <remarks>受信者エンティティはnullを許容</remarks>
    public static UserAndReceiver Create(User user, Receiver receiver)
    {
      return new UserAndReceiver(user, receiver);
    }

    /// <summary>
    /// インスタンスの作成
    /// </summary>
    /// <param name="user">ユーザーエンティティ</param>
    public static UserAndReceiver Create(User user)
    {
      return new UserAndReceiver(user, null);
    }
  }
}
