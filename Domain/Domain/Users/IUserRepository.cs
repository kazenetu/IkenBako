using System.Collections.Generic;

namespace Domain.Domain.Users
{
  /// <summary>
  /// ユーザーリポジトリインターフェース
  /// </summary>
  public interface IUserRepository: IRepositoryBase
  {
    /// <summary>
    /// ユーザーを取得
    /// </summary>
    /// <param name="unique_name">ユニーク名</param>
    /// <returns>ユーザー</returns>
    User GetUser(string unique_name);

    /// <summary>
    /// ユーザーリストを取得
    /// </summary>
    /// <returns>ユーザーリスト</returns>
    List<User> GetUsers();

    /// <summary>
    /// ユーザーの保存
    /// </summary>
    /// <param name="target">ユーザーエンティティ</param>
    /// <returns>保存可否</returns>
    bool Save(User target);

    /// <summary>
    /// パスワードチェック
    /// </summary>
    /// <param name="unique_name">ユーザーID</param>
    /// <param name="password">パスワード</param>
    /// <returns>パスワード一致か否か</returns>
    bool EqalsPassword(string unique_name, string password);

    /// <summary>
    /// パスワード作成
    /// </summary>
    /// <param name="password">パスワード</param>
    /// <returns>暗号化済パスワードとソルトのタプル</returns>
    (string encryptedPassword, string salt) CreateEncryptedPassword(string password);
  }
}
