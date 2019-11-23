using Domain.Domain.OpinionMessages;
using Domain.Domain.Receivers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;

namespace Infrastructures
{
  /// <summary>
  /// 意見メッセージリポジトリ
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
        // 各要素をDictionaryに格納
        var target = new Dictionary<string, string>();
        var properties = message.GetType().GetProperties();
        foreach (var p in properties)
        {
          target.Add(p.Name, p.GetValue(message) as string);
        }

        // Dictionaryをシリアライズ
        using (MemoryStream ms = new MemoryStream())
        {
          var jsonSerializer = new DataContractJsonSerializer(typeof(Dictionary<string, string>));
          jsonSerializer.WriteObject(ms, target);
          return ms.ToArray();
        }
      }

    }

    /// <summary>
    /// 送信対象宛ての意見メッセージ取得
    /// </summary>
    /// <param name="receiverId">送信対象ID</param>
    /// <returns>意見メッセージリスト</returns>
    public List<Message> FindMessage(ReceiverId receiverId)
    {
      var result = new List<Message>();

      // 対象ディレクトリパスを設定
      var targetPath = Path.Join(CurrentPath, receiverId.Value);

      // 対象ディレクトリパスの存在確認
      if (!Directory.Exists(targetPath))
      {
        return result;
      }

      // 対象ディレクトリからJSONファイルを取得する
      var targetFiles = Directory.GetFiles(targetPath, "*.txt");
      foreach (var filePath in targetFiles)
      {
        result.Add(ConvertJsonToMessage(filePath));
      }

      return result;
    }

    /// <summary>
    /// JsonファイルをMessageクラスインスタンスに変換
    /// </summary>
    /// <param name="filePath">JSONファイルのフルパス</param>
    /// <returns>意見メッセージインスタンス</returns>
    private Message ConvertJsonToMessage(string filePath)
    {
      using (var stream = new FileStream(filePath, FileMode.Open))
      {
        // Dictionaryにデシリアライズ
        var jsonSerializer = new DataContractJsonSerializer(typeof(Dictionary<string, string>));
        var messageModel = jsonSerializer.ReadObject(stream) as Dictionary<string, string>;

        // 意見メッセージインスタンスを生成する
        return Message.Create(messageModel["SendTo"], messageModel["Detail"]);
      }

    }
  }
}
