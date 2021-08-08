namespace Domain.Domain.Users
{
  /// <summary>
  /// ユーザー
  /// </summary>
  public class User
  {
    /// <summary>
    /// 送信対象ID
    /// </summary>
    /// <remarks>アルファベットで表現すること</remarks>
    public UserId ID { get; private set; }

    /// <summary>
    /// パスワード
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
    /// <param name="id">送信対象ID</param>
    /// <param name="password">パスワード</param>
    /// <param name="salt">ソルト</param>
    /// <param name="version">更新バージョン</param>
    private User(UserId id, string password, string salt, int version)
    {
      ID = id;
      Password = password;
      Salt = salt;
      Version = version;
    }

    /// <summary>
    /// ユーザーインスタンスの作成
    /// </summary>
    /// <param name="id">送信対象ID</param>
    /// <param name="password">パスワード</param>
    /// <param name="salt">ソルト</param>
    /// <param name="version">更新バージョン</param>
    public static User Create(string id, string password, string salt, int version = 1)
    {
      return new User(new UserId(id), password, salt, version);
    }
  }
}
