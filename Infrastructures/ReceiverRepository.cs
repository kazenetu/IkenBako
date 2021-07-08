using Domain.Domain.Receivers;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Infrastructures
{
  /// <summary>
  /// 送信対象者リポジトリ
  /// </summary>
  public class ReceiverRepository: RepositoryBase,IReceiverRepository
  {
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public ReceiverRepository(IOptions<DatabaseConfigModel> config):base(config)
    {
    }

    /// <summary>
    /// 送信対象者リストを取得
    /// </summary>
    /// <returns>送信対象者メッセージリスト</returns>
    public List<Receiver> GetReceivers()
    {
      var result = new List<Receiver>();

      var sql = new StringBuilder();
      sql.AppendLine("SELECT");
      sql.AppendLine("  unique_name");
      sql.AppendLine("  , fullname");
      sql.AppendLine("FROM");
      sql.AppendLine("  m_receiver");

      var sqlResult = db.Fill(sql.ToString());
      foreach(DataRow row in sqlResult.Rows)
      {
        var id = row["unique_name"].ToString();
        var name = row["fullname"].ToString();
        result.Add(Receiver.Create(name, id));
      }

      return result;
    }

    /// <summary>
    /// 送信担当者を取得
    /// </summary>
    /// <param name="unique_name">ユニーク名</param>
    /// <returns>送信担当者</returns>
    public Receiver GetReceiver(string unique_name)
    {
      Receiver result = null;

      var sql = new StringBuilder();
      sql.AppendLine("SELECT");
      sql.AppendLine(" * ");
      sql.AppendLine("FROM");
      sql.AppendLine("  m_receiver");
      sql.AppendLine("WHERE");
      sql.AppendLine("  unique_name = @unique_name");

      // Param設定
      db.ClearParam();
      db.AddParam("@unique_name", unique_name);

      var sqlResult = db.Fill(sql.ToString());
      foreach (DataRow row in sqlResult.Rows)
      {
        var id = row["unique_name"].ToString();
        var name = row["fullname"].ToString();

        result = Receiver.Create(name, id);
        break;
      }

      return result;
    }

  }
}
