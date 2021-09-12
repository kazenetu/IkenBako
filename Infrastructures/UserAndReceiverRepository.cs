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

          receiver = Receiver.Create(name, id, displayList, isAdminRole, version);
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
      // TODO 更新 OR 新規作成
      return false;
    }

    /// <summary>
    /// ユーザーの削除
    /// </summary>
    /// <param name="userId">ユーザーID</param>
    /// <returns>削除成否</returns>
    public bool Remove(UserId userId)
    {
      // 削除対象レコード追加
      var target = GetUserAndReceiver(userId);

      // 削除
      var deleteSQL = new StringBuilder();

      // 共通Param設定
      db.ClearParam();
      db.AddParam("@unique_name", target.ID.Value);

      // 受信者が存在する場合は削除する
      if (target.UserReceiver != null)
      {
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
      }

      // ユーザーの削除
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

  }
}
