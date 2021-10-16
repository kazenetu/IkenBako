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
    /// 更新バージョン
    /// </summary>
    public int Version{ get; private set; } = 1;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="id">ユーザーID</param>
    /// <param name="password">ハッシュ化済パスワード</param>
    /// <param name="salt">ソルト</param>
    /// <param name="version">更新バージョン</param>
    protected User(UserId id, string password, string salt, int version)
    {
      ID = id;
      Password = password;
      Salt = salt;
      Version = version;
    }

    /// <summary>
    /// ユーザーインスタンスの作成
    /// </summary>
    /// <param name="id">ユーザーID</param>
    /// <param name="password">ハッシュ化済パスワード</param>
    /// <param name="salt">ソルト</param>
    /// <param name="version">更新バージョン</param>
    public static User Create(string id, string password, string salt, int version = 1)
    {
      return new User(new UserId(id), password, salt, version);
    }
  }
}
