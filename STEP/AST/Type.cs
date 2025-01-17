﻿using STEP.AST.Nodes;

namespace STEP.AST;

public class Type : IEquatable<Type>
{
    public TypeVal ActualType { get; set; }
    public bool IsArray { get; set; }
    public int ArrSize { get; set; }
    public bool IsConstant { get; set; }
    public bool IsReturned { get; set; }
    public int ScopeLevel { get; set; }

    public bool Equals(Type other)
    {
        return Equals(ActualType, other.ActualType)
               && Equals(IsArray, other.IsArray);
    }

    public static bool operator ==(Type a, Type b) => a.Equals(b);
    public static bool operator !=(Type a, Type b) => !a.Equals(b);

    public override string ToString()
    {
        return $"{ActualType}{(IsArray ? " array" : "")}";
    }
}