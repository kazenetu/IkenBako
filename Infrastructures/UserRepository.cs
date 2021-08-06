using Domain.Domain.Users;
using Microsoft.Extensions.Options;
using System.Data;
using System.Text;

namespace Infrastructures
{
  /// <summary>
  /// ユーザーリポジトリ
  /// </summary>
  public class UserRepository : RepositoryBase, IUserRepository
  {
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public UserRepository(IOptions<DatabaseConfigModel> config) : base(config)
    {
    }

    /// <summary>
    /// ユーザーを取得
    /// </summary>
    /// <param name="unique_name">ユニーク名</param>
    /// <returns>ユーザー</returns>
    public User GetUser(string unique_name)
    {
      User result = null;

      var sql = new StringBuilder();
      sql.AppendLine("SELECT");
      sql.AppendLine(" * ");
      sql.AppendLine("FROM");
      sql.AppendLine("  m_user");
      sql.AppendLine("WHERE");
      sql.AppendLine("  unique_name = @unique_name");

      // Param設定
      db.ClearParam();
      db.AddParam("@unique_name", unique_name);

      var sqlResult = db.Fill(sql.ToString());
      foreach (DataRow row in sqlResult.Rows)
      {
        var id = row["unique_name"].ToString();
        var password = row["password"].ToString();
        var salt = row["salt"].ToString();
        var version = int.Parse(row["version"].ToString());
        result = User.Create(id, password, salt, version);
        break;
      }

      return result;
    }

    /// <summary>
    /// ユーザーの保存
    /// </summary>
    /// <param name="target">ユーザーエンティティ</param>
    /// <returns>保存可否</returns>
    public bool Save(User target)
    {
      // DBから最新のユーザーマスタを取得
      var dbUser = GetUser(target.ID.Value);

      // 共通Param設定
      db.ClearParam();
      db.AddParam("@password", target.Password);
      db.AddParam("@salt", target.Salt);
      db.AddParam("@unique_name", target.ID.Value);

      if(dbUser is null){
        // 更新対象がいない場合は登録
        var insrtSQL = new StringBuilder();
        insrtSQL.AppendLine("INSERT into m_user(unique_name, password, salt)");
        insrtSQL.AppendLine("VALUE(@unique_name, @password, @salt)");

        // SQL発行
        if(db.ExecuteNonQuery(insrtSQL.ToString()) == 1){
          return true;
        }
      }
      else if(dbUser.Version != target.Version)
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
      if(db.ExecuteNonQuery(updateSQL.ToString()) == 1){
        return true;
      }

      return false;
    }

  }
}
