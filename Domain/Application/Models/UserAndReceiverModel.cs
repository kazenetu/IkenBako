using Domain.Domain.Receivers;
using Domain.Domain.Users;

namespace Domain.Application.Models
{
  /// <summary>
  /// ユーザーと受信者の集約モデル
  /// </summary>
  public class UserAndReceiverModel: UserModel
  {
    /// <summary>
    /// 受信者Model
    /// </summary>
    public ReceiverModel Receiver { get; private set; } = null;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="user">ユーザーエンティティ</param>
    /// <param name="receiver">受信者エンティティ</param>
    public UserAndReceiverModel(User user,Receiver receiver) : base(user)
    {
      if(receiver != null)
      {
        Receiver = new ReceiverModel(receiver);
      }
    }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="user">ユーザーエンティティ</param>
    public UserAndReceiverModel(User user) : base(user)
    {
    }
  }
}
