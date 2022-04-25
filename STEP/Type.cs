﻿using STEP.AST.Nodes;

namespace STEP;

public class Type : IEquatable<Type>
{
    public TypeVal ActualType { get; set; }
    public Boolean IsArray { get; set; }
    public bool Equals(Type other) {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return ActualType == other.ActualType && IsArray == other.IsArray;
    }
    public static bool operator ==(Type a, Type b) => a.Equals(b);
    public static bool operator !=(Type a, Type b) => !a.Equals(b);

    public override string ToString() {
        return $"{ActualType}{(IsArray ? " array" : "")}";
    }
}