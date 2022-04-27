﻿using System.Collections.Generic;
using STEP;
using STEP.AST;
using STEP.AST.Nodes;
using Xunit;

namespace StepTests;

public class StandardEnvironmentTests
{
    private readonly TypeVisitor _typeVisitor = new();

    [Fact]
    public void DigitalRead_ValidParameter_ThrowsNoException()
    {
        /*
         * boolean function x()
         *   input digitalpin p = 1
         *   if(digitalRead(p) == HIGH)
         *     return true
         *   else
         *     return false
         * end function
         */
        
        // Arrange
        var retFalse = new RetNode 
        {
            RetVal = new BoolNode {Value = false},
            SurroundingFuncType = new Type {ActualType = TypeVal.Boolean}
        };
        var retTrue = new RetNode
        {
            RetVal = new BoolNode { Value = true},
            SurroundingFuncType = new Type { ActualType = TypeVal.Boolean }
        };
        var digitalRead = new FuncExprNode
        {
            Id = new IdNode {Id = "digitalRead"},
            Params = new List<ExprNode> {new IdNode {Id = "p"}}
        };
        var condition = new EqNode
        {
            Left = digitalRead,
            Right = new IdNode {Id = "HIGH"}
        };
        var ifStmt = new IfNode
        {
            Condition = condition,
            ThenClause = new List<StmtNode> {retTrue},
            ElseClause = new List<StmtNode> {retFalse}
        };
        var pinDcl = new PinDclNode
        {
            Left = new IdNode 
            {
                Id = "p",
                Type = new PinType
                {
                    ActualType = TypeVal.Digitalpin,
                    IsConstant = true,
                    Mode = PinMode.INPUT
                }
            },
            Right = new NumberNode {Value = 1}
        };
        var funcDcl = new FuncDefNode
        {
            Name = new IdNode {Id = "x"},
            Stmts = new List<StmtNode> {pinDcl, ifStmt},
            FormalParams = new List<IdNode>(),
            ReturnType = new Type {ActualType = TypeVal.Boolean}
        };
        
        // Act, assert
        _typeVisitor.Visit(funcDcl);
    }

    [Fact]
    public void DigitalRead_InvalidParameter_ThrowsTypeException()
    {
        // digitalRead(1) -> type exception since 1 is not a digitalpin!

        // Arrange
        var digitalRead = new FuncExprNode
        {
            Id = new IdNode { Id = "digitalRead" },
            Params = new List<ExprNode> { new NumberNode { Value = 1 } }
        };
        
        // Act
        var test = () => _typeVisitor.Visit(digitalRead);

        // Assert
        Assert.Throws<TypeException>(test);
    }
}