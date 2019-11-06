using System.Collections.Generic;

namespace IkenBako.Models
{
  /// <summary>
  /// 送信対象理リスト
  /// </summary>
  public class SendTargets
  {
    /// <summary>
    /// 送信対象者リスト
    /// </summary>
    public List<SendTarget> Targets { get; private set; }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    private SendTargets()
    {
      Targets = new List<SendTarget>();
    }

    /// <summary>
    /// 送信対象リストインスタンスの作成
    /// </summary>
    /// <returns>送信対象リストインスタンス</returns>
    private static SendTargets Create()
    {
      var result = new SendTargets();
      result.Targets.Add(SendTarget.Create("aa"));
      result.Targets.Add(SendTarget.Create("bb"));
      result.Targets.Add(SendTarget.Create("cc"));

      return result;
    }

    /// <summary>
    /// インスタンス取得
    /// </summary>
    /// <returns></returns>
    public static SendTargets GetInstance()
    {
      return Create();
    }
  }
}
