namespace Domain.Domain.Users
{
  /// <summary>
  /// ユーザーEntity
  /// </summary>
  public class User
  {
    /// <summary>
    /// ユーザーID
    /// </summary>
    /// <remarks>アルファベットで表現すること</remarks>
    public UserId ID { get; private set; }

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
    /// <param name="id">ユーザーID</param>
    /// <param name="password">パスワード</param>
    /// <param name="salt">ソルト</param>
    /// <param name="disabled">無効</param>
    /// <param name="version">更新バージョン</param>
    protected User(UserId id, string password, string salt, bool disabled, int version)
    {
      ID = id;
      Password = password;
      Salt = salt;
      Disabled = disabled;
      Version = version;
    }

    /// <summary>
    /// ユーザーインスタンスの作成
    /// </summary>
    /// <param name="id">ユーザーID</param>
    /// <param name="password">パスワード</param>
    /// <param name="salt">ソルト</param>
    /// <param name="version">更新バージョン</param>
    /// <param name="disabled">無効</param>
    public static User Create(string id, string password, string salt, int version = 1, bool disabled = false)
    {
      return new User(new UserId(id), password, salt, disabled, version);
    }
  }
}
