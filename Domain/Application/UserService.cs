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
      var user = repository.GetUser(unique_name);
      if(user is null){
        return null;
      }

      return new UserModel(user);
    }

    /// <summary>
    /// ユーザーの保存
    /// </summary>
    /// <param name="target">ユーザーエンティティ</param>
    /// <returns>保存可否</returns>
    public bool Save(User target)
    {
      return repository.Save(target);
    }
  }
}
