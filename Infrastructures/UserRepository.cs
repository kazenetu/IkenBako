using Domain.Domain.Users;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Options;
using System.Data;
using System.Text;
using System;
using System.Security.Cryptography;

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

        var disabled = false;
        if (!bool.TryParse(row["disabled"].ToString(), out disabled))
        {
          if (int.TryParse(row["disabled"].ToString(), out var disabledValue))
          {
            disabled = disabledValue == 1;
          }
        }

        result = User.Create(id, password, salt, version, disabled);
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

    /// <summary>
    /// パスワードチェック
    /// </summary>
    /// <param name="unique_name">ユーザーID</param>
    /// <param name="password">パスワード</param>
    /// <returns>パスワード一致か否か</returns>
    public bool EqalsPassword(string unique_name, string password)
    {
      var target = GetUser(unique_name);
      if (target is null || target.Disabled) return false;

      var salt = Convert.FromBase64String(target.Salt);

      string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
        password: password,
        salt: salt,
        prf: KeyDerivationPrf.HMACSHA1,
        iterationCount: 10000,
        numBytesRequested: 256 / 8));

      return hashed == target.Password;
    }

    /// <summary>
    /// パスワード作成
    /// </summary>
    /// <param name="password">パスワード</param>
    /// <returns>暗号化済パスワードとソルトのタプル</returns>
    public (string encryptedPassword, string salt) CreateEncryptedPassword(string password)
    {
      // 128ビットのソルトを生成する
      byte[] salt = new byte[128 / 8];
      using (var rng = RandomNumberGenerator.Create())
      {
        rng.GetBytes(salt);
      }
      var saltBase64 = Convert.ToBase64String(salt);

      // 256ビットのサブキーを導出
      string encryptedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
          password: password,
          salt: salt,
          prf: KeyDerivationPrf.HMACSHA1,
          iterationCount: 10000,
          numBytesRequested: 256 / 8));

      // 暗号化済パスワードとソルトを返す
      return (encryptedPassword, saltBase64);
    }
  }
}
