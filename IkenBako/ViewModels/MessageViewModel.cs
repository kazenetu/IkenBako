namespace IkenBako.ViewModels
{
  /// <summary>
  /// メッセージ用ViewModel
  /// </summary>
  public class MessageViewModel
  {
    /// <summary>
    /// メッセージ対象
    /// </summary>
    public string SendTo { get; set; }

    /// <summary>
    /// メッセージ対象名称
    /// </summary>
    public string SendTargetName { get; set; }

    /// <summary>
    /// メッセージ本文
    /// </summary>
    public string Detail { get; set; }

  }
}
