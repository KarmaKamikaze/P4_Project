﻿namespace STEP.AST.Nodes;

public class PlusNode : ExprNode
{
    public override void Accept(IVisitor v)
    {
        v.Visit(this);
    }

    public override bool Equals(object obj)
    {
        if (obj is PlusNode other)
        {
            return Equals(other.Left, Left) && Equals(other.Right, Right);
        }

        return false;
    }
}