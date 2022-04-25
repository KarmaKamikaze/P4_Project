﻿namespace STEP.AST.Nodes;

public class NodesList : AstNode
{
    public List<AstNode> Nodes { get; set; } = new();
    public override void Accept(IVisitor v)
    {
        throw new NotImplementedException();
    }
}