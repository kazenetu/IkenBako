using IkenBako.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;

namespace IkenBako.Infrastructures
{
  /// <summary>
  /// 意見メッセージリポジトリクラス
  /// </summary>
  public class MessageRepository: IMessageRepository
  {
    /// <summary>
    /// 投稿結果ディレクトリ名
    /// </summary>
    private const string SendResultDirectoryName = "SendResult";

    /// <summary>
    /// ファイル作成の起点パス
    /// </summary>
    private string CurrentPath;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public MessageRepository()
    {
      CurrentPath = Path.Join(Directory.GetCurrentDirectory(), SendResultDirectoryName);

      // 起点パスが存在しない場合はディレクトリを作成する
      if (!Directory.Exists(CurrentPath))
      {
        Directory.CreateDirectory(CurrentPath);
      }
    }

    /// <summary>
    /// 破棄
    /// </summary>
    public void Dispose()
    {
    }

    /// <summary>
    /// 保存
    /// </summary>
    /// <param name="message">意見メッセージクラス</param>
    public void Save(Message message)
    {
      // 格納ディレクトリパスを作成
      var targetPath = Path.Join(CurrentPath, message.SendTo);

      // 格納ディレクトリパスが存在しない場合はディレクトリを作成する
      if (!Directory.Exists(targetPath))
      {
        Directory.CreateDirectory(targetPath);
      }

      // 意見メッセージクラスインスタンスをjsonに変換
      var jsonBytes = ConvartJsonBytes();

      // ファイルパスを作成
      var filePath = Path.Join(targetPath, $"{DateTime.Now.ToString("yyyyMMddHHmmss")}.txt");

      // ファイルを作成
      using (FileStream stream = new FileStream(filePath, FileMode.Create))
      {
        stream.Write(jsonBytes, 0, jsonBytes.Length);
      }

      // Json変換メソッド
      Byte[] ConvartJsonBytes()
      {
        using (MemoryStream ms = new MemoryStream())
        {
          var jsonSerializer = new DataContractJsonSerializer(typeof(Message));
          jsonSerializer.WriteObject(ms, message);
          return ms.ToArray();
        }
      }

    }

    /// <summary>
    /// 送信対象宛ての意見メッセージ取得
    /// </summary>
    /// <param name="sendTarget">送信対象者クラス</param>
    /// <returns>意見メッセージリスト</returns>
    public List<Message> FindMessage(SendTarget sendTarget)
    {
      var result = new List<Message>();

      // 対象ディレクトリパスを設定
      var targetPath = Path.Join(CurrentPath, sendTarget.ID);

      // 対象ディレクトリパスの存在確認
      if (!Directory.Exists(targetPath))
      {
        return result;
      }

      // 対象ディレクトリからJsonファイルを取得する
      var targetFiles =  Directory.GetFiles(targetPath, "*.txt");
      foreach(var filePath in targetFiles)
      {
        result.Add(ConvertJsonToMessage(filePath));
      }

      return result;

      // JsonファイルをMessageクラスインスタンスに変換する
      Message ConvertJsonToMessage(string filePath)
      {
        using (var stream = new FileStream(filePath, FileMode.Open))
        {
          var jsonSerializer = new DataContractJsonSerializer(typeof(Message));
          return jsonSerializer.ReadObject(stream) as Message;
        }

      }
    }
  }
}
