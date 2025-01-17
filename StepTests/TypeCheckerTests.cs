﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.InteropServices;
using STEP.AST;
using STEP.AST.Nodes;
using Xunit;
using Moq;
using STEP.Exceptions;
using STEP.Symbols;
using STEP.SemanticAnalysis;
using Type = STEP.AST.Type;

namespace StepTests;

public class TypeCheckerTests
{
    private readonly IVisitor _typeVisitor;
    private readonly Mock<ISymbolTable> _symbolTableMock = new Mock<ISymbolTable>();

    public TypeCheckerTests()
    {
        _typeVisitor = new TypeVisitor(_symbolTableMock.Object);
    }

    #region Expressions

    [Fact]
    public void ExprNode_IdNotDeclared_ThrowsSymbolNotDeclaredException()
    {
        // Arrange
        var plusNode = new PlusNode()
        {
            Left = new IdNode() {Name = "lhs"},
            Right = new NumberNode() {Value = 5}
        };

        // Act
        var test = () => plusNode.Accept(_typeVisitor);

        // Assert
        Assert.Throws<SymbolNotDeclaredException>(test);
    }

    [Fact]
    public void NumberNode_ShouldBeNumber()
    {
        // Arrange
        var numLiteralNode = new NumberNode();

        // Act
        _typeVisitor.Visit(numLiteralNode);

        // Assert
        Assert.Equal(TypeVal.Number, numLiteralNode.Type.ActualType);
    }

    [Fact]
    public void StringNode_ShouldBeString()
    {
        // Arrange
        var strLiteralNode = new StringNode();

        // Act
        _typeVisitor.Visit(strLiteralNode);

        // Assert
        Assert.Equal(TypeVal.String, strLiteralNode.Type.ActualType);
    }

    [Fact]
    public void BoolNode_ShouldBeBool()
    {
        // Arrange
        var boolLiteralNode = new BoolNode();

        // Act
        _typeVisitor.Visit(boolLiteralNode);

        // Assert
        Assert.Equal(TypeVal.Boolean, boolLiteralNode.Type.ActualType);
    }

    [Fact]
    public void IdNode_GetsCorrectTypeFromDeclaration()
    {
        // Arrange
        const string id = "x";

        // Intercept symbol table entry request, save locally
        SymTableEntry symbolTableEntry = new();
        _symbolTableMock.Setup(x => x.EnterSymbol(It.IsAny<IdNode>()))
            .Callback<IdNode>((id) => symbolTableEntry = new SymTableEntry() {Name = id.Name, Type = id.Type});

        var dclNode = new VarDclNode()
        {
            Left = new IdNode() {Name = id, Type = new Type() {ActualType = TypeVal.Number}},
            Right = new NumberNode() {Value = 50}
        };
        var idNode = new IdNode() {Name = id};

        // Act
        _typeVisitor.Visit(dclNode);
        // Intercept symbol table receive request, use local
        _symbolTableMock.Setup(x => x.RetrieveSymbol(id))
            .Returns(symbolTableEntry);
        _typeVisitor.Visit(idNode);

        // Assert
        Assert.Equal(dclNode.Right.Type.ActualType, idNode.Type.ActualType);
    }

    [Fact]
    public void ParenNode_MaintainsInnerType()
    {
        // Arrange
        var exprNode = new NumberNode() {Value = 420};
        var parenNode = new ParenNode() {Left = exprNode};

        // Act
        _typeVisitor.Visit(parenNode);

        // Assert
        Assert.Equal(exprNode.Type.ActualType, parenNode.Type.ActualType);
    }

    [Fact]
    public void TypeEquals_SameProperties_IsTrue()
    {
        // Arrange
        Type type1 = new Type()
        {
            ActualType = TypeVal.Number,
            IsArray = false
        };
        Type type2 = new Type()
        {
            ActualType = TypeVal.Number,
            IsArray = false
        };

        // Act
        var test = type1 == type2;

        // Assert
        Assert.True(test);
    }

    [Fact]
    public void TypeEquals_OneIsArray_IsFalse()
    {
        // Arrange
        var type1 = new Type()
        {
            ActualType = TypeVal.Number,
            IsArray = false
        };
        var type2 = new Type()
        {
            ActualType = TypeVal.Number,
            IsArray = true
        };

        // Act
        var test = type1 == type2;

        // Assert
        Assert.False(test);
    }

    [Fact]
    public void UMinusNode_LeftIsNumber_IsTypeNumber()
    {
        // Arrange
        var exprNode = new NumberNode() {Value = 69420};
        var uMinusNode = new UMinusNode() {Left = exprNode};

        // Act
        uMinusNode.Accept(_typeVisitor);

        // Assert
        Assert.Equal(TypeVal.Number, uMinusNode.Type.ActualType);
    }

    [Theory]
    [InlineData(TypeVal.Boolean)]
    [InlineData(TypeVal.String)]
    [InlineData(TypeVal.Error)]
    public void UMinusNode_LeftIsNotNumber_ThrowsTypeException(TypeVal type)
    {
        // Arrange
        var symbol = new SymTableEntry() {Type = new Type() {ActualType = type}};
        _symbolTableMock.Setup(x => x.RetrieveSymbol(It.IsAny<string>()))
            .Returns(symbol);
        var exprNode = new IdNode() {Name = "val"};
        var uMinusNode = new UMinusNode() {Left = exprNode};

        // Act
        var test = () => uMinusNode.Accept(_typeVisitor);

        // Assert
        Assert.Throws<TypeMismatchException>(test);
    }

    [Fact]
    public void NegNode_ExprIsNotBoolean_ThrowsTypeException()
    {
        // Arrange
        var exprNode = new NumberNode() {Value = 420};
        var negNode = new NegNode() {Left = exprNode};

        // Act
        var test = () => _typeVisitor.Visit(negNode);

        // Assert
        Assert.Throws<TypeMismatchException>(test);
    }

    [Fact]
    public void NegNode_ExprIsBoolean_HasTypeBoolean()
    {
        // Arrange
        var exprNode = new BoolNode() {Value = true};
        var negNode = new NegNode() {Left = exprNode};

        // Act
        _typeVisitor.Visit(negNode);

        // Assert
        Assert.Equal(TypeVal.Boolean, negNode.Type.ActualType);
    }

    [Theory]
    [InlineData(TypeVal.Boolean)]
    [InlineData(TypeVal.String)]
    public void MultNode_InvalidDomain_ThrowsTypeException(TypeVal type)
    {
        // Arrange
        // Set up a symbol table mock which always returns a SymTableEntry with the specified type.
        var symbol = new SymTableEntry() {Type = new Type() {ActualType = type}};
        _symbolTableMock.Setup(x => x.RetrieveSymbol(It.IsAny<string>()))
            .Returns(symbol);
        var multNode = new MultNode()
        {
            Left = new IdNode() {Name = "x"},
            Right = new IdNode() {Name = "y"}
        };

        // Act
        var test = () => _typeVisitor.Visit(multNode);

        // Assert
        Assert.Throws<TypeMismatchException>(test);
    }

    [Fact]
    public void MultNode_BothOperandsAreNumbers_HasTypeNumber()
    {
        // Arrange
        var leftExpr = new NumberNode();
        var rightExpr = new NumberNode();
        var multNode = new MultNode()
        {
            Left = leftExpr,
            Right = rightExpr
        };

        // Act
        _typeVisitor.Visit(multNode);

        // Assert
        Assert.Equal(TypeVal.Number, multNode.Type.ActualType);
    }

    [Theory]
    [InlineData(TypeVal.Boolean)]
    [InlineData(TypeVal.String)]
    public void DivNode_InvalidDomain_ThrowsTypeException(TypeVal type)
    {
        // Arrange
        // Set up a symbol table mock which always returns a SymTableEntry with the specified type.
        var symbol = new SymTableEntry() {Type = new Type() {ActualType = type}};
        _symbolTableMock.Setup(x => x.RetrieveSymbol(It.IsAny<string>()))
            .Returns(symbol);
        var divNode = new DivNode()
        {
            Left = new IdNode() {Name = "x"},
            Right = new IdNode() {Name = "y"}
        };

        // Act
        var test = () => _typeVisitor.Visit(divNode);

        // Assert
        Assert.Throws<TypeMismatchException>(test);
    }

    [Fact]
    public void DivNode_BothOperandsAreNumbers_HasTypeNumber()
    {
        // Arrange
        var leftExpr = new NumberNode();
        var rightExpr = new NumberNode();
        var divNode = new DivNode()
        {
            Left = leftExpr,
            Right = rightExpr
        };

        // Act
        _typeVisitor.Visit(divNode);

        // Assert
        Assert.Equal(TypeVal.Number, divNode.Type.ActualType);
    }

    [Theory]
    [InlineData(TypeVal.Boolean)]
    [InlineData(TypeVal.String)]
    public void PowNode_InvalidDomain_ThrowsTypeException(TypeVal type)
    {
        // Arrange
        // Set up a symbol table mock which always returns a SymTableEntry with the specified type.
        var symbol = new SymTableEntry() {Type = new Type() {ActualType = type}};
        _symbolTableMock.Setup(x => x.RetrieveSymbol(It.IsAny<string>()))
            .Returns(symbol);
        var powNode = new PowNode()
        {
            Left = new IdNode() {Name = "x"},
            Right = new IdNode() {Name = "y"}
        };

        // Act
        var test = () => _typeVisitor.Visit(powNode);

        // Assert
        Assert.Throws<TypeMismatchException>(test);
    }

    [Fact]
    public void PowNode_BothOperandsAreNumbers_HasTypeNumber()
    {
        // Arrange
        var leftExpr = new NumberNode();
        var rightExpr = new NumberNode();
        var powNode = new PowNode()
        {
            Left = leftExpr,
            Right = rightExpr
        };

        // Act
        _typeVisitor.Visit(powNode);

        // Assert
        Assert.Equal(TypeVal.Number, powNode.Type.ActualType);
    }

    [Theory]
    [InlineData(TypeVal.Boolean)]
    [InlineData(TypeVal.String)]
    public void MinusNode_InvalidDomain_ThrowsTypeException(TypeVal type)
    {
        // Arrange
        // Set up a symbol table mock which always returns a SymTableEntry with the specified type.
        var symbol = new SymTableEntry() {Type = new Type() {ActualType = type}};
        _symbolTableMock.Setup(x => x.RetrieveSymbol(It.IsAny<string>()))
            .Returns(symbol);
        var minusNode = new MinusNode()
        {
            Left = new IdNode() {Name = "x"},
            Right = new IdNode() {Name = "y"}
        };

        // Act
        var test = () => _typeVisitor.Visit(minusNode);

        // Assert
        Assert.Throws<TypeMismatchException>(test);
    }

    [Fact]
    public void MinusNode_BothOperandsAreNumbers_HasTypeNumber()
    {
        // Arrange
        var leftExpr = new NumberNode();
        var rightExpr = new NumberNode();
        var minusNode = new MinusNode()
        {
            Left = leftExpr,
            Right = rightExpr
        };

        // Act
        _typeVisitor.Visit(minusNode);

        // Assert
        Assert.Equal(TypeVal.Number, minusNode.Type.ActualType);
    }

    [Fact]
    public void PlusNode_BothOperandsAreNumbers_HasTypeNumber()
    {
        // Arrange
        var leftExpr = new NumberNode();
        var rightExpr = new NumberNode();
        var plusNode = new PlusNode()
        {
            Left = leftExpr,
            Right = rightExpr
        };

        // Act
        _typeVisitor.Visit(plusNode);

        // Assert
        Assert.Equal(TypeVal.Number, plusNode.Type.ActualType);
    }

    [Fact]
    public void AddNode_EitherOperandIsString_HasTypeString()
    {
        // Arrange
        var leftExpr = new StringNode();
        var rightExpr = new NumberNode();
        var plusNode = new PlusNode()
        {
            Left = leftExpr,
            Right = rightExpr
        };

        // Act
        _typeVisitor.Visit(plusNode);

        // Assert
        Assert.Equal(TypeVal.String, plusNode.Type.ActualType);
    }

    [Fact]
    public void AddNode_EitherOperandIsBoolean_ThrowsTypeException()
    {
        // Arrange
        var leftExpr = new NumberNode();
        var rightExpr = new BoolNode();
        var plusNode = new PlusNode()
        {
            Left = leftExpr,
            Right = rightExpr
        };

        // Act
        var test = () => _typeVisitor.Visit(plusNode);

        // Assert
        Assert.Throws<TypeMismatchException>(test);
    }

    //TODO: Function call expression

    [Fact]
    public void AndNode_BothOperandsAreBoolean_HasTypeBoolean()
    {
        // Arrange
        var leftExpr = new BoolNode();
        var rightExpr = new BoolNode();
        var andNode = new AndNode()
        {
            Left = leftExpr,
            Right = rightExpr
        };

        // Act
        _typeVisitor.Visit(andNode);

        // Assert
        Assert.Equal(TypeVal.Boolean, andNode.Type.ActualType);
    }

    [Theory]
    [InlineData(TypeVal.Number)]
    [InlineData(TypeVal.String)]
    public void AndNode_InvalidDomain_ThrowsTypeException(TypeVal type)
    {
        // Arrange
        // Set up a symbol table mock which always returns a SymTableEntry with the specified type.
        var symbol = new SymTableEntry() {Type = new Type() {ActualType = type}};
        _symbolTableMock.Setup(x => x.RetrieveSymbol(It.IsAny<string>()))
            .Returns(symbol);
        var andNode = new AndNode()
        {
            Left = new IdNode() {Name = "x"},
            Right = new IdNode() {Name = "y"}
        };

        // Act
        var test = () => _typeVisitor.Visit(andNode);

        // Assert
        Assert.Throws<TypeMismatchException>(test);
    }

    [Fact]
    public void OrNode_BothOperandsAreBoolean_HasTypeBoolean()
    {
        // Arrange
        var leftExpr = new BoolNode();
        var rightExpr = new BoolNode();
        var orNode = new OrNode()
        {
            Left = leftExpr,
            Right = rightExpr
        };

        // Act
        _typeVisitor.Visit(orNode);

        // Assert
        Assert.Equal(TypeVal.Boolean, orNode.Type.ActualType);
    }

    [Theory]
    [InlineData(TypeVal.Number)]
    [InlineData(TypeVal.String)]
    public void OrNode_InvalidDomain_ThrowsTypeException(TypeVal type)
    {
        // Arrange
        // Set up a symbol table mock which always returns a SymTableEntry with the specified type.
        var symbol = new SymTableEntry() {Type = new Type() {ActualType = type}};
        _symbolTableMock.Setup(x => x.RetrieveSymbol(It.IsAny<string>()))
            .Returns(symbol);
        var orNode = new OrNode()
        {
            Left = new IdNode() {Name = "x"},
            Right = new IdNode() {Name = "y"}
        };

        // Act
        var test = () => _typeVisitor.Visit(orNode);

        // Assert
        Assert.Throws<TypeMismatchException>(test);
    }

    [Theory]
    [InlineData(TypeVal.Number)]
    [InlineData(TypeVal.String)]
    [InlineData(TypeVal.Boolean)]
    public void EqNode_ValidDomain_HasTypeBoolean(TypeVal type)
    {
        // Arrange
        // Set up a symbol table mock which always returns a SymTableEntry with the specified type.
        var symbol = new SymTableEntry() {Type = new Type() {ActualType = type}};
        _symbolTableMock.Setup(x => x.RetrieveSymbol(It.IsAny<string>()))
            .Returns(symbol);
        var eqNode = new EqNode()
        {
            Left = new IdNode() {Name = "x"},
            Right = new IdNode() {Name = "y"}
        };

        // Act
        _typeVisitor.Visit(eqNode);

        // Assert
        Assert.Equal(TypeVal.Boolean, eqNode.Type.ActualType);
    }

    [Fact]
    public void EqNode_OperandsAreNotSameType_ThrowsTypeException()
    {
        // Arrange
        var eqNode = new EqNode()
        {
            Left = new NumberNode(),
            Right = new StringNode()
        };

        // Act
        var test = () => _typeVisitor.Visit(eqNode);

        // Assert
        Assert.Throws<TypeMismatchException>(test);
    }

    [Theory]
    [InlineData(TypeVal.Number)]
    [InlineData(TypeVal.String)]
    [InlineData(TypeVal.Boolean)]
    public void NeqNode_ValidDomain_HasTypeBoolean(TypeVal type)
    {
        // Arrange
        // Set up a symbol table mock which always returns a SymTableEntry with the specified type.
        var symbol = new SymTableEntry() {Type = new Type() {ActualType = type}};
        _symbolTableMock.Setup(x => x.RetrieveSymbol(It.IsAny<string>()))
            .Returns(symbol);
        var neqNode = new NeqNode()
        {
            Left = new IdNode() {Name = "x"},
            Right = new IdNode() {Name = "y"}
        };

        // Act
        _typeVisitor.Visit(neqNode);

        // Assert
        Assert.Equal(TypeVal.Boolean, neqNode.Type.ActualType);
    }

    [Fact]
    public void NeqNode_OperandsAreNotSameType_ThrowsTypeException()
    {
        // Arrange
        var neqNode = new NeqNode()
        {
            Left = new NumberNode(),
            Right = new StringNode()
        };

        // Act
        var test = () => _typeVisitor.Visit(neqNode);

        // Assert
        Assert.Throws<TypeMismatchException>(test);
    }

    [Theory]
    [InlineData(TypeVal.String)]
    [InlineData(TypeVal.Boolean)]
    public void GThanNode_TypeMismatch_ThrowsTypeException(TypeVal type)
    {
        // Arrange
        var symbol = new SymTableEntry() {Type = new Type() {ActualType = type}};
        _symbolTableMock.Setup(x => x.RetrieveSymbol(It.IsAny<string>()))
            .Returns(symbol);
        var gThanNode = new GThanNode()
        {
            Left = new IdNode() {Name = "left"},
            Right = new IdNode() {Name = "right"}
        };
        // Act
        var test = () => gThanNode.Accept(_typeVisitor);
        // Assert
        Assert.Throws<TypeMismatchException>(test);
    }

    [Fact]
    public void GThanNode_TypeMismatch_IsTypeBoolean()
    {
        // Arrange
        var symbol = new SymTableEntry() {Type = new Type() {ActualType = TypeVal.Number}};
        _symbolTableMock.Setup(x => x.RetrieveSymbol(It.IsAny<string>()))
            .Returns(symbol);
        var gThanNode = new GThanNode()
        {
            Left = new IdNode() {Name = "left"},
            Right = new IdNode() {Name = "right"}
        };
        // Act
        gThanNode.Accept(_typeVisitor);
        // Assert
        Assert.Equal(TypeVal.Boolean, gThanNode.Type.ActualType);
    }

    [Theory]
    [InlineData(TypeVal.String)]
    [InlineData(TypeVal.Boolean)]
    public void GThanEqNode_TypeMismatch_ThrowsTypeException(TypeVal type)
    {
        // Arrange
        var symbol = new SymTableEntry() {Type = new Type() {ActualType = type}};
        _symbolTableMock.Setup(x => x.RetrieveSymbol(It.IsAny<string>()))
            .Returns(symbol);
        var gThanEqNode = new GThanEqNode()
        {
            Left = new IdNode() {Name = "left"},
            Right = new IdNode() {Name = "right"}
        };
        // Act
        var test = () => gThanEqNode.Accept(_typeVisitor);
        // Assert
        Assert.Throws<TypeMismatchException>(test);
    }

    [Fact]
    public void GThanEqNode_TypeMismatch_IsTypeBoolean()
    {
        // Arrange
        var symbol = new SymTableEntry() {Type = new Type() {ActualType = TypeVal.Number}};
        _symbolTableMock.Setup(x => x.RetrieveSymbol(It.IsAny<string>()))
            .Returns(symbol);
        var gThanEqNode = new GThanEqNode()
        {
            Left = new IdNode() {Name = "left"},
            Right = new IdNode() {Name = "right"}
        };
        // Act
        gThanEqNode.Accept(_typeVisitor);
        // Assert
        Assert.Equal(TypeVal.Boolean, gThanEqNode.Type.ActualType);
    }

    [Theory]
    [InlineData(TypeVal.String)]
    [InlineData(TypeVal.Boolean)]
    public void LThanNode_WrongTypes_ThrowsTypeException(TypeVal type)
    {
        // Arrange
        var symbol = new SymTableEntry() {Type = new Type() {ActualType = type}};
        _symbolTableMock.Setup(x => x.RetrieveSymbol(It.IsAny<string>()))
            .Returns(symbol);
        var lThanNode = new LThanNode()
        {
            Left = new IdNode() {Name = "left"},
            Right = new IdNode() {Name = "right"}
        };

        // Act
        var test = () => lThanNode.Accept(_typeVisitor);

        // Assert
        Assert.Throws<TypeMismatchException>(test);
    }

    [Fact]
    public void LThanNode_BothNumbers_IsTypeBoolean()
    {
        // Arrange
        var symbol = new SymTableEntry() {Type = new Type() {ActualType = TypeVal.Number}};
        _symbolTableMock.Setup(x => x.RetrieveSymbol(It.IsAny<string>()))
            .Returns(symbol);
        var lThanNode = new LThanNode()
        {
            Left = new IdNode() {Name = "left"},
            Right = new IdNode() {Name = "right"}
        };

        // Act
        lThanNode.Accept(_typeVisitor);

        // Assert
        Assert.Equal(TypeVal.Boolean, lThanNode.Type.ActualType);
    }

    [Theory]
    [InlineData(TypeVal.String)]
    [InlineData(TypeVal.Boolean)]
    public void LThanEqNode_WrongTypes_ThrowsTypeException(TypeVal type)
    {
        // Arrange
        var symbol = new SymTableEntry() {Type = new Type() {ActualType = type}};
        _symbolTableMock.Setup(x => x.RetrieveSymbol(It.IsAny<string>()))
            .Returns(symbol);
        var lThanEqNode = new LThanEqNode()
        {
            Left = new IdNode() {Name = "left"},
            Right = new IdNode() {Name = "right"}
        };

        // Act
        var test = () => lThanEqNode.Accept(_typeVisitor);

        // Assert
        Assert.Throws<TypeMismatchException>(test);
    }

    [Fact]
    public void LThanEqNode_BothNumbers_IsTypeBoolean()
    {
        // Arrange
        var symbol = new SymTableEntry() {Type = new Type() {ActualType = TypeVal.Number}};
        _symbolTableMock.Setup(x => x.RetrieveSymbol(It.IsAny<string>()))
            .Returns(symbol);
        var lThanEqNode = new LThanEqNode()
        {
            Left = new IdNode() {Name = "left"},
            Right = new IdNode() {Name = "right"}
        };

        // Act
        lThanEqNode.Accept(_typeVisitor);

        // Assert
        Assert.Equal(TypeVal.Boolean, lThanEqNode.Type.ActualType);
    }

    [Theory]
    [InlineData(TypeVal.Number)]
    [InlineData(TypeVal.String)]
    [InlineData(TypeVal.Boolean)]
    public void ArrayAccessNode_IndexIsNumber_AssignsArrayType(TypeVal type)
    {
        // Arrange
        var symbol = new SymTableEntry() {Type = new Type() {ActualType = type}};
        _symbolTableMock.Setup(x => x.RetrieveSymbol("array"))
            .Returns(symbol);
        var symbol2 = new SymTableEntry() {Type = new Type() {ActualType = TypeVal.Number}};
        _symbolTableMock.Setup(x => x.RetrieveSymbol("index"))
            .Returns(symbol2);
        var arrAccNode = new ArrayAccessNode()
        {
            Array = new IdNode() {Name = "array"},
            Index = new IdNode() {Name = "index"}
        };
        // Act
        arrAccNode.Accept(_typeVisitor);

        // Assert
        Assert.Equal(type, arrAccNode.Type.ActualType);
    }

    [Theory]
    [InlineData(TypeVal.String)]
    [InlineData(TypeVal.Boolean)]
    [InlineData(TypeVal.Error)]
    public void ArrayAccessNode_IndexIsNotNumber_ThrowsTypeException(TypeVal type)
    {
        // Arrange
        var symbol = new SymTableEntry() {Type = new Type() {ActualType = TypeVal.Number}};
        _symbolTableMock.Setup(x => x.RetrieveSymbol("array"))
            .Returns(symbol);
        var symbol2 = new SymTableEntry() {Type = new Type() {ActualType = type}};
        _symbolTableMock.Setup(x => x.RetrieveSymbol("index"))
            .Returns(symbol2);
        var arrAccNode = new ArrayAccessNode()
        {
            Array = new IdNode() {Name = "array"},
            Index = new IdNode() {Name = "index"}
        };
        // Act
        var test = () => arrAccNode.Accept(_typeVisitor);

        // Assert
        Assert.Throws<TypeMismatchException>(test);
    }

    [Theory]
    [InlineData(TypeVal.Number)]
    [InlineData(TypeVal.String)]
    [InlineData(TypeVal.Boolean)]
    public void FuncExprNode_ParamsMatch_IsTypeMatch(TypeVal type)
    {
        // Arrange
        var symbol = new FunctionSymTableEntry()
        {
            Type = new Type() {ActualType = type},
            Parameters = new Dictionary<string, Type>()
            {
                {"a", new Type() {ActualType = TypeVal.Number}},
                {"b", new Type() {ActualType = TypeVal.String}},
                {"c", new Type() {ActualType = TypeVal.Boolean}}
            }
        };
        _symbolTableMock.Setup(x => x.RetrieveSymbol("func"))
            .Returns(symbol);
        _symbolTableMock.Setup(x => x.RetrieveSymbol("a"))
            .Returns(new SymTableEntry() {Type = new Type() {ActualType = TypeVal.Number}});
        _symbolTableMock.Setup(x => x.RetrieveSymbol("b"))
            .Returns(new SymTableEntry() {Type = new Type() {ActualType = TypeVal.String}});
        _symbolTableMock.Setup(x => x.RetrieveSymbol("c"))
            .Returns(new SymTableEntry() {Type = new Type() {ActualType = TypeVal.Boolean}});
        var funcExprNode = new FuncExprNode()
        {
            Id = new IdNode() {Name = "func"},
            Params = new List<ExprNode>()
            {
                new IdNode() {Name = "a"},
                new IdNode() {Name = "b"},
                new IdNode() {Name = "c"}
            }
        };

        // Act
        funcExprNode.Accept(_typeVisitor);

        // Assert
        Assert.Equal(type, funcExprNode.Type.ActualType);
    }

    [Theory]
    [InlineData(TypeVal.Number)]
    [InlineData(TypeVal.String)]
    [InlineData(TypeVal.Boolean)]
    public void FuncExprNode_ParamsMismatch_ThrowsTypeException(TypeVal type)
    {
        // Arrange
        var symbol = new FunctionSymTableEntry()
        {
            Type = new Type() {ActualType = type},
            Parameters = new Dictionary<string, Type>()
            {
                {"a", new Type() {ActualType = TypeVal.Number}},
                {"b", new Type() {ActualType = TypeVal.String}},
                {"c", new Type() {ActualType = TypeVal.Boolean}}
            }
        };
        _symbolTableMock.Setup(x => x.RetrieveSymbol("func"))
            .Returns(symbol);
        _symbolTableMock.Setup(x => x.RetrieveSymbol("a"))
            .Returns(new SymTableEntry() {Type = new Type() {ActualType = TypeVal.Number}});
        _symbolTableMock.Setup(x => x.RetrieveSymbol("b"))
            .Returns(new SymTableEntry() {Type = new Type() {ActualType = TypeVal.String}});
        _symbolTableMock.Setup(x => x.RetrieveSymbol("c"))
            .Returns(new SymTableEntry() {Type = new Type() {ActualType = TypeVal.Boolean}});
        var funcExprNode = new FuncExprNode()
        {
            Id = new IdNode() {Name = "func"},
            Params = new List<ExprNode>()
            {
                new IdNode() {Name = "c"},
                new IdNode() {Name = "a"},
                new IdNode() {Name = "b"}
            }
        };

        // Act
        var test = () => funcExprNode.Accept(_typeVisitor);

        // Assert
        Assert.Throws<TypeMismatchException>(test);
    }

    [Fact]
    public void FuncExprNode_IsNotFunc_ThrowsException()
    {
        // Arrange
        var symbol = new SymTableEntry() {Type = new Type() {ActualType = TypeVal.Number}};
        _symbolTableMock.Setup(x => x.RetrieveSymbol("func"))
            .Returns(symbol);
        var funcExprNode = new FuncExprNode()
        {
            Id = new IdNode() {Name = "func"},
        };

        // Act
        var action = () => funcExprNode.Accept(_typeVisitor);

        // Assert
        Assert.Throws<SymbolNotDeclaredException>(action);
    }

    [Fact]
    public void FuncExprNode_ParamsHasPinAndMatches_IsTypeMatch()
    {
        // Arrange
        var symbol = new FunctionSymTableEntry()
        {
            Type = new Type() {ActualType = TypeVal.Number},
            Parameters = new Dictionary<string, Type>()
            {
                {"a", new Type() {ActualType = TypeVal.Analogpin}},
                {"b", new Type() {ActualType = TypeVal.Digitalpin}},
                {"c", new Type() {ActualType = TypeVal.Boolean}}
            }
        };
        _symbolTableMock.Setup(x => x.RetrieveSymbol("func"))
            .Returns(symbol);
        _symbolTableMock.Setup(x => x.RetrieveSymbol("a"))
            .Returns(new SymTableEntry() {Type = new Type() {ActualType = TypeVal.Analogpin}});
        _symbolTableMock.Setup(x => x.RetrieveSymbol("b"))
            .Returns(new SymTableEntry() {Type = new Type() {ActualType = TypeVal.Digitalpin}});
        _symbolTableMock.Setup(x => x.RetrieveSymbol("c"))
            .Returns(new SymTableEntry() {Type = new Type() {ActualType = TypeVal.Boolean}});
        var funcExprNode = new FuncExprNode()
        {
            Id = new IdNode() {Name = "func"},
            Params = new List<ExprNode>()
            {
                new IdNode() {Name = "a"},
                new IdNode() {Name = "b"},
                new IdNode() {Name = "c"}
            }
        };

        // Act
        funcExprNode.Accept(_typeVisitor);

        // Assert
        Assert.Equal(TypeVal.Number, funcExprNode.Type.ActualType);
    }

    [Fact]
    public void FuncExprNode_ParamsHasPinMismatch_ThrowsTypeException()
    {
        // Arrange
        var symbol = new FunctionSymTableEntry()
        {
            Type = new Type() {ActualType = TypeVal.Number},
            Parameters = new Dictionary<string, Type>()
            {
                {"a", new Type() {ActualType = TypeVal.Digitalpin}},
                {"b", new Type() {ActualType = TypeVal.Analogpin}},
                {"c", new Type() {ActualType = TypeVal.Boolean}}
            }
        };
        _symbolTableMock.Setup(x => x.RetrieveSymbol("func"))
            .Returns(symbol);
        _symbolTableMock.Setup(x => x.RetrieveSymbol("a"))
            .Returns(new SymTableEntry() {Type = new Type() {ActualType = TypeVal.Analogpin}});
        _symbolTableMock.Setup(x => x.RetrieveSymbol("b"))
            .Returns(new SymTableEntry() {Type = new Type() {ActualType = TypeVal.Digitalpin}});
        _symbolTableMock.Setup(x => x.RetrieveSymbol("c"))
            .Returns(new SymTableEntry() {Type = new Type() {ActualType = TypeVal.Boolean}});
        var funcExprNode = new FuncExprNode()
        {
            Id = new IdNode() {Name = "func"},
            Params = new List<ExprNode>()
            {
                new IdNode() {Name = "a"},
                new IdNode() {Name = "b"},
                new IdNode() {Name = "c"}
            }
        };

        // Act
        var test = () => funcExprNode.Accept(_typeVisitor);

        // Assert
        Assert.Throws<TypeMismatchException>(test);
    }

    [Theory]
    [InlineData(TypeVal.Number)]
    [InlineData(TypeVal.String)]
    [InlineData(TypeVal.Boolean)]
    public void ArrLiteralNode_TypeMatch_IsCorrectType(TypeVal type)
    {
        // Arrange
        var symbol = new SymTableEntry() {Type = new Type() {ActualType = type}};
        _symbolTableMock.Setup(x => x.RetrieveSymbol(It.IsAny<string>()))
            .Returns(symbol);
        var arrLiteralNode = new ArrLiteralNode()
        {
            Type = new Type() {ActualType = type, IsArray = true},
            Elements = new List<ExprNode>()
            {
                new IdNode() {Name = "a"},
                new IdNode() {Name = "b"}
            },
            ExpectedSize = 2
        };

        // Act
        arrLiteralNode.Accept(_typeVisitor);

        // Assert
        Assert.Equal(type, arrLiteralNode.Type.ActualType);
    }

    [Fact]
    public void ArrLiteralNode_TypeMismatch_ThrowsTypeException()
    {
        // Arrange
        var arrLiteralNode = new ArrLiteralNode()
        {
            Elements = new List<ExprNode>()
            {
                new NumberNode() {Value = 5},
                new BoolNode() {Value = false}
            },
            ExpectedSize = 2
        };

        // Act
        var test = () => arrLiteralNode.Accept(_typeVisitor);

        // Assert
        Assert.Throws<TypeMismatchException>(test);
    }

    #endregion

    #region Declarations

    [Theory]
    [InlineData(TypeVal.Number)]
    [InlineData(TypeVal.String)]
    [InlineData(TypeVal.Boolean)]
    public void VarDclNode_TypeMatch_IsTypeOk(TypeVal type)
    {
        //Arrange
        var symbol = new SymTableEntry() {Type = new Type() {ActualType = type}};
        _symbolTableMock.Setup(x => x.RetrieveSymbol("right"))
            .Returns(symbol);
        var varDclNode = new VarDclNode
        {
            Left = new IdNode()
            {
                Name = "left",
                Type = new Type() {ActualType = type}
            },
            Right = new IdNode() {Name = "right"}
        };

        //Act
        varDclNode.Accept(_typeVisitor);

        //Assert
        Assert.Equal(TypeVal.Ok, varDclNode.Type.ActualType);
    }

    [Theory]
    [InlineData(TypeVal.Number, TypeVal.Boolean)]
    [InlineData(TypeVal.String, TypeVal.Number)]
    [InlineData(TypeVal.Boolean, TypeVal.String)]
    public void VarDclNode_TypeMismatch_ThrowsTypeException(TypeVal type1, TypeVal type2)
    {
        //Arrange
        var symbol = new SymTableEntry() {Type = new Type() {ActualType = type2}};
        _symbolTableMock.Setup(x => x.RetrieveSymbol("right"))
            .Returns(symbol);
        var varDclNode = new VarDclNode
        {
            Left = new IdNode()
            {
                Name = "left",
                Type = new Type() {ActualType = type1}
            },
            Right = new IdNode() {Name = "right"}
        };

        //Act
        var test = () => varDclNode.Accept(_typeVisitor);

        //Assert
        Assert.Throws<TypeMismatchException>(test);
    }

    [Theory]
    [InlineData(TypeVal.Number)]
    [InlineData(TypeVal.Boolean)]
    [InlineData(TypeVal.String)]
    public void VarDclNode_ExprHasError_ThrowsTypeException(TypeVal type)
    {
        //Arrange
        var symbol2 = new SymTableEntry() {Type = new Type() {ActualType = TypeVal.Error}};
        _symbolTableMock.Setup(x => x.RetrieveSymbol("right"))
            .Returns(symbol2);
        var varDclNode = new VarDclNode
        {
            Left = new IdNode()
            {
                Name = "left",
                Type = new Type() {ActualType = type}
            },
            Right = new IdNode() {Name = "right"}
        };

        //Act
        var test = () => varDclNode.Accept(_typeVisitor);

        //Assert
        Assert.Throws<TypeMismatchException>(test);
    }

    [Fact]
    public void ArrDclNode_TypeMatch_IsTypeOk()
    {
        // Arrange
        var symbol = new SymTableEntry() {Type = new Type() {ActualType = TypeVal.Number, IsArray = true}};
        _symbolTableMock.Setup(x => x.RetrieveSymbol("right"))
            .Returns(symbol);
        var arrDclNode = new ArrDclNode()
        {
            Left = new IdNode() {Name = "left", Type = new Type() {ActualType = TypeVal.Number, IsArray = true}},
            Right = new ArrLiteralNode()
                {Elements = new List<ExprNode>() {new IdNode() {Name = "right"}}, ExpectedSize = 1}
        };

        // Act
        arrDclNode.Accept(_typeVisitor);

        // Assert
        Assert.Equal(TypeVal.Number, arrDclNode.Type.ActualType);
    }

    [Theory]
    [InlineData(TypeVal.Number, TypeVal.Boolean)]
    [InlineData(TypeVal.String, TypeVal.Number)]
    [InlineData(TypeVal.Boolean, TypeVal.String)]
    public void ArrDclNode_TypeMismatch_ThrowsTypeException(TypeVal type1, TypeVal type2)
    {
        //Arrange
        var symbol1 = new SymTableEntry() {Type = new Type() {ActualType = type1, IsArray = true}};
        var symbol2 = new SymTableEntry() {Type = new Type() {ActualType = type2, IsArray = true}};
        _symbolTableMock.Setup(x => x.RetrieveSymbol("left"))
            .Returns(symbol1);
        _symbolTableMock.Setup(x => x.RetrieveSymbol("right"))
            .Returns(symbol2);
        var arrDclNode = new ArrDclNode()
        {
            Left = new IdNode() {Name = "left"},
            Right = new ArrLiteralNode()
                {Elements = new List<ExprNode>() {new IdNode() {Name = "right"}}, ExpectedSize = 1}
        };

        //Act
        var test = () => arrDclNode.Accept(_typeVisitor);

        //Assert
        Assert.Throws<TypeMismatchException>(test);
    }

    [Theory]
    [InlineData(TypeVal.Number)]
    [InlineData(TypeVal.Boolean)]
    [InlineData(TypeVal.String)]
    public void ArrDclNode_ExprHasError_ThrowsTypeException(TypeVal type)
    {
        //Arrange
        var symbol = new SymTableEntry() {Type = new Type() {ActualType = TypeVal.Error, IsArray = true}};
        _symbolTableMock.Setup(x => x.RetrieveSymbol("right"))
            .Returns(symbol);
        var arrDclNode = new ArrDclNode
        {
            Size = 1,
            Left = new IdNode() {Name = "left", Type = new Type() {ActualType = type, IsArray = true}},
            Right = new ArrLiteralNode()
                {Elements = new List<ExprNode>() {new IdNode() {Name = "right"}}, ExpectedSize = 1}
        };

        //Act
        var test = () => arrDclNode.Accept(_typeVisitor);

        //Assert
        Assert.Throws<TypeMismatchException>(test);
    }

    [Fact]
    public void FuncDefNode_IsTypeOk_CorrectlyEnteredInSymbolTable()
    {
        // Arrange
        _symbolTableMock.Setup(x => x.RetrieveSymbol(It.IsAny<string>()))
            .Returns(new SymTableEntry() {Type = new Type() {ActualType = TypeVal.Number}});
        var funcDefNode = new FuncDefNode()
        {
            Id = new IdNode() {Name = "Add2"},
            Stmts = new List<StmtNode>()
            {
                new AssNode()
                {
                    Id = new IdNode() {Name = "a"},
                    Expr = new PlusNode()
                    {
                        Left = new IdNode() {Name = "a", Type = new Type() {ActualType = TypeVal.Number}},
                        Right = new NumberNode() {Value = 2}
                    }
                },
                new RetNode()
                {
                    RetVal = new IdNode() {Name = "return1", Type = new Type() {ActualType = TypeVal.Number}},
                    SurroundingFuncType = new Type() {ActualType = TypeVal.Number}
                }
            },
            FormalParams = new List<IdNode>()
            {
                new IdNode() {Name = "a"}
            },
            ReturnType = new Type() {ActualType = TypeVal.Number}
        };
        var funcsNode = new FuncsNode()
        {
            FuncDcls = new List<FuncDefNode>() {funcDefNode}
        };

        // Act
        funcsNode.Accept(_typeVisitor);

        // Assert
        Assert.Equal(TypeVal.Number, funcDefNode.Type.ActualType);
        _symbolTableMock.Verify(x => x.EnterSymbol(funcDefNode), Times.Once);
    }

    [Fact]
    public void FuncDefNode_ReturnMismatch_NotEnteredInSymbolTable()
    {
        // Arrange
        var funcDefNode = new FuncDefNode()
        {
            Id = new IdNode() {Name = "Add2"},
            Stmts = new List<StmtNode>()
            {
                new AssNode()
                {
                    Id = new IdNode() {Name = "a"},
                    Expr = new PlusNode()
                    {
                        Left = new IdNode() {Name = "a", Type = new Type() {ActualType = TypeVal.Number}},
                        Right = new NumberNode() {Value = 2}
                    }
                },
                new RetNode()
                {
                    RetVal = new IdNode() {Name = "a"},
                    SurroundingFuncType = new Type() {ActualType = TypeVal.Boolean}
                }
            },
            FormalParams = new List<IdNode>()
            {
                new IdNode() {Name = "a", Type = new Type() {ActualType = TypeVal.Number, IsArray = false}}
            },
            ReturnType = new Type() {ActualType = TypeVal.Boolean}
        };

        // Act
        var test = () => funcDefNode.Accept(_typeVisitor);

        // Assert
        Assert.Throws<SymbolNotDeclaredException>(test);
        _symbolTableMock.Verify(x => x.EnterSymbol(funcDefNode), Times.Never);
    }

    [Fact]
    public void FuncDefNode_HasPinParams_CorrectlyEnteredInSymbolTable()
    {
        // Arrange
        _symbolTableMock.Setup(x => x.RetrieveSymbol(It.IsAny<string>()))
            .Returns(new SymTableEntry() {Type = new Type() {ActualType = TypeVal.Number}});
        var funcDefNode = new FuncDefNode()
        {
            Id = new IdNode() {Name = "Add2"},
            Stmts = new List<StmtNode>()
            {
                new AssNode()
                {
                    Id = new IdNode() {Name = "a"},
                    Expr = new PlusNode()
                    {
                        Left = new IdNode() {Name = "a", Type = new Type() {ActualType = TypeVal.Number}},
                        Right = new NumberNode() {Value = 2}
                    }
                },
                new RetNode()
                {
                    RetVal = new IdNode() {Name = "return1", Type = new Type() {ActualType = TypeVal.Number}},
                    SurroundingFuncType = new Type() {ActualType = TypeVal.Number}
                }
            },
            FormalParams = new List<IdNode>()
            {
                new IdNode() {Name = "b", Type = new Type() {ActualType = TypeVal.Analogpin}},
                new IdNode() {Name = "c", Type = new Type() {ActualType = TypeVal.Digitalpin}}
            },
            ReturnType = new Type() {ActualType = TypeVal.Number}
        };

        // Act
        funcDefNode.Accept(_typeVisitor);

        // Assert
        Assert.Equal(TypeVal.Number, funcDefNode.Type.ActualType);
        _symbolTableMock.Verify(x => x.EnterSymbol(funcDefNode), Times.Once);
    }

    // The following tests are for the integration of the typevisitor and pin table
    [Theory]
    [InlineData(TypeVal.Analogpin)]
    [InlineData(TypeVal.Digitalpin)]
    public void PinDclNode_PinAlreadyDeclared_ThrowsDuplicatePinDeclarationException(TypeVal type)
    {
        // Arrange
        var pinDclNode1 = new PinDclNode()
        {
            Left = new IdNode() {Name = "a", Type = new Type() {ActualType = type}},
            Right = new NumberNode() {Value = 5}
        };
        var pinDclNode2 = new PinDclNode()
        {
            Left = new IdNode() {Name = "b", Type = new Type() {ActualType = type}},
            Right = new NumberNode() {Value = 5}
        };
        var varsNode = new VarsNode()
        {
            Dcls = new List<VarDclNode>()
            {
                pinDclNode1,
                pinDclNode2
            }
        };

        // Act
        var test = () => varsNode.Accept(_typeVisitor);

        // Assert
        Assert.Throws<DuplicatePinDeclarationException>(test);
    }

    [Theory]
    [InlineData(TypeVal.Analogpin)]
    [InlineData(TypeVal.Digitalpin)]
    public void PinDclNode_PinNotDeclared_DoesNotThrowException(TypeVal type)
    {
        // Arrange
        var pinDclNode1 = new PinDclNode()
        {
            Left = new IdNode() {Name = "a", Type = new Type() {ActualType = type}},
            Right = new NumberNode() {Value = 4}
        };
        var pinDclNode2 = new PinDclNode()
        {
            Left = new IdNode() {Name = "b", Type = new Type() {ActualType = type}},
            Right = new NumberNode() {Value = 5}
        };
        var varsNode = new VarsNode()
        {
            Dcls = new List<VarDclNode>()
            {
                pinDclNode1,
                pinDclNode2
            }
        };

        // Act & Assert
        try
        {
            varsNode.Accept(_typeVisitor);
        }
        catch (DuplicateDeclarationException e)
        {
            Assert.True(false, $"Expected no exception, but got {e.Message}");
        }
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(6)]
    public void PinDclNode_AnalogPinOutOfRange(int pinVal)
    {
        // Arrange
        var pinDclNode = new PinDclNode()
        {
            Left = new IdNode() {Name = "a", Type = new Type() {ActualType = TypeVal.Analogpin}},
            Right = new NumberNode() {Value = pinVal}
        };

        // Act
        var test = () => pinDclNode.Accept(_typeVisitor);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(test);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(14)]
    public void PinDclNode_DigitalPinOutOfRange(int pinVal)
    {
        // Arrange
        var pinDclNode = new PinDclNode()
        {
            Left = new IdNode() {Name = "a", Type = new Type() {ActualType = TypeVal.Digitalpin}},
            Right = new NumberNode() {Value = pinVal}
        };

        // Act
        var test = () => pinDclNode.Accept(_typeVisitor);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(test);
    }

    #endregion

    #region Statements

    [Fact]
    public void AssNode_LeftIsConst_ThrowsTypeException()
    {
        // Arrange
        var symbol = new SymTableEntry() {Type = new Type() {ActualType = TypeVal.Number, IsConstant = true}};
        _symbolTableMock.Setup(x => x.RetrieveSymbol(It.IsAny<string>()))
            .Returns(symbol);
        var assNode = new AssNode()
        {
            Id = new IdNode() {Name = "left"},
            Expr = new NumberNode() {Value = 2}
        };

        // Act
        var test = () => assNode.Accept(_typeVisitor);

        // Assert
        Assert.Throws<TypeMismatchException>(test);
    }

    [Theory]
    [InlineData(TypeVal.Number)]
    [InlineData(TypeVal.String)]
    public void IfNode_ConditionTypeMismatch_ThrowsTypeException(TypeVal type)
    {
        //Arrange
        var symbol = new SymTableEntry() {Type = new Type() {ActualType = type}};
        _symbolTableMock.Setup(x => x.RetrieveSymbol("bool"))
            .Returns(symbol);
        var ifNode = new IfNode
        {
            Condition = new IdNode() {Name = "bool"},
            ThenClause = new List<StmtNode>() {new ContNode()},
            ElseClause = new List<StmtNode>() {new BreakNode()}
        };

        //Act
        var test = () => ifNode.Accept(_typeVisitor);

        //Assert
        Assert.Throws<TypeMismatchException>(test);
    }

    [Fact]
    public void IfNode_ConditionIsBool_DoesNotThrow()
    {
        // Arrange
        var symbol = new SymTableEntry() {Type = new Type() {ActualType = TypeVal.Boolean}};
        _symbolTableMock.Setup(x => x.RetrieveSymbol("bool"))
            .Returns(symbol);
        var ifNode = new IfNode
        {
            Condition = new IdNode() {Name = "bool"},
            ThenClause = new List<StmtNode>() {new ContNode()},
            ElseClause = new List<StmtNode>() {new BreakNode()}
        };

        // Act & Assert
        try
        {
            ifNode.Accept(_typeVisitor);
        }
        catch (TypeMismatchException e)
        {
            Assert.True(false, $"Did not expect exception, but received {e.Message}");
        }
    }

    [Theory]
    [InlineData(TypeVal.Number)]
    [InlineData(TypeVal.String)]
    public void WhileNode_ConditionTypeMismatch_ThrowsTypeException(TypeVal type)
    {
        //Arrange
        var symbol = new SymTableEntry() {Type = new Type() {ActualType = type}};
        _symbolTableMock.Setup(x => x.RetrieveSymbol("cond"))
            .Returns(symbol);
        var whileNode = new WhileNode
        {
            Condition = new IdNode() {Name = "cond"},
            Body = new List<StmtNode>() {new ContNode()}
        };

        //Act
        var test = () => whileNode.Accept(_typeVisitor);

        //Assert
        Assert.Throws<TypeMismatchException>(test);
    }

    [Fact]
    public void WhileNode_ConditionIsBool_DoesNotThrow()
    {
        //Arrange
        var symbol = new SymTableEntry() {Type = new Type() {ActualType = TypeVal.Boolean}};
        _symbolTableMock.Setup(x => x.RetrieveSymbol("cond"))
            .Returns(symbol);
        var whileNode = new WhileNode
        {
            Condition = new IdNode() {Name = "cond"},
            Body = new List<StmtNode>() {new ContNode()}
        };

        // Act & Assert
        try
        {
            whileNode.Accept(_typeVisitor);
        }
        catch (TypeMismatchException e)
        {
            Assert.True(false, $"Did not expect exception, but received {e.Message}");
        }
    }

    [Theory]
    [InlineData(TypeVal.String, TypeVal.Number, TypeVal.Boolean)]
    public void ForNode_TypeMismatch_ThrowsTypeException(TypeVal type1, TypeVal type2, TypeVal type3)
    {
        //Arrange
        var symbol1 = new SymTableEntry() {Type = new Type() {ActualType = type1}};
        var symbol2 = new SymTableEntry() {Type = new Type() {ActualType = type2}};
        var symbol3 = new SymTableEntry() {Type = new Type() {ActualType = type3}};
        _symbolTableMock.Setup(x => x.RetrieveSymbol("init"))
            .Returns(symbol1);
        _symbolTableMock.Setup(x => x.RetrieveSymbol("limit"))
            .Returns(symbol2);
        _symbolTableMock.Setup(x => x.RetrieveSymbol("update"))
            .Returns(symbol3);
        var forNode = new ForNode
        {
            Initializer = new IdNode() {Name = "init"},
            Limit = new IdNode() {Name = "limit"},
            Update = new IdNode() {Name = "update"},
            Body = new List<StmtNode>() {new ContNode()}
        };

        //Act
        var test = () => forNode.Accept(_typeVisitor);

        //Assert
        Assert.Throws<TypeMismatchException>(test);
    }

    [Fact]
    public void ForNode_TypesAreNumber_DoesNotThrow()
    {
        //Arrange
        var symbol1 = new SymTableEntry() {Type = new Type() {ActualType = TypeVal.Number}};
        _symbolTableMock.Setup(x => x.RetrieveSymbol(It.IsAny<string>()))
            .Returns(symbol1);
        var forNode = new ForNode
        {
            Initializer = new IdNode() {Name = "init"},
            Limit = new IdNode() {Name = "limit"},
            Update = new IdNode() {Name = "update"},
            Body = new List<StmtNode>() {new ContNode()}
        };

        // Act & Assert
        try
        {
            forNode.Accept(_typeVisitor);
        }
        catch (TypeMismatchException e)
        {
            Assert.True(false, $"Did not expect exception, but received {e.Message}");
        }
    }

    [Theory]
    [InlineData(TypeVal.Number)]
    [InlineData(TypeVal.String)]
    [InlineData(TypeVal.Boolean)]
    public void AssNode_TypeMatch_IsTypeOk(TypeVal type)
    {
        // Arrange
        var symbol = new SymTableEntry() {Type = new Type() {ActualType = type}};
        _symbolTableMock.Setup(x => x.RetrieveSymbol(It.IsAny<string>()))
            .Returns(symbol);
        var assNode = new AssNode()
        {
            Id = new IdNode() {Name = "left"},
            Expr = new IdNode() {Name = "right"}
        };
        var loopNode = new LoopNode()
        {
            Stmts = new List<StmtNode>() {assNode}
        };

        // Act
        loopNode.Accept(_typeVisitor);

        // Assert
        Assert.Equal(TypeVal.Ok, assNode.Type.ActualType);
    }

    [Theory]
    [InlineData(TypeVal.Number, TypeVal.Boolean)]
    [InlineData(TypeVal.String, TypeVal.Number)]
    [InlineData(TypeVal.Boolean, TypeVal.String)]
    public void AssNode_TypeMismatch_ThrowsTypeException(TypeVal type1, TypeVal type2)
    {
        //Arrange
        var symbol1 = new SymTableEntry() {Type = new Type() {ActualType = type1}};
        var symbol2 = new SymTableEntry() {Type = new Type() {ActualType = type2}};
        _symbolTableMock.Setup(x => x.RetrieveSymbol("left"))
            .Returns(symbol1);
        _symbolTableMock.Setup(x => x.RetrieveSymbol("right"))
            .Returns(symbol2);
        var assNode = new AssNode()
        {
            Id = new IdNode() {Name = "left"},
            Expr = new IdNode() {Name = "right"}
        };

        //Act
        var test = () => assNode.Accept(_typeVisitor);

        //Assert
        Assert.Throws<TypeMismatchException>(test);
    }

    [Theory]
    [InlineData(TypeVal.Analogpin)]
    [InlineData(TypeVal.Digitalpin)]
    public void AssNode_LHSIsPin_ThrowsTypeException(TypeVal type)
    {
        // Arrange
        var symbol = new SymTableEntry() {Type = new Type() {ActualType = type}};
        _symbolTableMock.Setup(x => x.RetrieveSymbol("id"))
            .Returns(symbol);
        var assNode = new AssNode()
        {
            Id = new IdNode() {Name = "id"},
            Expr = new NumberNode() {Value = 5}
        };

        // Act
        var test = () => assNode.Accept(_typeVisitor);

        // Assert
        Assert.Throws<TypeMismatchException>(test);
    }

    [Theory]
    [InlineData(TypeVal.Number)]
    [InlineData(TypeVal.String)]
    [InlineData(TypeVal.Boolean)]
    public void RetNode_TypeMatch_IsTypeOk(TypeVal type)
    {
        // Arrange
        var symbol = new SymTableEntry() {Type = new Type() {ActualType = type}};
        _symbolTableMock.Setup(x => x.RetrieveSymbol(It.IsAny<string>()))
            .Returns(symbol);
        var retNode = new RetNode()
        {
            RetVal = new IdNode() {Name = "type"},
            SurroundingFuncType = new Type() {ActualType = type}
        };

        // Act
        retNode.Accept(_typeVisitor);

        // Assert
        Assert.Equal(TypeVal.Ok, retNode.Type.ActualType);
    }

    [Theory]
    [InlineData(TypeVal.Number, TypeVal.Boolean)]
    [InlineData(TypeVal.String, TypeVal.Number)]
    [InlineData(TypeVal.Boolean, TypeVal.String)]
    public void RetNode_TypeMismatch_ThrowsTypeException(TypeVal type1, TypeVal type2)
    {
        // Arrange
        var symbol = new SymTableEntry() {Type = new Type() {ActualType = type2}};
        _symbolTableMock.Setup(x => x.RetrieveSymbol(It.IsAny<string>()))
            .Returns(symbol);
        var retNode = new RetNode()
        {
            RetVal = new IdNode() {Name = "type2"},
            SurroundingFuncType = new Type() {ActualType = type1}
        };

        // Act
        var test = () => retNode.Accept(_typeVisitor);

        // Assert
        Assert.Throws<TypeMismatchException>(test);
    }

    [Fact]
    public void RetNode_NullExprParentIsBlank_IsTypeOk()
    {
        // Arrange
        var retNode = new RetNode()
        {
            RetVal = null,
            SurroundingFuncType = new Type() {ActualType = TypeVal.Blank}
        };

        // Act
        retNode.Accept(_typeVisitor);

        // Assert
        Assert.Equal(TypeVal.Ok, retNode.Type.ActualType);
    }

    [Fact]
    public void FuncStmtNode_ParamsMatch_IsTypeMatch()
    {
        // Arrange
        var symbol = new FunctionSymTableEntry()
        {
            Type = new Type() {ActualType = TypeVal.Blank},
            Parameters = new Dictionary<string, Type>()
            {
                {"a", new Type() {ActualType = TypeVal.Number}},
                {"b", new Type() {ActualType = TypeVal.String}},
                {"c", new Type() {ActualType = TypeVal.Boolean}}
            }
        };
        _symbolTableMock.Setup(x => x.RetrieveSymbol("func"))
            .Returns(symbol);
        _symbolTableMock.Setup(x => x.RetrieveSymbol("a"))
            .Returns(new SymTableEntry() {Type = new Type() {ActualType = TypeVal.Number}});
        _symbolTableMock.Setup(x => x.RetrieveSymbol("b"))
            .Returns(new SymTableEntry() {Type = new Type() {ActualType = TypeVal.String}});
        _symbolTableMock.Setup(x => x.RetrieveSymbol("c"))
            .Returns(new SymTableEntry() {Type = new Type() {ActualType = TypeVal.Boolean}});
        var funcExprNode = new FuncStmtNode()
        {
            Id = new IdNode() {Name = "func"},
            Params = new List<ExprNode>()
            {
                new IdNode() {Name = "a"},
                new IdNode() {Name = "b"},
                new IdNode() {Name = "c"}
            }
        };

        // Act
        funcExprNode.Accept(_typeVisitor);

        // Assert
        Assert.Equal(TypeVal.Blank, funcExprNode.Type.ActualType);
    }

    [Fact]
    public void FuncStmtNode_ParamsMismatch_ThrowsTypeException()
    {
        // Arrange
        var symbol = new FunctionSymTableEntry()
        {
            Type = new Type() {ActualType = TypeVal.Blank},
            Parameters = new Dictionary<string, Type>()
            {
                {"a", new Type() {ActualType = TypeVal.Number}},
                {"b", new Type() {ActualType = TypeVal.String}},
                {"c", new Type() {ActualType = TypeVal.Boolean}}
            }
        };
        _symbolTableMock.Setup(x => x.RetrieveSymbol("func"))
            .Returns(symbol);
        _symbolTableMock.Setup(x => x.RetrieveSymbol("a"))
            .Returns(new SymTableEntry() {Type = new Type() {ActualType = TypeVal.Number}});
        _symbolTableMock.Setup(x => x.RetrieveSymbol("b"))
            .Returns(new SymTableEntry() {Type = new Type() {ActualType = TypeVal.String}});
        _symbolTableMock.Setup(x => x.RetrieveSymbol("c"))
            .Returns(new SymTableEntry() {Type = new Type() {ActualType = TypeVal.Boolean}});
        var funcExprNode = new FuncStmtNode()
        {
            Id = new IdNode() {Name = "func"},
            Params = new List<ExprNode>()
            {
                new IdNode() {Name = "c"},
                new IdNode() {Name = "a"},
                new IdNode() {Name = "b"}
            }
        };

        // Act
        var test = () => funcExprNode.Accept(_typeVisitor);

        // Assert
        Assert.Throws<TypeMismatchException>(test);
    }

    [Fact]
    public void FuncsTMTNode_IsNotFunc_ThrowsException()
    {
        // Arrange
        var symbol = new SymTableEntry() {Type = new Type() {ActualType = TypeVal.Number}};
        _symbolTableMock.Setup(x => x.RetrieveSymbol("func"))
            .Returns(symbol);
        var funcExprNode = new FuncStmtNode()
        {
            Id = new IdNode() {Name = "func"},
        };

        // Act
        var test = () => funcExprNode.Accept(_typeVisitor);

        // Assert
        Assert.Throws<SymbolNotDeclaredException>(test);
    }

    #endregion Statements
}