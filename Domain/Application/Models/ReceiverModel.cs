using Domain.Domain.Receivers;

namespace Domain.Application.Models
{
  public class ReceiverModel
  {
    /// <summary>
    /// 送信表示象名称
    /// </summary>
    public string DisplayName { get; private set; }

    /// <summary>
    /// 送信対象ID
    /// </summary>
    /// <remarks>アルファベットで表現すること</remarks>
    public string ID { get; private set; }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="source">送信対象インスタンス</param>
    public ReceiverModel(Receiver source)
    {
      DisplayName = source.DisplayName;
      ID = source.ID;
    }
  }
}
