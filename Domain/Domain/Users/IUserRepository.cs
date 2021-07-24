namespace Domain.Domain.Users
{
  /// <summary>
  /// ユーザーリポジトリインターフェース
  /// </summary>
  public interface IUserRepository
  {
    /// <summary>
    /// ユーザーを取得
    /// </summary>
    /// <param name="unique_name">ユニーク名</param>
    /// <returns>ユーザー</returns>
    User GetUser(string unique_name);

    /// <summary>
    /// ユーザーの保存
    /// </summary>
    /// <param name="target">ユーザーエンティティ</param>
    /// <returns>保存可否</returns>
    bool Save(User target);
  }
}
