using System;
using Unity.Entities;

public class NameComponent : IComponentData, IEquatable<NameComponent>
{
    public string Value = string.Empty;

    public bool Equals(NameComponent other)
    {
        return Value == other.Value;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}