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
        result = User.Create(id, password, salt);
        break;
      }

      return result;
    }

  }
}
