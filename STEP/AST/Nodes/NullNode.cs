﻿namespace STEP.AST.Nodes;

public class NullNode : AstNode
{
    // This is a null node to represent absence in the AST.
    public override void Accept(IVisitor v) {
        v.Visit(this);
    }
}