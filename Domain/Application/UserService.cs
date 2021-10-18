using Domain.Application.Models;
using Domain.Domain.Receivers;
using Domain.Domain.UserAndReceivers;
using Domain.Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Application
{
  /// <summary>
  /// ユーザー用サービス
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
    private bool Save(User target)
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
    /// ユーザー情報の保存
    /// </summary>
    /// <param name="ID">ユーザーID</param>
    /// <param name="userVersion">取得時のユーザーマスタのバージョン</param>
    /// <param name="useReceiver">受信者マスタ利用か否か</param>
    /// <param name="displayName">受信者名</param>
    /// <param name="displayList">送信元表示</param>
    /// <param name="isAdminRole">管理者権限</param>
    /// <param name="isViewlistRole">一覧確認権限</param>
    /// <param name="receiverVersion">取得時の受信マスタのバージョン</param>
    /// <param name="newPassword">再設定のパスワード(平文)</param>
    /// <returns>登録成功・失敗</returns>
    /// <remarks>newPasswordがnullの場合はパスワード変更を行わない</remarks>
    public bool Save(string ID, int userVersion,
                     bool useReceiver, string displayName, bool displayList, bool isAdminRole, bool isViewlistRole, int receiverVersion,
                     string newPassword)
    {
      try
      {
        var result = false;

        // トランザクション開始
        userAndReceiverRepository.BeginTransaction();

        // パスワードの取得または再設定
        var password = string.Empty;
        var salt = string.Empty;
        if (string.IsNullOrEmpty(newPassword))
        {
          var user = userAndReceiverRepository.GetUserAndReceiver(new UserId(ID));
          password = user.Password;
          salt = user.Salt;
        }
        else
        {
          // 新しいパスワードで暗号化済みパスワードとソルトを取得
          var encryptedPasswordAndSalt = repository.CreateEncryptedPassword(newPassword);
          password = encryptedPasswordAndSalt.encryptedPassword;
          salt = encryptedPasswordAndSalt.salt;
        }

        // 登録するエンティティを生成
        var targetUser = User.Create(ID, password, salt, userVersion);
        Receiver targetReceiver = null;
        if (useReceiver)
        {
          targetReceiver = Receiver.Create(displayName, ID, displayList, isAdminRole, isViewlistRole, receiverVersion);
        }

        // DB更新
        result = userAndReceiverRepository.Save(UserAndReceiver.Create(targetUser, targetReceiver));

        // 更新結果を受けてCommit/Rollback
        if (result)
        {
          userAndReceiverRepository.Commit();
        }
        else
        {
          userAndReceiverRepository.Rollback();
        }

        return result;
      }
      catch (Exception ex)
      {
        userAndReceiverRepository.Rollback();
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

    #region ユーザー削除

    /// <summary>
    /// ユーザー削除
    /// </summary>
    /// <param name="userId">ユーザーID</param>
    /// <returns>削除成否</returns>
    public bool Remove(UserId userId)
    {
      try
      {
        var result = false;

        // トランザクション開始
        repository.BeginTransaction();

        result = userAndReceiverRepository.Remove(userId);

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
    /// ユーザー一括削除
    /// </summary>
    /// <param name="userId">ユーザーID</param>
    /// <returns>削除成否</returns>
    public bool RemoveAll(List<UserId> userIds)
    {
      try
      {
        var result = true;

        // トランザクション開始
        userAndReceiverRepository.BeginTransaction();

        foreach (var userId in userIds)
        {
          result = userAndReceiverRepository.Remove(userId);

          // 更新失敗の場合は終了
          if (!result)
            break;
        }

        // 更新結果を受けてCommit/Rollback
        if (result)
        {
          userAndReceiverRepository.Commit();
        }
        else
        {
          userAndReceiverRepository.Rollback();
        }

        return result;
      }
      catch (Exception ex)
      {
        userAndReceiverRepository.Rollback();
        throw ex;
      }
    }
    #endregion
  }
}
