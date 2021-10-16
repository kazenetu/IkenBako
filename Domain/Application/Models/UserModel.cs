using Domain.Domain.Users;

namespace Domain.Application.Models
{
  public class UserModel
  {
    /// <summary>
    /// ユーザーID
    /// </summary>
    /// <remarks>アルファベットで表現すること</remarks>
    public string ID { get; private set; }

    /// <summary>
    /// ハッシュ化済パスワード
    /// </summary>
    public string Password { get; private set; } = string.Empty;

    /// <summary>
    /// ソルト
    /// </summary>
    public string Salt { get; private set; } = string.Empty;

    /// <summary>
    /// 更新バージョン
    /// </summary>
    public int Version{ get; private set; } = 1;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="source">ユーザーインスタンス</param>
    public UserModel(User source)
    {
      ID = source.ID.Value;
      Password = source.Password;
      Salt = source.Salt;
      Version = source.Version;
    }
  }
}
