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
    /// <returns>保存可否</returns>
    public bool Save(Message message)
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
      if (db.ExecuteNonQuery(sql.ToString()) == 1)
      {
        return true;
      }

      return false;
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

      // 「すべて」以外の場合は対象を絞り込む
      if(receiverId.Value != ReceiverId.AllReceiverId)
      {
        sql.AppendLine("WHERE ");
        sql.AppendLine("  send_to = @send_to");
        db.ClearParam();
        db.AddParam("@send_to", receiverId.Value);
      }
      sql.AppendLine("ORDER BY id DESC");

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
