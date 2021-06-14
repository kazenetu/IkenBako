using Domain.Application.Models;
using Domain.Domain.Users;

namespace Domain.Application
{
  /// <summary>
  /// ユーザー
  /// </summary>
  public class UserService
  {
    private readonly IUserRepository repository;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="repository">ユーザーリポジトリ</param>
    public UserService(IUserRepository repository)
    {
      this.repository = repository;
    }

    /// <summary>
    /// ユーザーを取得
    /// </summary>
    /// <param name="unique_name">ユニーク名</param>
    /// <returns>ユーザー</returns>
    public UserModel GetUser(string unique_name)
    {
      return new UserModel(repository.GetUser(unique_name));
    }
  }
}
