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
    /// 送信元表示
    /// </summary>
    /// <remarks>送信先に表示するか</remarks>
    public bool DisplayList { get; private set; } = true;

    /// <summary>
    /// 管理者権限
    /// </summary>
    /// <remarks>一覧ですべてを選択できるか</remarks>
    public bool IsAdminRole { get; private set; } = false;

    /// <summary>
    /// 一覧確認権限
    /// </summary>
    /// <remarks>一覧の表示権限があるか</remarks>
    public bool IsViewListRole { get; set; } = true;

    /// <summary>
    /// 更新バージョン
    /// </summary>
    public int Version{ get; private set; } = 1;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="source">送信対象インスタンス</param>
    public ReceiverModel(Receiver source)
    {
      DisplayName = source.DisplayName;
      ID = source.ID.Value;
      DisplayList = source.DisplayList;
      IsAdminRole = source.IsAdminRole;
      IsViewListRole = source.IsViewListRole;
      Version = source.Version;
    }
  }
}
