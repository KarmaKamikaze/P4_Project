//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.9.2
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from E:/Code/repos/STEP/STEP/ANTLR gens\STEP.g4 by ANTLR 4.9.2

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

namespace STEP {
using Antlr4.Runtime.Misc;
using IParseTreeListener = Antlr4.Runtime.Tree.IParseTreeListener;
using IToken = Antlr4.Runtime.IToken;

/// <summary>
/// This interface defines a complete listener for a parse tree produced by
/// <see cref="STEPParser"/>.
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.9.2")]
[System.CLSCompliant(false)]
public interface ISTEPListener : IParseTreeListener {
	/// <summary>
	/// Enter a parse tree produced by <see cref="STEPParser.program"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterProgram([NotNull] STEPParser.ProgramContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="STEPParser.program"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitProgram([NotNull] STEPParser.ProgramContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="STEPParser.setuploop"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSetuploop([NotNull] STEPParser.SetuploopContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="STEPParser.setuploop"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSetuploop([NotNull] STEPParser.SetuploopContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="STEPParser.setup"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSetup([NotNull] STEPParser.SetupContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="STEPParser.setup"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSetup([NotNull] STEPParser.SetupContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="STEPParser.loop"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLoop([NotNull] STEPParser.LoopContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="STEPParser.loop"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLoop([NotNull] STEPParser.LoopContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="STEPParser.variables"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterVariables([NotNull] STEPParser.VariablesContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="STEPParser.variables"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitVariables([NotNull] STEPParser.VariablesContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="STEPParser.var_or_nl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterVar_or_nl([NotNull] STEPParser.Var_or_nlContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="STEPParser.var_or_nl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitVar_or_nl([NotNull] STEPParser.Var_or_nlContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="STEPParser.functions"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFunctions([NotNull] STEPParser.FunctionsContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="STEPParser.functions"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFunctions([NotNull] STEPParser.FunctionsContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="STEPParser.funcdcl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFuncdcl([NotNull] STEPParser.FuncdclContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="STEPParser.funcdcl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFuncdcl([NotNull] STEPParser.FuncdclContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="STEPParser.funcdcl_or_nl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFuncdcl_or_nl([NotNull] STEPParser.Funcdcl_or_nlContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="STEPParser.funcdcl_or_nl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFuncdcl_or_nl([NotNull] STEPParser.Funcdcl_or_nlContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="STEPParser.brackets"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterBrackets([NotNull] STEPParser.BracketsContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="STEPParser.brackets"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitBrackets([NotNull] STEPParser.BracketsContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="STEPParser.params"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterParams([NotNull] STEPParser.ParamsContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="STEPParser.params"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitParams([NotNull] STEPParser.ParamsContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="STEPParser.params_content"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterParams_content([NotNull] STEPParser.Params_contentContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="STEPParser.params_content"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitParams_content([NotNull] STEPParser.Params_contentContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="STEPParser.params_multi"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterParams_multi([NotNull] STEPParser.Params_multiContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="STEPParser.params_multi"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitParams_multi([NotNull] STEPParser.Params_multiContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="STEPParser.type"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterType([NotNull] STEPParser.TypeContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="STEPParser.type"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitType([NotNull] STEPParser.TypeContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="STEPParser.stmt"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterStmt([NotNull] STEPParser.StmtContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="STEPParser.stmt"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitStmt([NotNull] STEPParser.StmtContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="STEPParser.stmts"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterStmts([NotNull] STEPParser.StmtsContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="STEPParser.stmts"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitStmts([NotNull] STEPParser.StmtsContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="STEPParser.loop_stmt"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLoop_stmt([NotNull] STEPParser.Loop_stmtContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="STEPParser.loop_stmt"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLoop_stmt([NotNull] STEPParser.Loop_stmtContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="STEPParser.loop_stmts"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLoop_stmts([NotNull] STEPParser.Loop_stmtsContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="STEPParser.loop_stmts"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLoop_stmts([NotNull] STEPParser.Loop_stmtsContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="STEPParser.loopifbody"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLoopifbody([NotNull] STEPParser.LoopifbodyContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="STEPParser.loopifbody"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLoopifbody([NotNull] STEPParser.LoopifbodyContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="STEPParser.ifstmt"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterIfstmt([NotNull] STEPParser.IfstmtContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="STEPParser.ifstmt"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitIfstmt([NotNull] STEPParser.IfstmtContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="STEPParser.elseifstmt"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterElseifstmt([NotNull] STEPParser.ElseifstmtContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="STEPParser.elseifstmt"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitElseifstmt([NotNull] STEPParser.ElseifstmtContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="STEPParser.elsestmt"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterElsestmt([NotNull] STEPParser.ElsestmtContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="STEPParser.elsestmt"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitElsestmt([NotNull] STEPParser.ElsestmtContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="STEPParser.loopifstmt"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLoopifstmt([NotNull] STEPParser.LoopifstmtContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="STEPParser.loopifstmt"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLoopifstmt([NotNull] STEPParser.LoopifstmtContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="STEPParser.loopelseifstmt"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLoopelseifstmt([NotNull] STEPParser.LoopelseifstmtContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="STEPParser.loopelseifstmt"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLoopelseifstmt([NotNull] STEPParser.LoopelseifstmtContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="STEPParser.loopelsestmt"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLoopelsestmt([NotNull] STEPParser.LoopelsestmtContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="STEPParser.loopelsestmt"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLoopelsestmt([NotNull] STEPParser.LoopelsestmtContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="STEPParser.whilestmt"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterWhilestmt([NotNull] STEPParser.WhilestmtContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="STEPParser.whilestmt"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitWhilestmt([NotNull] STEPParser.WhilestmtContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="STEPParser.forstmt"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterForstmt([NotNull] STEPParser.ForstmtContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="STEPParser.forstmt"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitForstmt([NotNull] STEPParser.ForstmtContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="STEPParser.for_iter_opt"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFor_iter_opt([NotNull] STEPParser.For_iter_optContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="STEPParser.for_iter_opt"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFor_iter_opt([NotNull] STEPParser.For_iter_optContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="STEPParser.assstmt"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAssstmt([NotNull] STEPParser.AssstmtContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="STEPParser.assstmt"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAssstmt([NotNull] STEPParser.AssstmtContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="STEPParser.funccall"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFunccall([NotNull] STEPParser.FunccallContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="STEPParser.funccall"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFunccall([NotNull] STEPParser.FunccallContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="STEPParser.params_options"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterParams_options([NotNull] STEPParser.Params_optionsContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="STEPParser.params_options"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitParams_options([NotNull] STEPParser.Params_optionsContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="STEPParser.multi_expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMulti_expr([NotNull] STEPParser.Multi_exprContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="STEPParser.multi_expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMulti_expr([NotNull] STEPParser.Multi_exprContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="STEPParser.retstmt"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterRetstmt([NotNull] STEPParser.RetstmtContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="STEPParser.retstmt"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitRetstmt([NotNull] STEPParser.RetstmtContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="STEPParser.arrindex"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterArrindex([NotNull] STEPParser.ArrindexContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="STEPParser.arrindex"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitArrindex([NotNull] STEPParser.ArrindexContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="STEPParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExpr([NotNull] STEPParser.ExprContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="STEPParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExpr([NotNull] STEPParser.ExprContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="STEPParser.term"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterTerm([NotNull] STEPParser.TermContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="STEPParser.term"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitTerm([NotNull] STEPParser.TermContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="STEPParser.factor"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFactor([NotNull] STEPParser.FactorContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="STEPParser.factor"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFactor([NotNull] STEPParser.FactorContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="STEPParser.value"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterValue([NotNull] STEPParser.ValueContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="STEPParser.value"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitValue([NotNull] STEPParser.ValueContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="STEPParser.constant"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterConstant([NotNull] STEPParser.ConstantContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="STEPParser.constant"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitConstant([NotNull] STEPParser.ConstantContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="STEPParser.logicexpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLogicexpr([NotNull] STEPParser.LogicexprContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="STEPParser.logicexpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLogicexpr([NotNull] STEPParser.LogicexprContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="STEPParser.logicequal"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLogicequal([NotNull] STEPParser.LogicequalContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="STEPParser.logicequal"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLogicequal([NotNull] STEPParser.LogicequalContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="STEPParser.logiccomp"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLogiccomp([NotNull] STEPParser.LogiccompContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="STEPParser.logiccomp"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLogiccomp([NotNull] STEPParser.LogiccompContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="STEPParser.logiccompop"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLogiccompop([NotNull] STEPParser.LogiccompopContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="STEPParser.logiccompop"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLogiccompop([NotNull] STEPParser.LogiccompopContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="STEPParser.logicvalue"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLogicvalue([NotNull] STEPParser.LogicvalueContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="STEPParser.logicvalue"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLogicvalue([NotNull] STEPParser.LogicvalueContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="STEPParser.vardcl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterVardcl([NotNull] STEPParser.VardclContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="STEPParser.vardcl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitVardcl([NotNull] STEPParser.VardclContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="STEPParser.var_options"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterVar_options([NotNull] STEPParser.Var_optionsContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="STEPParser.var_options"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitVar_options([NotNull] STEPParser.Var_optionsContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="STEPParser.numdcl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterNumdcl([NotNull] STEPParser.NumdclContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="STEPParser.numdcl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitNumdcl([NotNull] STEPParser.NumdclContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="STEPParser.stringdcl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterStringdcl([NotNull] STEPParser.StringdclContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="STEPParser.stringdcl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitStringdcl([NotNull] STEPParser.StringdclContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="STEPParser.booldcl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterBooldcl([NotNull] STEPParser.BooldclContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="STEPParser.booldcl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitBooldcl([NotNull] STEPParser.BooldclContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="STEPParser.pindcl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterPindcl([NotNull] STEPParser.PindclContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="STEPParser.pindcl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitPindcl([NotNull] STEPParser.PindclContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="STEPParser.arrdcl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterArrdcl([NotNull] STEPParser.ArrdclContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="STEPParser.arrdcl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitArrdcl([NotNull] STEPParser.ArrdclContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="STEPParser.arr_id_or_lit"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterArr_id_or_lit([NotNull] STEPParser.Arr_id_or_litContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="STEPParser.arr_id_or_lit"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitArr_id_or_lit([NotNull] STEPParser.Arr_id_or_litContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="STEPParser.arrsizedcl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterArrsizedcl([NotNull] STEPParser.ArrsizedclContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="STEPParser.arrsizedcl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitArrsizedcl([NotNull] STEPParser.ArrsizedclContext context);
}
} // namespace STEP
