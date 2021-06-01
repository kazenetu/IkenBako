using Domain.Domain.OpinionMessages;
using Domain.Domain.Receivers;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Infrastructures
{
  /// <summary>
  /// 意見メッセージリポジトリ
  /// </summary>
  public class MessageRepository: RepositoryBase,IMessageRepository
  {
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public MessageRepository(IOptions<DatabaseConfigModel> config):base(config)
    {
    }

    /// <summary>
    /// 保存
    /// </summary>
    /// <param name="message">意見メッセージクラス</param>
    public void Save(Message message)
    {
      var sql = new StringBuilder();
      sql.AppendLine("INSERT ");
      sql.AppendLine("INTO t_message(send_to, detail) ");
      sql.AppendLine("VALUES (@send_to, @detail) ");

      // Param設定
      db.ClearParam();
      db.AddParam("@send_to", message.SendTo.Value);
      db.AddParam("@detail", message.Detail);

      // SQL発行
      db.ExecuteNonQuery(sql.ToString());
    }

    /// <summary>
    /// 送信対象宛ての意見メッセージ取得
    /// </summary>
    /// <param name="receiverId">送信対象ID</param>
    /// <returns>意見メッセージリスト</returns>
    public List<Message> Find(ReceiverId receiverId)
    {
      var result = new List<Message>();

      var sql = new StringBuilder();
      sql.AppendLine("SELECT");
      sql.AppendLine("  send_to");
      sql.AppendLine("  , detail");
      sql.AppendLine("FROM");
      sql.AppendLine("  t_message");

      var sqlResult = db.Fill(sql.ToString());
      foreach (DataRow row in sqlResult.Rows)
      {
        var sendTo = row["send_to"].ToString();
        var detail = row["detail"].ToString();
        result.Add(Message.Create(sendTo, detail));
      }

      return result;
    }

  }
}
