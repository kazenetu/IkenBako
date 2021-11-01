using Domain.Domain.Receivers;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Infrastructures
{
  /// <summary>
  /// 受信者リポジトリ
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
    /// 受信者リストを取得
    /// </summary>
    /// <returns>受信者メッセージリスト</returns>
    public List<Receiver> GetReceivers()
    {
      var result = new List<Receiver>();

      var sql = new StringBuilder();
      sql.AppendLine("SELECT");
      sql.AppendLine("  unique_name");
      sql.AppendLine("  , fullname");
      sql.AppendLine("FROM");
      sql.AppendLine("  m_receiver");
      sql.AppendLine("WHERE ");
      sql.AppendLine("  display_list = true");
      sql.AppendLine("ORDER BY unique_name");

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
    /// 受信者を取得
    /// </summary>
    /// <param name="unique_name">ユニーク名</param>
    /// <returns>受信者</returns>
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
        var displayList = false;
        var isAdminRole = false;
        var isViewlistRole = false;
        var version = int.Parse(row["version"].ToString());

        if(!bool.TryParse(row["display_list"].ToString(), out displayList))
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

        result = Receiver.Create(name, id, displayList, isAdminRole, isViewlistRole, version);

        break;
      }

      return result;
    }

  }
}
