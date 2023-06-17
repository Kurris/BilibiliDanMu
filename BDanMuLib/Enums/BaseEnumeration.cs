using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LiveCore.Enums;

public abstract class BaseEnumeration : IComparable
{
    public string Name { get; }

    public int Id { get; }

    protected BaseEnumeration(int id, string name) => (Id, Name) = (id, name);

    public override string ToString() => Name;

    public static IEnumerable<T> GetAll<T>() where T : BaseEnumeration =>
        typeof(T).GetFields(BindingFlags.Public |
                            BindingFlags.Static |
                            BindingFlags.DeclaredOnly)
            .Select(f => f.GetValue(null))
            .Cast<T>();

    public override bool Equals(object obj)
    {
        if (obj is not BaseEnumeration otherValue)
        {
            return false;
        }

        var typeMatches = GetType() == obj.GetType();
        var valueMatches = Id.Equals(otherValue.Id);

        return typeMatches && valueMatches;
    }

    public int CompareTo(object other) => Id.CompareTo(((BaseEnumeration)other).Id);

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}