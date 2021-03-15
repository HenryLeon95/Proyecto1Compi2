using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Parsing;

namespace Proyecto1.Analizador
{
    class Gramatica : Grammar
    {
        public Gramatica() : base (false)
        {
            #region ER
            IdentifierTerminal Id = new IdentifierTerminal("id");
            RegexBasedTerminal Entero = new RegexBasedTerminal("numero", "[0-9]+");
            RegexBasedTerminal Decimal = new RegexBasedTerminal("real", "[0-9]+[.][0-9]+");
            StringLiteral Cadenas = new StringLiteral("Cadenas", "\"");
            StringLiteral Cadena = new StringLiteral("cadena", "'", StringOptions.IsTemplate);
            CommentTerminal comentarioLinea = new CommentTerminal("comentarioLinea", "//", "\n", "\r\n");
            CommentTerminal comentarioBloque1 = new CommentTerminal("comentarioBloque1", "(*", "*)");
            CommentTerminal comentarioBloque2 = new CommentTerminal("comentarioBloque2", "{", "}");
            #endregion


            #region Terminales
            #region SIGNOS
            var PYC = ToTerm(";");
            var DOSP = ToTerm(":");
            var COMA = ToTerm(",");
            var PUNTO = ToTerm(".");
            var PAR_IZQ = ToTerm("(");
            var PAR_DER = ToTerm(")");
            var COR_IZQ = ToTerm("[");
            var COR_DER = ToTerm("]");
            var IGUAL = ToTerm("=");
            var ASIGNACION = ToTerm(":=");
            #endregion

            #region Operaciones
            var MAS = ToTerm("+");
            var MENOS = ToTerm("-");
            var POR = ToTerm("*");
            var DIVISION = ToTerm("/");
            var MODULO = ToTerm("%");
            var MAYOR_IGUAL_QUE = ToTerm(">=");
            var MENOR_IGUAL_QUE = ToTerm("<=");
            var DIFERENCIA = ToTerm("<>");
            var MAYOR_QUE = ToTerm(">");
            var MENOR_QUE = ToTerm("<");
            var AND_Y = ToTerm("and");
            var OR_O = ToTerm("or");
            var NOT_NO = ToTerm("not");
            #endregion

            #region TIPOS
            var STRING_T = ToTerm("string");
            var INTEGER_T = ToTerm("integer");
            var REAL_T = ToTerm("real");
            var BOOLEAN_T = ToTerm("boolean");
            var OBJECT_T = ToTerm("object");
            var ARRAY_T = ToTerm("array");
            var TYPE_T = ToTerm("type");
            var VAR_T = ToTerm("var");
            var CONST_T = ToTerm("const");
            #endregion

            #region Reservadas
            var IFS = ToTerm("if");
            var ELSEIF = ToTerm("else if");
            var ELSES = ToTerm("else");
            var THENS = ToTerm("then");
            var CASES = ToTerm("case");
            var WHILES = ToTerm("while");
            var DOS = ToTerm("do");
            var REPEATS = ToTerm("repeat");
            var UNTILS = ToTerm("until");
            var FORS = ToTerm("for");
            var BREAKS = ToTerm("break");
            var CONTINUES = ToTerm("continue");

            var PROGRAM_T = ToTerm("program");
            var FUNCTION_T = ToTerm("function");
            var PROCEDURE_T = ToTerm("procedure");
            var BEGIN_T = ToTerm("begin");
            var END_T = ToTerm("end");
            var VOID_T = ToTerm("void");
            var USES_T = ToTerm("uses");
            var OF_T = ToTerm("of");
            var TO_T = ToTerm("to");
            var TRUE_T = ToTerm("true");
            var FALSE_T = ToTerm("false");
            var WRITES = ToTerm("write");
            var WRITELNS = ToTerm("writeln");
            var EXITS = ToTerm("exit");
            var GRAF = ToTerm("graficar_ts");
            #endregion
            #endregion


            #region NoTerminales
            NonTerminal inicio = new NonTerminal("inicio"),
                cuerpo = new NonTerminal("cuerpo"),
                interno = new NonTerminal("interno"),
                var1 = new NonTerminal("var1"),
                var2 = new NonTerminal("var2"),
                list_vars = new NonTerminal("list_vars"),
                list_id = new NonTerminal("list_id"),
                cont_array = new NonTerminal("cont_array"),
                list_cond = new NonTerminal("list_cond"),
                cond = new NonTerminal("cond"),
                cond1 = new NonTerminal("cond1"),
                cond2 = new NonTerminal("cond2"),
                cond3 = new NonTerminal("cond3"),
                cond4 = new NonTerminal("cond4"),
                cond5 = new NonTerminal("cond5"),
                cond6 = new NonTerminal("cond6"),
                cond7 = new NonTerminal("cond7"),
                cond8 = new NonTerminal("cond8"),
                exp = new NonTerminal("exp"),
                exp1 = new NonTerminal("exp1"),
                exp2 = new NonTerminal("exp2"),
                exp3 = new NonTerminal("exp3"),
                exp4 = new NonTerminal("exp4"),
                terminal = new NonTerminal("terminal"),
                list_param = new NonTerminal("list_param"),
                param = new NonTerminal("param"),
                param2 = new NonTerminal("param2"),
                funcion = new NonTerminal("funcion"),
                procedimiento = new NonTerminal("procedimiento"),
                list_sentencias = new NonTerminal("list_sentencias"),
                sentencia = new NonTerminal("sentencia"),
                list_case = new NonTerminal("list_case"),
                case_n = new NonTerminal("case_n"),

                tipo = new NonTerminal("tipo"),
                comentarios = new NonTerminal("comentarios"),
                comentario = new NonTerminal("comentario")
            ;
            #endregion

            #region Gramatica
            //--------------------------------------------- INICIO
            inicio.Rule = PROGRAM_T + Id + PYC + cuerpo + BEGIN_T + list_sentencias + END_T + PUNTO;
            inicio.ErrorRule = SyntaxError + PYC
                            | SyntaxError + PUNTO;

            //--------------------------------------------- CUERPO
            cuerpo.Rule = cuerpo + interno
                            | interno
                            | Empty;

            interno.Rule = var1
                            | funcion
                            | procedimiento;

            //-------------------------------------------- VARIABLES SIMPLES
            var1.Rule = TYPE_T + list_vars
                            | VAR_T + list_vars
                            | CONST_T + list_vars
                            | CONST_T + Id + IGUAL + cond + PYC
                            | Id + IGUAL + cond + PYC;
            var1.ErrorRule = SyntaxError + PYC;

            list_vars.Rule = list_vars + var2
                            | var2;

            //--------------------------------------------- VARIABLES CON ASIGNACIONY LISTA DE VARIABLES
            var2.Rule = list_id + DOSP + tipo + PYC
                            | list_id + IGUAL + tipo + PYC
                            | list_id + DOSP + tipo + IGUAL + cond + PYC
                            | list_id + IGUAL + OBJECT_T + VAR_T + list_vars + END_T + PYC
                            | list_id + IGUAL + ARRAY_T + COR_IZQ + cont_array + COR_DER + OF_T + tipo + PYC;
            var2.ErrorRule = SyntaxError + PYC;

            list_id.Rule = list_id + COMA + Id
                            | Id;

            cont_array.Rule = Entero
                            | MENOS + Entero
                            | Entero + ToTerm("..") + Entero
                            | Entero + ToTerm("..") + MENOS + Entero
                            | MENOS + Entero + ToTerm("..") + Entero
                            | MENOS + Entero + ToTerm("..") + MENOS + Entero
                            | Id
                            | Cadena;

            //------------------------------------------- PARAMETROS
            list_param.Rule = list_param + PYC + param
                            | param;

            param.Rule = param2 + DOSP + tipo
                            | VAR_T + param2 + DOSP + tipo
                            | Empty;

            param2.Rule = param2 + COMA + Id
                            | Id;

            //--------------------------------------------FUNCIONES
            funcion.Rule = FUNCTION_T + Id + PAR_IZQ + list_param + PAR_DER + DOSP + tipo + PYC + cuerpo + BEGIN_T + list_sentencias + END_T + PYC
                            | FUNCTION_T + Id + DOSP + tipo + PYC + cuerpo + BEGIN_T + list_sentencias + END_T + PYC;
            funcion.ErrorRule = SyntaxError + PYC;

            //--------------------------------------------PROCEDIMIENTOS
            procedimiento.Rule = PROCEDURE_T + Id + PAR_IZQ + list_param + PAR_DER + PYC + cuerpo + BEGIN_T + list_sentencias + END_T + PYC
                            | PROCEDURE_T + Id + PYC + cuerpo + BEGIN_T + list_sentencias + END_T + PYC;
            procedimiento.ErrorRule = SyntaxError + PYC;

            //-------------------------------------------- SENTENCIAS
            list_sentencias.Rule = list_sentencias + sentencia
                            | sentencia;

            sentencia.Rule = Id + PAR_IZQ + PAR_DER + PYC
                            | Id + PAR_IZQ + list_cond + PAR_DER + PYC
                            | Id + COR_IZQ + list_cond + COR_DER + ASIGNACION + cond + PYC
                            | Id + ASIGNACION + cond + PYC
                            | WRITES + PAR_IZQ + list_cond + PAR_DER + PYC
                            | WRITELNS + PAR_IZQ + list_cond + PAR_DER + PYC
                            | GRAF + PAR_IZQ + PAR_DER + PYC
                            | IFS + cond + THENS + list_sentencias
                            | IFS + cond + THENS + list_sentencias + ELSES + list_sentencias
                            | IFS + cond + THENS + BEGIN_T + list_sentencias + END_T
                            | IFS + cond + THENS + BEGIN_T + list_sentencias + END_T + ELSES + BEGIN_T + list_sentencias + END_T
                            | IFS + cond + THENS + BEGIN_T + list_sentencias + END_T + ELSES + list_sentencias
                            | IFS + cond + THENS + BEGIN_T + list_sentencias + END_T + ELSES + list_sentencias + PYC
                            | IFS + cond + THENS + BEGIN_T + list_sentencias + END_T + PYC
                            | IFS + cond + THENS + BEGIN_T + list_sentencias + END_T + ELSES + BEGIN_T + list_sentencias + END_T + PYC
                            | CASES + cond + OF_T + list_case + ELSES + list_sentencias + END_T + PYC
                            | CASES + cond + OF_T + list_case + ELSES + BEGIN_T + list_sentencias + END_T + PYC + END_T + PYC
                            | CASES + cond + OF_T + list_case + END_T + PYC
                            | WHILES + cond + DOS + BEGIN_T + list_sentencias + END_T + PYC
                            | REPEATS + list_sentencias + UNTILS + cond + PYC
                            | FORS + Id + ASIGNACION + terminal + TO_T + terminal + DOS + BEGIN_T + list_sentencias + END_T + PYC
                            | EXITS + PAR_IZQ + PAR_DER + PYC
                            | EXITS + PAR_IZQ + cond + PAR_DER + PYC
                            | BREAKS + PYC
                            | CONTINUES + PYC;
            sentencia.ErrorRule = SyntaxError + PYC;

            list_case.Rule = list_case + case_n
                            | case_n;

            case_n.Rule = cont_array + DOSP + list_sentencias
                            | cont_array + DOSP + BEGIN_T + list_sentencias + END_T + PYC;

            //-------------------------------------------- CONDICIONES
            list_cond.Rule = list_cond + COMA + cond
                            | cond;

            //Se trabajará con recursividad por la izquierda para quitar la ambiguedad. Porque dió problemas con las precedencias
            cond.Rule = cond + AND_Y + cond1
                            | cond1;

            cond1.Rule = cond1 + OR_O + cond2
                            | cond2;

            cond2.Rule = NOT_NO + cond3
                            | cond3;

            // cond2.Rule = cond2 + NOT_NO + cond3
            //                | cond3;

            cond3.Rule = cond3 + MENOR_IGUAL_QUE + cond4
                            | cond4;

            cond4.Rule = cond4 + MAYOR_IGUAL_QUE + cond5
                            | cond5;

            cond5.Rule = cond5 + MENOR_QUE + cond6
                            | cond6;

            cond6.Rule = cond6 + MAYOR_QUE + cond7
                            | cond7;

            cond7.Rule = cond7 + IGUAL + cond8
                            | cond8;

            cond8.Rule = cond8 + DIFERENCIA + exp
                            | exp;

            //-------------------------------------------- EXPRESIONES ARITMETICAS
            exp.Rule = exp + MAS + exp1
                            | exp1;

            exp1.Rule = exp1 + MENOS + exp2
                            | exp2;

            exp2.Rule = exp2 + POR + exp3
                            | exp3;

            exp3.Rule = exp3 + DIVISION + exp4
                            | exp4;

            exp4.Rule = exp4 + MODULO + terminal
                            | terminal;

            //------------------------------------------ULTIMO-TERMINALES
            terminal.Rule = Id
                            | Cadena
                            | Entero
                            | REAL_T
                            | MENOS + Entero
                            | MENOS + REAL_T
                            | TRUE_T
                            | FALSE_T
                            | Id + PAR_IZQ + PAR_DER // Invocación sin parámetros
                            | Id + PAR_IZQ + list_cond + PAR_DER //Invocación con parámetros
                            | Id + COR_IZQ + list_cond + COR_DER //Accediendo a un array
                            | PAR_IZQ + cond + PAR_DER;
                            //| NOT_NO + terminal;

            //-------------------------------------------ULTIMO-VARIABLES
            tipo.Rule = STRING_T
                            | INTEGER_T
                            | REAL_T
                            | BOOLEAN_T
                            | Id;

            comentarios.Rule = MakePlusRule(comentarios, comentario);

            comentario.Rule = comentarioLinea
                            | comentarioBloque1
                            | comentarioBloque2;
            #endregion

            this.Root = inicio;

            #region Config
            MarkReservedWords(PROGRAM_T.ToString(), BEGIN_T.ToString(), END_T.ToString());

            NonGrammarTerminals.Add(comentarioLinea);
            NonGrammarTerminals.Add(comentarioBloque1);
            NonGrammarTerminals.Add(comentarioBloque2);
            #endregion
        }
    }
}
