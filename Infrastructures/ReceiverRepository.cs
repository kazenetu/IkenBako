using Domain.Domain.Receivers;
using System;
using System.Collections.Generic;
using System.IO;

namespace Infrastructures
{
  /// <summary>
  /// 送信対象者リポジトリ
  /// </summary>
  public class ReceiverRepository: IReceiverRepository
  {
    /// <summary>
    /// 送信者ディレクトリ名
    /// </summary>
    private const string ReceiverDirectoryName = "Receivers";

    /// <summary>
    /// ファイルの起点パス
    /// </summary>
    private string CurrentPath;

    /// <summary>
    /// 対象ファイル名
    /// </summary>
    private List<string> files = new List<string>();

    /// <summary>
    /// 送信者リスト
    /// </summary>
    private List<Receiver> receivers = new List<Receiver>();

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public ReceiverRepository()
    {
      CurrentPath = Path.Join(Directory.GetCurrentDirectory(), ReceiverDirectoryName);

      CheckAndLoadFiles();
    }

    /// <summary>
    /// 送信対象者リストを取得
    /// </summary>
    /// <returns>送信対象者メッセージリスト</returns>
    public List<Receiver> GetReceivers()
    {
      CheckAndLoadFiles();
      return new List<Receiver>(receivers);
    }


    /// <summary>
    /// 送信対象者ディレクトリのチェックと読み出し
    /// </summary>
    private void CheckAndLoadFiles()
    {
      // 対象ファイルを取得
      var targetFiles = Directory.GetFiles(CurrentPath, "*.txt");
      foreach (var filePath in targetFiles)
      {
        // 既存確認
        if (files.Contains(filePath))
        {
          continue;
        }

        // ファイルの存在確認
        if (!File.Exists(filePath))
        {
          throw new Exception($"送信対象者情報「{filePath}」が存在しません。");
        }

        var receiverId = (new FileInfo(filePath)).Name.Replace(".txt", string.Empty);
        var displayName = File.ReadAllText(filePath);
        receivers.Add(Receiver.Create(displayName, receiverId));

        files.Add(filePath);
      }

    }
  }
}
