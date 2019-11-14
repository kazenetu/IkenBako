using System;

namespace Domain.Domain.Receivers
{
  public class ReceiverId : IEquatable<ReceiverId>
  {
    public string Value { get; }

    public ReceiverId(string id)
    {
      if (string.IsNullOrEmpty(id))
      {
        throw new ArgumentNullException("IDがありません。", nameof(id));
      }

      Value = id;
    }

    public bool Equals(ReceiverId other)
    {
      if (ReferenceEquals(null, other)) return false;
      if (ReferenceEquals(this, other)) return true;
      return string.Equals(Value, other.Value);
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      if (obj.GetType() != this.GetType()) return false;
      return Equals((ReceiverId)obj);
    }

    public override int GetHashCode()
    {
      return (Value != null ? Value.GetHashCode() : 0);
    }
  }
}
