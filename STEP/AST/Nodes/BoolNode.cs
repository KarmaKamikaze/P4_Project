﻿namespace STEP.AST.Nodes;

public class BoolNode : ExprNode
{
    public bool Value { get; set; }

    public override void Accept(IVisitor v)
    {
        v.Visit(this);
    }

    public override bool Equals(object obj)
    {
        return (obj as BoolNode)?.Value == Value;
    }
}