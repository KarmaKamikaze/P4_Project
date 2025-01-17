﻿namespace STEP.AST.Nodes;

public class FuncDefNode : AstNode
{
    public IdNode Id { get; set; }
    public Type ReturnType { get; init; } = new();
    public List<IdNode> FormalParams { get; init; } = new();
    public List<StmtNode> Stmts { get; set; }

    public override void Accept(IVisitor v)
    {
        v.Visit(this);
    }

    public override bool Equals(object obj)
    {
        if (obj is FuncDefNode other)
        {
            return Equals(other.Id, Id)
                   && Equals(other.ReturnType, ReturnType)
                   && FormalParams.SequenceEqual(other.FormalParams)
                   && Stmts.SequenceEqual(other.Stmts);
        }

        return base.Equals(obj);
    }
}