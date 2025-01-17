﻿using STEP.AST.Nodes;
using STEP.CodeGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using STEP;
using Xunit;
using STEP.AST;
using Type = STEP.AST.Type;

namespace StepTests;

public class CodeGenerationVisitorTests
{
    private readonly CodeGenerationVisitor _visitor = new();

    #region FuncDefNode

    [Fact]
    public void FuncDefNode_WithMultipleParameters_OutputsCorrectCode()
    {
        /* number function x(number a, number b)
         * end function
         * 
         * double x(double a, double b) {
         * }
         */

        // Arrange
        const string expected = "double x(double a, double b) {\r\n}\r\n\r\n";
        var param1 = new IdNode {Name = "a", Type = new Type {ActualType = TypeVal.Number}};
        var param2 = new IdNode {Name = "b", Type = new Type {ActualType = TypeVal.Number}};
        var funcId = new IdNode {Name = "x", Type = new Type {ActualType = TypeVal.Number}};
        var funcDcl = new FuncDefNode
        {
            Id = funcId,
            Stmts = new List<StmtNode>(),
            FormalParams = new List<IdNode> {param1, param2},
            ReturnType = new Type {ActualType = TypeVal.Number}
        };

        // Act
        _visitor.Visit(funcDcl);
        string actual = _visitor.OutputToString();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FuncDefNode_NoParameters_OutputsCorrectCode()
    {
        /* number function x()
         * end function
         * 
         * double x() {
         * }
         */

        // Arrange
        const string expected = "double x() {\r\n}\r\n\r\n";
        var funcId = new IdNode {Name = "x", Type = new Type {ActualType = TypeVal.Number}};
        var funcDcl = new FuncDefNode
        {
            Id = funcId,
            Stmts = new List<StmtNode>(),
            FormalParams = new List<IdNode>(),
            ReturnType = new Type {ActualType = TypeVal.Number}
        };

        // Act
        _visitor.Visit(funcDcl);
        string actual = _visitor.OutputToString();

        // Assert
        Assert.Equal(expected, actual);
    }

    #endregion

    #region String concatenation

    [Fact]
    public void PlusNode_StringConcat_Turns_simple_numeric_expression_into_string()
    {
        // Arrange
        // string x = "This is a string" + 25 + 10 * 2 ->
        // => String x = "This is a string" + String(25 + 10 * 2);\r\n
        const string expected = "String x = \"This is a string\" + String(25 + 10 * 2);\r\n";
        var two = new NumberNode() {Value = 2, Type = new Type() {ActualType = TypeVal.Number}};
        var ten = new NumberNode() {Value = 10, Type = new Type() {ActualType = TypeVal.Number}};
        var multExpr = new MultNode()
        {
            Left = ten,
            Right = two,
            Type = new Type() {ActualType = TypeVal.Number}
        };
        var twentyFive = new NumberNode() {Value = 25};
        var plusExpr = new PlusNode()
        {
            Left = twentyFive,
            Right = multExpr,
            Type = new Type() {ActualType = TypeVal.Number}
        };
        var str = new StringNode()
            {Value = "\"This is a string\"", Type = new Type() {ActualType = TypeVal.String}};
        var concatExpr = new PlusNode()
        {
            Left = str,
            Right = plusExpr,
            Type = new Type() {ActualType = TypeVal.String}
        };
        var id = new IdNode() {Name = "x", Type = new Type() {ActualType = TypeVal.String}};
        var varDcl = new VarDclNode()
        {
            Left = id,
            Right = concatExpr,
            Type = new Type() {ActualType = TypeVal.String, IsConstant = false}
        };
        // Act
        _visitor.Visit(varDcl);
        string actual = _visitor.OutputToString();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void PlusNode_StringConcat_Turns_boolean_into_string()
    {
        // Arrange
        // string x = true + " " + "funk"
        // => String x = String(true) + " " + "funk";\r\n
        const string expected = "String x = String(true) + \" \" + \"funk\";\r\n";

        var trueBool = new BoolNode {Value = true, Type = new Type {ActualType = TypeVal.Boolean}};
        var space = new StringNode {Value = "\" \"", Type = new Type {ActualType = TypeVal.String}};
        var firstConcat = new PlusNode
        {
            Left = trueBool,
            Right = space,
            Type = new Type {ActualType = TypeVal.String}
        };
        var funk = new StringNode {Value = "\"funk\"", Type = new Type {ActualType = TypeVal.String}};
        var secondConcat = new PlusNode
        {
            Left = firstConcat,
            Right = funk,
            Type = new Type {ActualType = TypeVal.String}
        };
        var id = new IdNode {Name = "x", Type = new Type() {ActualType = TypeVal.String}};
        var varDcl = new VarDclNode
        {
            Left = id,
            Right = secondConcat,
            Type = new Type {ActualType = TypeVal.String}
        };

        // Act
        _visitor.Visit(varDcl);
        string actual = _visitor.OutputToString();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void PlusNode_StringConcat_Turns_multiple_numbers_surrounded_by_strings_into_strings()
    {
        // string x = "foo" + 15 + 30 + "bar"
        // => String x = "foo" + String(15) + String(30) + "bar";\r\n
        // Arrange
        const string expected = "String x = \"foo\" + String(15) + String(30) + \"bar\";\r\n";

        var foo = new StringNode {Value = "\"foo\"", Type = new Type {ActualType = TypeVal.String}};
        var num15 = new NumberNode {Value = 15, Type = new Type {ActualType = TypeVal.Number}};
        var firstConcat = new PlusNode
        {
            Left = foo,
            Right = num15,
            Type = new Type {ActualType = TypeVal.String}
        };
        var num30 = new NumberNode {Value = 30, Type = new Type {ActualType = TypeVal.Number}};
        var secondConcat = new PlusNode
        {
            Left = firstConcat,
            Right = num30,
            Type = new Type {ActualType = TypeVal.String}
        };
        var bar = new StringNode {Value = "\"bar\"", Type = new Type {ActualType = TypeVal.String}};
        var thirdConcat = new PlusNode
        {
            Left = secondConcat,
            Right = bar,
            Type = new Type {ActualType = TypeVal.String}
        };
        var id = new IdNode {Name = "x", Type = new Type() {ActualType = TypeVal.String}};
        var varDcl = new VarDclNode
        {
            Left = id,
            Right = thirdConcat,
            Type = new Type {ActualType = TypeVal.String}
        };

        // Act
        _visitor.Visit(varDcl);
        string actual = _visitor.OutputToString();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void PlusNode_StringConcat_Turns_non_string_variables_and_functions_into_strings()
    {
        // string x = GetDay() + ", " + month
        // where GetDay returns a number and month is a number
        // => String x = String(GetDay()) + ", " + String(month);\r\n
        // Arrange
        const string expected = "String x = String(GetDay()) + \", \" + String(month);\r\n";
        var getDayId = new IdNode {Name = "GetDay"};
        var getDay = new FuncExprNode
            {Id = getDayId, Params = new(), Type = new Type {ActualType = TypeVal.Number}};
        var comma = new StringNode {Value = "\", \"", Type = new Type {ActualType = TypeVal.String}};
        var firstConcat = new PlusNode
        {
            Left = getDay,
            Right = comma,
            Type = new Type {ActualType = TypeVal.String}
        };
        var month = new IdNode {Name = "month", Type = new Type {ActualType = TypeVal.Number}};
        var secondConcat = new PlusNode
        {
            Left = firstConcat,
            Right = month,
            Type = new Type {ActualType = TypeVal.String}
        };
        var id = new IdNode {Name = "x", Type = new Type() {ActualType = TypeVal.String}};
        var varDcl = new VarDclNode
        {
            Left = id,
            Right = secondConcat,
            Type = new Type {ActualType = TypeVal.String}
        };

        // Act
        _visitor.Visit(varDcl);
        string actual = _visitor.OutputToString();

        // Assert
        Assert.Equal(expected, actual);
    }

    #endregion


    #region IfNode, ElseIfNode

    [Fact]
    public void IfNode_OnlyThenClause_OutputsOnlyThenClause()
    {
        /* if(true)
         * end if
         *
         * if(true) {
         * }
         */

        // Arrange
        const string expected = "if(true) {\r\n}\r\n";
        var condition = new BoolNode {Value = true, Type = new Type {ActualType = TypeVal.Boolean}};
        var ifStmt = new IfNode {Condition = condition, ThenClause = new List<StmtNode>()};

        // Act
        _visitor.Visit(ifStmt);
        string actual = _visitor.OutputToString();

        // Assert
        Assert.Equal(expected, actual);
    }


    [Fact]
    public void IfNode_ThenAndElseClause_OutputsBothClauses()
    {
        /* if(true)
         *   return a
         * else
         *   return b
         * end if
         *
         * if(true) {
         *   return a;
         * }
         * else {
         *   return b;
         * }
         */

        // Arrange
        const string expected = "if(true) {\r\n    return a;\r\n}\r\nelse {\r\n    return b;\r\n}\r\n";
        var condition = new BoolNode {Value = true, Type = new Type {ActualType = TypeVal.Boolean}};
        var a = new IdNode {Name = "a"};
        var retStmt1 = new RetNode {RetVal = a};
        var b = new IdNode {Name = "b"};
        var retStmt2 = new RetNode {RetVal = b};
        var ifStmt = new IfNode
        {
            Condition = condition,
            ThenClause = new List<StmtNode> {retStmt1},
            ElseClause = new List<StmtNode> {retStmt2}
        };

        // Act
        _visitor.Visit(ifStmt);
        string actual = _visitor.OutputToString();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void IfNode_ThenAndTwoElseIfClauses_OutputsAllThreeClauses()
    {
        /* if(x)
         *   return x
         * else if(y)
         *   return y
         * else if(z)
         *   return z
         * end if
         *
         * if(x) {
         *   return x;
         * }
         * else if(y) {
         *   return y;
         * }
         * else if(z) {
         *   return z;
         * }
         */

        // Arrange
        const string expected =
            "if(x) {\r\n    return x;\r\n}\r\nelse if(y) {\r\n    return y;\r\n}\r\nelse if(z) {\r\n    return z;\r\n}\r\n";
        var x = new IdNode {Name = "x"};
        var retx = new RetNode {RetVal = x};
        var y = new IdNode {Name = "y"};
        var rety = new RetNode {RetVal = y};
        var elseIf1 = new ElseIfNode {Condition = y, Body = new List<StmtNode> {rety}};
        var z = new IdNode {Name = "z"};
        var retz = new RetNode {RetVal = z};
        var elseIf2 = new ElseIfNode {Condition = z, Body = new List<StmtNode> {retz}};
        var ifStmt = new IfNode
        {
            Condition = x,
            ThenClause = new List<StmtNode> {retx},
            ElseIfClauses = new List<ElseIfNode> {elseIf1, elseIf2}
        };

        // Act
        _visitor.Visit(ifStmt);
        string actual = _visitor.OutputToString();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void IfNode_HasAllClauses_OutputsAllClauses()
    {
        /* if(x)
         *   return x
         * else if(y)
         *   return y
         * else if(z)
         *   return z
         * else
         *   return q
         * end if
         *
         * if(x) {
         *   return x;
         * }
         * else if(y) {
         *   return y;
         * }
         * else if(z) {
         *   return z;
         * }
         * else {
         *   return q;
         * }
         */

        // Arrange
        const string expected =
            "if(x) {\r\n    return x;\r\n}\r\nelse if(y) {\r\n    return y;\r\n}\r\nelse if(z) {\r\n    return z;\r\n}\r\nelse {\r\n    return q;\r\n}\r\n";
        var x = new IdNode {Name = "x"};
        var retx = new RetNode {RetVal = x};
        var y = new IdNode {Name = "y"};
        var rety = new RetNode {RetVal = y};
        var elseIf1 = new ElseIfNode {Condition = y, Body = new List<StmtNode> {rety}};
        var z = new IdNode {Name = "z"};
        var retz = new RetNode {RetVal = z};
        var elseIf2 = new ElseIfNode {Condition = z, Body = new List<StmtNode> {retz}};
        var q = new IdNode {Name = "q"};
        var retq = new RetNode {RetVal = q};
        var ifStmt = new IfNode
        {
            Condition = x,
            ThenClause = new List<StmtNode> {retx},
            ElseIfClauses = new List<ElseIfNode> {elseIf1, elseIf2},
            ElseClause = new List<StmtNode> {retq}
        };

        // Act
        _visitor.Visit(ifStmt);
        string actual = _visitor.OutputToString();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void IfNode_EmptyElseIfClause_ShouldNotGenerateCode()
    {
        // This is kind of an optimization - if a clause is empty, there is no point in generating code for it.
        /* if(x)
         *   return x
         * else if(y)
         * end if
         *
         * if(x) {
         *   return x;
         * }
         */

        // Arrange
        const string expected = "if(x) {\r\n    return x;\r\n}\r\n";
        var x = new IdNode {Name = "x"};
        var retx = new RetNode {RetVal = x};
        var y = new IdNode {Name = "y"};
        var elseIf1 = new ElseIfNode {Condition = y, Body = new List<StmtNode>()};
        var ifStmt = new IfNode
        {
            Condition = x,
            ThenClause = new List<StmtNode> {retx},
            ElseIfClauses = new List<ElseIfNode> {elseIf1},
        };

        // Act
        _visitor.Visit(ifStmt);
        string actual = _visitor.OutputToString();

        // Assert
        Assert.Equal(expected, actual);
    }

    #endregion

    #region ForNode

    [Fact]
    public void ForNodeVarDclInitializerIsCreatedCorrectly()
    {
        //Arrange
        const string expected = "for(double i = 0; i <= 10; i = i + 1) {\r\n}\r\n";
        IdNode idNode = new IdNode() {Name = "i", Type = new Type() {ActualType = TypeVal.Number}};
        NumberNode exprNode = new NumberNode() {Value = 0, Type = new Type() {ActualType = TypeVal.Number}};
        VarDclNode varInit = new VarDclNode()
            {Left = idNode, Right = exprNode, Type = new Type() {ActualType = TypeVal.Number}};

        NumberNode limitNode = new NumberNode() {Value = 10, Type = new Type() {ActualType = TypeVal.Number}};
        NumberNode updateNode = new NumberNode() {Value = 1, Type = new Type() {ActualType = TypeVal.Number}};

        ForNode forNode = new ForNode() {Initializer = varInit, Limit = limitNode, Update = updateNode, IsUpTo = true};

        //Act
        _visitor.Visit(forNode);
        string actual = _visitor.OutputToString();

        //Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ForNodeIdAssNodeInitializerIsCreatedCorrectly()
    {
        //Arrange
        const string expected = "for(i = 0; i <= 10; i = i + 1) {\r\n}\r\n";
        IdNode idNode = new IdNode() {Name = "i", Type = new Type() {ActualType = TypeVal.Number}};
        NumberNode exprNode = new NumberNode() {Value = 0, Type = new Type() {ActualType = TypeVal.Number}};
        AssNode assInit = new AssNode() {Id = idNode, Expr = exprNode};

        NumberNode limitNode = new NumberNode() {Value = 10, Type = new Type() {ActualType = TypeVal.Number}};
        NumberNode updateNode = new NumberNode() {Value = 1, Type = new Type() {ActualType = TypeVal.Number}};

        ForNode forNode = new ForNode() {Initializer = assInit, Limit = limitNode, Update = updateNode, IsUpTo = true};

        //Act
        _visitor.Visit(forNode);
        string actual = _visitor.OutputToString();

        //Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ForNodeArrAccAssNodeInitializerIsCreatedCorrectly()
    {
        //Arrange
        const string expected = "for(i[(int) 1] = 0; i[(int) 1] <= 10; i[(int) 1] = i[(int) 1] + 1) {\r\n}\r\n";
        IdNode idNode = new IdNode() {Name = "i", Type = new Type() {ActualType = TypeVal.Number}};
        NumberNode exprNode = new NumberNode() {Value = 0, Type = new Type() {ActualType = TypeVal.Number}};
        NumberNode indexNode = new NumberNode() {Value = 1, Type = new Type() {ActualType = TypeVal.Number}};

        AssNode assInit = new AssNode() {Id = idNode, Expr = exprNode, ArrIndex = indexNode};

        NumberNode limitNode = new NumberNode() {Value = 10, Type = new Type() {ActualType = TypeVal.Number}};
        NumberNode updateNode = new NumberNode() {Value = 1, Type = new Type() {ActualType = TypeVal.Number}};

        ForNode forNode = new ForNode() {Initializer = assInit, Limit = limitNode, Update = updateNode, IsUpTo = true};

        //Act
        _visitor.Visit(forNode);
        string actual = _visitor.OutputToString();

        //Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ForNodeIdInitializerIsCreatedCorrectly()
    {
        //Arrange
        const string expected = "for(i; i <= 10; i = i + 1) {\r\n}\r\n";
        IdNode idNode = new IdNode() {Name = "i", Type = new Type() {ActualType = TypeVal.Number}};
        NumberNode limitNode = new NumberNode() {Value = 10, Type = new Type() {ActualType = TypeVal.Number}};
        NumberNode updateNode = new NumberNode() {Value = 1, Type = new Type() {ActualType = TypeVal.Number}};

        ForNode forNode = new ForNode() {Initializer = idNode, Limit = limitNode, Update = updateNode, IsUpTo = true};

        //Act
        _visitor.Visit(forNode);
        string actual = _visitor.OutputToString();

        //Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ForNodeArrAccInitializerIsCreatedCorrectly()
    {
        //Arrange
        const string expected = "for(i[(int) 1]; i[(int) 1] <= 10; i[(int) 1] = i[(int) 1] + 1) {\r\n}\r\n";
        IdNode idNode = new IdNode() {Name = "i", Type = new Type() {ActualType = TypeVal.Number}};
        NumberNode indexNode = new NumberNode() {Value = 1, Type = new Type() {ActualType = TypeVal.Number}};

        ArrayAccessNode arraccInit = new ArrayAccessNode() {Array = idNode, Index = indexNode};

        NumberNode limitNode = new NumberNode() {Value = 10, Type = new Type() {ActualType = TypeVal.Number}};
        NumberNode updateNode = new NumberNode() {Value = 1, Type = new Type() {ActualType = TypeVal.Number}};

        ForNode forNode = new ForNode() {Initializer = arraccInit, Limit = limitNode, Update = updateNode, IsUpTo = true};

        //Act
        _visitor.Visit(forNode);
        string actual = _visitor.OutputToString();

        //Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ForNodeBodyIsCreatedCorrectly()
    {
        //Arrange
        const string expected = "for(i; i <= 10; i = i + 1) {\r\n    break;\r\n}\r\n";
        IdNode idNode = new IdNode() {Name = "i", Type = new Type() {ActualType = TypeVal.Number}};
        NumberNode limitNode = new NumberNode() {Value = 10, Type = new Type() {ActualType = TypeVal.Number}};
        NumberNode updateNode = new NumberNode() {Value = 1, Type = new Type() {ActualType = TypeVal.Number}};
        BreakNode breakNode = new BreakNode();

        ForNode forNode = new ForNode()
        {
            Initializer = idNode, Limit = limitNode, Update = updateNode,
            Body = new List<StmtNode>() {breakNode},
            IsUpTo = true
        };

        //Act
        _visitor.Visit(forNode);
        string actual = _visitor.OutputToString();

        //Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(1, DurationNode.DurationScale.Ms)]
    [InlineData(1000, DurationNode.DurationScale.S)]
    [InlineData(60000, DurationNode.DurationScale.M)]
    [InlineData(3600000, DurationNode.DurationScale.H)]
    [InlineData(86400000, DurationNode.DurationScale.D)]
    public void WaitFuncCall_CorrectConversion(double duration, DurationNode.DurationScale scale) {
        // Arrange
        string expected = $"delay({duration});{Environment.NewLine}";
        FuncStmtNode funcNode = new FuncStmtNode() {
            Id = new IdNode() {Name = "delay"}, // Std env visitor changes "Wait" to "delay"
            Params = new List<ExprNode>() {
                new DurationNode() {
                    Value = 1,
                    Scale = scale
                }
            }
        };
        
        // Act
        _visitor.Visit(funcNode);
        string actual = _visitor.OutputToString();
        
        // Assert
        Assert.Equal(expected, actual);
    }

    #endregion

    #region PinDclNode

    [Theory]
    [InlineData("OUTPUT", PinMode.OUTPUT)]
    [InlineData("INPUT", PinMode.INPUT)]
    public void PinDclNode_InputOutputAnalogpin_ShouldGeneratepinMode(string expectedMode, PinMode givenPinMode)
    {
        // Arrange
        string expected =
            $"#define x 1\r\n// Global variables\r\n\r\nvoid setup() {{\r\n    Serial.begin(9600);\r\n    pinMode(1, {expectedMode});\r\n}}\r\nvoid loop() {{\r\n}}\r\n";
        var n = new NumberNode() {Value = 1};
        var x = new IdNode {Name = "x"};
        var pinDclNode = new PinDclNode() {Left = x, Right = n, Type = new PinType() {Mode = givenPinMode}};
        var setup = new SetupNode() {Stmts = new List<StmtNode>()};
        var vars = new VarsNode() {Dcls = new List<VarDclNode>() {pinDclNode}};
        var prog = new ProgNode() {SetupBlock = setup, VarsBlock = vars};

        // Act
        _visitor.Visit(prog);
        string actual = _visitor.OutputToString();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("pin1", 1)]
    [InlineData("LEDPIN", 5)]
    public void PinDclNode_DeclarePinVariableName_ShouldDefinePinConstantVariable(string variableName, int pinNumber)
    {
        // Arrange
        string expected =
            $"#define {variableName} {pinNumber}\r\n// Global variables\r\n\r\nvoid setup() {{\r\n    Serial.begin(9600);\r\n    pinMode({pinNumber}, INPUT);\r\n}}\r\nvoid loop() {{\r\n}}\r\n";
        var n = new NumberNode() {Value = pinNumber};
        var x = new IdNode {Name = variableName};
        var pinDclNode = new PinDclNode() {Left = x, Right = n, Type = new PinType() {Mode = PinMode.INPUT}};
        var setup = new SetupNode() {Stmts = new List<StmtNode>()};
        var vars = new VarsNode() {Dcls = new List<VarDclNode>() {pinDclNode}};
        var prog = new ProgNode() {SetupBlock = setup, VarsBlock = vars};

        // Act
        _visitor.Visit(prog);
        string actual = _visitor.OutputToString();

        // Assert
        Assert.Equal(expected, actual);
    }

    #endregion
}