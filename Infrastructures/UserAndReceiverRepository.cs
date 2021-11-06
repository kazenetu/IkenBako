using Domain.Domain.Receivers;
using Domain.Domain.UserAndReceivers;
using Domain.Domain.Users;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Infrastructures
{
  /// <summary>
  /// ユーザーと受信者の集約Entityインターフェース
  /// </summary>
  public class UserAndReceiverRepository : RepositoryBase, IUserAndReceiverRepository
  {
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public UserAndReceiverRepository(IOptions<DatabaseConfigModel> config) : base(config)
    {
    }

    #region 取得

    /// <summary>
    /// ユーザーを取得
    /// </summary>
    /// <param name="userId">ユーザーID</param>
    /// <returns>ユーザー</returns>
    public UserAndReceiver GetUserAndReceiver(UserId userId)
    {
      return Find(userId).FirstOrDefault();
    }

    /// <summary>
    /// ユーザーリストを取得
    /// </summary>
    /// <returns>ユーザーリスト</returns>
    public List<UserAndReceiver> GetUserAndReceivers()
    {
      return Find(null);
    }

    /// <summary>
    /// ユーザーリストを取得：メイン部分
    /// </summary>
    /// <param name="userId">ユーザーID</param>
    /// <returns>ユーザーリスト</returns>
    /// <remarks>メイン処理</remarks>
    private List<UserAndReceiver> Find(UserId userId)
    {
      var result = new List<UserAndReceiver>();

      // パラメータ初期化
      db.ClearParam();

      var sql = new StringBuilder();

      sql.AppendLine("SELECT");
      sql.AppendLine(" m_user.unique_name");
      sql.AppendLine(" ,m_user.password");
      sql.AppendLine(" ,m_user.salt");
      sql.AppendLine(" ,m_user.version u_version");
      sql.AppendLine(" ,m_receiver.fullname");
      sql.AppendLine(" ,m_receiver.display_list");
      sql.AppendLine(" ,m_receiver.is_admin_role");
      sql.AppendLine(" ,m_receiver.is_viewlist_role");
      sql.AppendLine(" ,m_receiver.version r_version");
      sql.AppendLine("FROM");
      sql.AppendLine("  m_user");
      sql.AppendLine("LEFT OUTER JOIN  m_receiver ");
      sql.AppendLine("  ON m_receiver.unique_name = m_user.unique_name");

      // ユーザ指定
      if(userId != null)
      {
        sql.AppendLine("WHERE");
        sql.AppendLine("  m_user.unique_name = @unique_name");

        // Param設定
        db.AddParam("@unique_name", userId.Value);
      }
      else
      {
        sql.AppendLine("ORDER BY m_user.unique_name");
      }

      var sqlResult = db.Fill(sql.ToString());
      foreach (DataRow row in sqlResult.Rows)
      {
        // ユーザー
        var id = row["unique_name"].ToString();
        var password = row["password"].ToString();
        var salt = row["salt"].ToString();
        var version = int.Parse(row["u_version"].ToString());
        var user = User.Create(id, password, salt, version);

        // 受信者
        Receiver receiver = null;
        if (row["r_version"] != DBNull.Value)
        {
          var name = row["fullname"].ToString();
          var displayList = false;
          var isAdminRole = false;
          var isViewlistRole = false;
          var r_version = int.Parse(row["r_version"].ToString());

          if (!bool.TryParse(row["display_list"].ToString(), out displayList))
          {
            if (int.TryParse(row["display_list"].ToString(), out var displayListValue))
            {
              displayList = displayListValue == 1;
            }
          }

          if (!bool.TryParse(row["is_admin_role"].ToString(), out isAdminRole))
          {
            if (int.TryParse(row["is_admin_role"].ToString(), out var isAdminRoleValue))
            {
              isAdminRole = isAdminRoleValue == 1;
            }
          }

          if (!bool.TryParse(row["is_viewlist_role"].ToString(), out isViewlistRole))
          {
            if (int.TryParse(row["is_viewlist_role"].ToString(), out var isViewlistRoleValue))
            {
              isViewlistRole = isViewlistRoleValue == 1;
            }
          }

          receiver = Receiver.Create(name, id, displayList, isAdminRole, isViewlistRole, r_version);
        }

        // 集約エンティティ作成
        result.Add(UserAndReceiver.Create(user, receiver));
      }
      return result;
    }

    #endregion

    /// <summary>
    /// ユーザーの保存
    /// </summary>
    /// <param name="target">ユーザーと受信者の集約Entity</param>
    /// <returns>保存成否</returns>
    public bool Save(UserAndReceiver target)
    {
      // DBから最新のユーザーマスタを取得
      var dbUserAndReceiver = GetUserAndReceiver(target.ID);

      // ユーザーマスタの保存
      if (!UserSave(dbUserAndReceiver))
      {
        return false;
      }

      // 受信者マスタの保存・削除
      Receiver receiver = null;
      if(dbUserAndReceiver != null)
      {
        receiver = dbUserAndReceiver.UserReceiver;
      }
      if (target.UserReceiver is null && receiver != null)
      {
        // 受信者マスタを廃止する場合、受信者マスタの削除を行う
        if (!RemoveReceiver(target.ID))
        {
          return false;
        }
      }
      else
      {
        // 受信マスタを更新・元々使用しない場合、受信者マスタの登録・更新を行う
        if (!UserReceiver(receiver))
        {
          return false;
        }
      }

      // 保存成功
      return true;

      // ユーザーマスタの保存
      bool UserSave(UserAndReceiver dbUser)
      {
        // 共通Param設定
        db.ClearParam();
        db.AddParam("@password", target.Password);
        db.AddParam("@salt", target.Salt);
        db.AddParam("@unique_name", target.ID.Value);

        if (dbUser is null)
        {
          // 新規登録時にパスワードやソルトが存在しない場合はエラーとする
          if(string.IsNullOrEmpty(target.Password) || string.IsNullOrEmpty(target.Salt))
          {
            return false;
          }

          // 更新対象がいない場合は登録
          var insrtSQL = new StringBuilder();
          insrtSQL.AppendLine("INSERT into m_user(unique_name, password, salt)");
          insrtSQL.AppendLine("VALUES(@unique_name, @password, @salt)");

          // SQL発行
          if (db.ExecuteNonQuery(insrtSQL.ToString()) == 1)
          {
            return true;
          }
        }
        else if (dbUser is null || dbUser.Version != target.Version)
        {
          return false;
        }

        // 更新
        var updateSQL = new StringBuilder();
        updateSQL.AppendLine("Update m_user");
        updateSQL.AppendLine("SET");
        updateSQL.AppendLine("password = @password,");
        updateSQL.AppendLine("salt = @salt,");
        updateSQL.AppendLine("version = version+1");
        updateSQL.AppendLine("WHERE");
        updateSQL.AppendLine("  unique_name = @unique_name");

        // SQL発行
        if (db.ExecuteNonQuery(updateSQL.ToString()) == 1)
        {
          return true;
        }

        return false;
      }

      // 受信者マスタの保存
      bool UserReceiver(Receiver dbReceiver)
      {
        // 受信者マスタを作成しない場合はtrueを返して終了
        if(target.UserReceiver is null)
        {
          return true;
        }

        // 共通Param設定
        db.ClearParam();
        db.AddParam("@unique_name", target.ID.Value);
        db.AddParam("@fullname", target.UserReceiver.DisplayName);
        db.AddParam("@display_list", target.UserReceiver.DisplayList);
        db.AddParam("@is_admin_role", target.UserReceiver.IsAdminRole);
        db.AddParam("@is_viewlist_role", target.UserReceiver.IsViewListRole);

        if (dbReceiver is null)
        {
          // 更新対象がいない場合は登録
          var insrtSQL = new StringBuilder();
          insrtSQL.AppendLine("INSERT INTO m_receiver(unique_name, fullname, display_list, is_admin_role, is_viewlist_role)");
          insrtSQL.AppendLine("VALUES(@unique_name, @fullname, @display_list, @is_admin_role, @is_viewlist_role)");

          // SQL発行
          if (db.ExecuteNonQuery(insrtSQL.ToString()) == 1)
          {
            return true;
          }
        }
        else if (dbReceiver is null || dbReceiver.Version != target.UserReceiver.Version)
        {
          return false;
        }

        // 更新
        var updateSQL = new StringBuilder();
        updateSQL.AppendLine("Update m_receiver");
        updateSQL.AppendLine("SET");
        updateSQL.AppendLine("fullname = @fullname,");
        updateSQL.AppendLine("display_list = @display_list,");
        updateSQL.AppendLine("is_admin_role = @is_admin_role,");
        updateSQL.AppendLine("is_viewlist_role = @is_viewlist_role,");
        updateSQL.AppendLine("version = version+1");
        updateSQL.AppendLine("WHERE");
        updateSQL.AppendLine("  unique_name = @unique_name");

        // SQL発行
        if (db.ExecuteNonQuery(updateSQL.ToString()) == 1)
        {
          return true;
        }

        return false;
      }
    }

    #region 削除処理

    /// <summary>
    /// ユーザーの削除
    /// </summary>
    /// <param name="userId">ユーザーID</param>
    /// <returns>削除成否</returns>
    public bool Remove(UserId userId)
    {
      // 削除対象レコード追加
      var target = GetUserAndReceiver(userId);

      // 受信者が存在する場合は削除する
      if (target.UserReceiver != null)
      {
        // 削除
        if (!RemoveReceiver(target.ID))
        {
          return false;
        }
      }

      // ユーザーの削除
      var deleteSQL = new StringBuilder();

      // Param設定
      db.ClearParam();
      db.AddParam("@unique_name", target.ID.Value);

      // 削除
      deleteSQL.Clear();
      deleteSQL.AppendLine("DELETE FROM m_user");
      deleteSQL.AppendLine("WHERE");
      deleteSQL.AppendLine("  unique_name = @unique_name");

      // SQL発行
      if (db.ExecuteNonQuery(deleteSQL.ToString()) != 1)
      {
        return false;
      }

      return true;
    }

    /// <summary>
    /// 受信者の削除
    /// </summary>
    /// <param name="userId">ユーザーID</param>
    /// <returns>削除成否</returns>
    private bool RemoveReceiver(UserId userId)
    {
      var deleteSQL = new StringBuilder();

      // Param設定
      db.ClearParam();
      db.AddParam("@unique_name", userId.Value);

      // 削除
      deleteSQL.Clear();
      deleteSQL.AppendLine("DELETE FROM m_receiver");
      deleteSQL.AppendLine("WHERE");
      deleteSQL.AppendLine("  unique_name = @unique_name");

      // SQL発行
      if (db.ExecuteNonQuery(deleteSQL.ToString()) != 1)
      {
        return false;
      }

      return true;
    }

    #endregion
  }
}
