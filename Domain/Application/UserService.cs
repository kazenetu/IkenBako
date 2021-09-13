using Domain.Application.Models;
using Domain.Domain.UserAndReceivers;
using Domain.Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Application
{
  /// <summary>
  /// ユーザー
  /// </summary>
  public class UserService
  {
    private readonly IUserRepository repository;
    private readonly IUserAndReceiverRepository userAndReceiverRepository;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="repository">ユーザーリポジトリ</param>
    /// <param name="userAndReceiverRepository">ユーザーと受信者の集約Entityインターフェース</param>
    public UserService(IUserRepository repository, IUserAndReceiverRepository userAndReceiverRepository)
    {
      this.repository = repository;
      this.userAndReceiverRepository = userAndReceiverRepository;
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
    /// ユーザーと受信者の集約リストを取得
    /// </summary>
    /// <returns>ユーザーと受信者の集約リスト</returns>
    public List<UserAndReceiverModel> GetList()
    {
      var userAndReceiverModels = userAndReceiverRepository.GetUserAndReceivers().Select(user =>
      {
        return new UserAndReceiverModel(user, user.UserReceiver);
      });

      return userAndReceiverModels.ToList();
    }

    /// <summary>
    /// ユーザーの保存
    /// </summary>
    /// <param name="target">ユーザーエンティティ</param>
    /// <returns>保存可否</returns>
    public bool Save(User target)
    {
      try
      {
        var result = false;

        // トランザクション開始
        repository.BeginTransaction();
        
        // DB更新
        result = repository.Save(target);

        // 更新結果を受けてCommit/Rollback
        if (result)
        {
          repository.Commit();
        }
        else
        {
          repository.Rollback();
        }

        return result;
      }
      catch (Exception ex)
      {
        repository.Rollback();
        throw ex;
      }
    }

    /// <summary>
    /// パスワードチェック
    /// </summary>
    /// <param name="unique_name">ID</param>
    /// <param name="password">パスワード</param>
    /// <returns></returns>
    public bool EqalsPassword(string unique_name, string password)
    {
      // 暗号化してDBの暗号化済パスワードと一致確認
      return repository.EqalsPassword(unique_name, password);
    }

    /// <summary>
    /// パスワード変更
    /// </summary>
    /// <param name="unique_name">ユーザーID</param>
    /// <param name="password">新パスワード</param>
    /// <param name="version">更新バージョン</param>
    /// <returns>更新可否</returns>
    public bool ChangePassword(string unique_name, string password, int version)
    {
      // 暗号化済みパスワード、ソルトの生成
      var encryptedPasswordAndSalt = repository.CreateEncryptedPassword(password);

      // DB更新
      return Save(User.Create(unique_name, encryptedPasswordAndSalt.encryptedPassword, encryptedPasswordAndSalt.salt, version));
    }
  }
}
