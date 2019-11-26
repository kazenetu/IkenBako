﻿using System;

namespace Domain.Domain.Receivers
{
  /// <summary>
  /// 送信対象ID
  /// </summary>
  public class ReceiverId : IEquatable<ReceiverId>
  {
    /// <summary>
    /// 送信対象ID
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="id">送信対象ID</param>
    public ReceiverId(string id)
    {
      if (string.IsNullOrEmpty(id))
      {
        throw new ArgumentNullException("IDがありません。", nameof(id));
      }

      Value = id;
    }

    /// <summary>
    /// 比較
    /// </summary>
    /// <param name="other">比較対象</param>
    /// <returns>比較結果</returns>
    public bool Equals(ReceiverId other)
    {
      if (ReferenceEquals(null, other)) return false;
      if (ReferenceEquals(this, other)) return true;
      return string.Equals(Value, other.Value);
    }

    /// <summary>
    /// 比較
    /// </summary>
    /// <param name="obj">比較対象</param>
    /// <returns>比較結果</returns>
    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      if (obj.GetType() != this.GetType()) return false;
      return Equals((ReceiverId)obj);
    }

    /// <summary>
    /// ハッシュ値取得
    /// </summary>
    /// <returns>ハッシュ値</returns>
    public override int GetHashCode()
    {
      return (Value != null ? Value.GetHashCode() : 0);
    }
  }
}
