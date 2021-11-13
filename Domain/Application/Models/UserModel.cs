using Domain.Domain.Users;

namespace Domain.Application.Models
{
  /// <summary>
  /// ユーザーモデル
  /// </summary>
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
    /// 無効
    /// </summary>
    /// <remarks>trueの場合は無効</remarks>
    public bool Disabled { get; private set; } = false;

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
      Disabled = source.Disabled;
      Version = source.Version;
    }
  }
}
