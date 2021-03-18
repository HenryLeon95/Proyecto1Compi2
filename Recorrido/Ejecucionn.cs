using Irony.Parsing;
using Proyecto1.Analizador;
using Proyecto1.Ejecucion;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Proyecto1.Recorrido
{
    class Ejecucionn
    {
        bool flag = false;
        private static List<Entorno> entG;
        private Entorno cimaEnt;
        public Stack<Entorno> pilaEntornos;
        public List<Error> error = new List<Error>();
        public Stack<TablaSimbolos> pilaSimbolos;
        private TablaSimbolos cimaTS;
        private int nivelActual = 0;

        public Ejecucionn(List<Entorno> ent)
        {
            entG = ent;
            pilaSimbolos = new Stack<TablaSimbolos>();
            pilaEntornos = new Stack<Entorno>();
        }

        public void Procedure()
        {
            Debug.WriteLine("Buscando entorno: {0}, {1} ",entG[0].Ambito, entG[0].AmbitoPadre);
            cimaEnt = getEntorno(entG[0].Ambito, entG[0].AmbitoPadre, entG);
            Debug.WriteLine(cimaEnt.var.Count);
            //cimaEnt = getEntorno("raiz", "raiz", entG);
            pilaEntornos.Push(cimaEnt);
            Ejecutar(cimaEnt.nodoAux);
        }

        private void Ejecutar(ParseTreeNode Nodo)
        {
            TablaSimbolos proc = new TablaSimbolos(0, Reservada.Program, false, false, false);
            pilaSimbolos.Push(proc);
            cimaTS = proc;      
            nivelActual = 0;
            Retorno2 ret = Sentencias(Nodo);
        }

        private Retorno2 Sentencias(ParseTreeNode Nodo)
        {
            switch (Nodo.Term.Name)
            {
                case "list_sentencias":
                    foreach (ParseTreeNode hijo in Nodo.ChildNodes)
                    {
                        Retorno2 retorno = Sentencias(hijo); 
                    }
                    break;
                case "sentencia":
                    #region
                    switch (Nodo.ChildNodes.Count)
                    {
                        case 1: 
                        case 4:
                            switch (Nodo.ChildNodes[1].Term.Name)
                            {
                                case "(":
                                    string id4 = Nodo.ChildNodes[0].Token.Value.ToString();
                                    Entorno ent4 = getEntornoL(id4, cimaEnt.AmbitoPadre);

                                    if (ent4 != null)
                                    {
                                        nivelActual++; 
                                        TablaSimbolos proc = new TablaSimbolos(0, Reservada.Procedure, false, false, false);
                                        pilaSimbolos.Push(proc);
                                        pilaEntornos.Push(ent4);
                                        cimaTS = proc;          
                                        cimaEnt = ent4;         

                                        Sentencias(cimaEnt.nodoAux);

                                        nivelActual--;
                                        pilaSimbolos.Pop(); 
                                        pilaEntornos.Pop();
                                        cimaTS = pilaSimbolos.Peek(); 
                                        cimaEnt = pilaEntornos.Peek();

                                        return new Retorno2("-", "-", "0", "0");
                                    }
                                    else
                                    {
                                        Debug.WriteLine("**Error Semantico** Funcion/Procedimiento en la linea: " + getLine(Nodo.ChildNodes[0]) + " columna:" + getColumn(Nodo.ChildNodes[0]));
                                        error.Add(new Error(int.Parse(getLine(Nodo.ChildNodes[0])), int.Parse(getColumn(Nodo.ChildNodes[0])), Reservada.ErrorSemantico, "Funcion / Procedimiento no existente"));
                                    }
                                    break;

                                case ":=":
                                    #region
                                    id4 = Nodo.ChildNodes[0].Token.Value.ToString();

                                    Simbolo var = RetornarSimbolo(id4);

                                    if (var != null) 
                                    {
                                        Retorno ret = Cond(Nodo.ChildNodes[2]);

                                        if (ret != null)
                                        {
                                            if (ret.Type.Equals(var.Tipo)) 
                                            {
                                                Console.WriteLine("Se asigno variable: " + id4 + ": " + ret.Value + " (" + ret.Type + ")");
                                                var.Valor = ret.Value;
                                            }
                                            else
                                            {
                                                Debug.WriteLine("**Error Semantico** Asignación inválida. Tipo de dato incorrecto en la linea: " + getLine(Nodo.ChildNodes[0]) + " columna:" + getColumn(Nodo.ChildNodes[0]));
                                                error.Add(new Error(int.Parse(getLine(Nodo.ChildNodes[0])), int.Parse(getColumn(Nodo.ChildNodes[0])), Reservada.ErrorSemantico, "Asignación inválida, tipo de dato incorrecto"));
                                            }
                                        }
                                        else
                                        {
                                            Debug.WriteLine("**Error Semantico** Asignación inválida. Expresión incorrecta en la linea: " + getLine(Nodo.ChildNodes[0]) + " columna:" + getColumn(Nodo.ChildNodes[0]));
                                            error.Add(new Error(int.Parse(getLine(Nodo.ChildNodes[0])), int.Parse(getColumn(Nodo.ChildNodes[0])), Reservada.ErrorSemantico, "Asignación inválida. Expresión incorrecta"));
                                        }
                                    }
                                    else
                                    {
                                        Debug.WriteLine("**Error Semantico** Variable: " + id4 + " no existe en la linea: " + getLine(Nodo.ChildNodes[0]) + " columna:" + getColumn(Nodo.ChildNodes[0]));
                                        error.Add(new Error(int.Parse(getLine(Nodo.ChildNodes[0])), int.Parse(getColumn(Nodo.ChildNodes[0])), Reservada.ErrorSemantico, "Variable: " + id4 + " no existe."));
                                    }
                                    #endregion

                                    break;
                            }

                            switch (Nodo.ChildNodes[0].Term.Name)
                            {
                                case "graficar_ts":
                                    #region
                                    PdfTS();
                                    Debug.WriteLine("Graficando TS *****************************");
                                    #endregion
                                    break;

                                case "else":
                                    Debug.WriteLine("IIIIIIIIIIIIIIIIIIIIF 0");

                                    Retorno2 ret2 = Sentencias(Nodo.ChildNodes[2]);
                                    return ret2;
                            }
                            break;
                        case 5:
                            #region
                            switch (Nodo.ChildNodes[0].Term.Name)
                            {
                                case "write":
                                    string retWrite = getPrint(Nodo.ChildNodes[2]);
                                    Form1.Salida_Inst.AppendText(retWrite);
                                    return new Retorno2("-", "-", "0", "0");

                                case "writeln":
                                    string retWriteln = getPrint(Nodo.ChildNodes[2]);
                                    Form1.Salida_Inst.AppendText(retWriteln + "\n");
                                    Debug.WriteLine("IMPRIMIENDO");
                                    return new Retorno2("-", "-", "0", "0");

                                case "repeat":
                                    Debug.WriteLine("REPEAT 0000001");
                                    bool flag2 = true;
                                    int con = 0;
                                    while (flag2)
                                    {
                                        Retorno retW = Cond(Nodo.ChildNodes[3]);
                                        Debug.WriteLine("El REPEAT es: " + retW.Value);

                                        if (retW != null)
                                        {
                                            if (retW.Value.Equals("False") || con == 0)
                                            {
                                                Debug.WriteLine("El WHILE es True");
                                                Retorno2 ret2 = Sentencias(Nodo.ChildNodes[1]);
                                                con += 1;
                                            }
                                            else
                                            {
                                                flag2 = false;
                                            }
                                        }
                                        else
                                        {
                                            Debug.WriteLine("**Error Semantico** . REPEAT incorrecto en la linea: " + getLine(Nodo.ChildNodes[0]) + " columna:" + getColumn(Nodo.ChildNodes[0]));
                                            error.Add(new Error(int.Parse(getLine(Nodo.ChildNodes[0])), int.Parse(getColumn(Nodo.ChildNodes[0])), Reservada.ErrorSemantico, "REPEAT incorrecto"));
                                        }
                                    }

                                    return new Retorno2("-", "-", "0", "0");
                            }
                            #endregion
                            break;

                        case 7:
                            #region
                            switch (Nodo.ChildNodes[0].Term.Name)
                            {
                                case "if":
                                    if (Nodo.ChildNodes[6].Term.Name.Equals(";"))
                                    { 
                                        Debug.WriteLine("IIIIIIIIIIIIIIIIIIIIF 00000000");
                                        Retorno ret = Cond(Nodo.ChildNodes[1]);

                                        if (ret != null)
                                        {
                                            Debug.WriteLine("Valor retornado del IF: " + ret.Value);
                                            if (ret.Value.Equals("True"))
                                            {
                                                Debug.WriteLine("El if es True");
                                                Retorno2 ret2 = Sentencias(Nodo.ChildNodes[4]);
                                                return ret2;
                                            }
                                        }
                                        else
                                        {
                                            Debug.WriteLine("**Error Semantico** . IF incorrecto en la linea: " + getLine(Nodo.ChildNodes[0]) + " columna:" + getColumn(Nodo.ChildNodes[0]));
                                            error.Add(new Error(int.Parse(getLine(Nodo.ChildNodes[0])), int.Parse(getColumn(Nodo.ChildNodes[0])), Reservada.ErrorSemantico, "IF incorrecto"));
                                        }
                                        return new Retorno2("-", "-", "0", "0");

                                    }
                                    else {
                                        Debug.WriteLine("IIIIIIIIIIIIIIIIIIIIF 0000001");
                                        Retorno ret3 = Cond(Nodo.ChildNodes[1]);

                                        if (ret3 != null)
                                        {
                                            Debug.WriteLine("Valor retornado del IF: " + ret3.Value);
                                            if (ret3.Value.Equals("True"))
                                            {
                                                Debug.WriteLine("El if es True");
                                                Retorno2 ret2 = Sentencias(Nodo.ChildNodes[4]);
                                                return ret2;
                                            }
                                            else
                                            {
                                                Debug.WriteLine("El if es False");
                                                Retorno2 ret2 = Sentencias(Nodo.ChildNodes[6]);
                                                return ret2;
                                            }
                                        }
                                        else
                                        {
                                            Debug.WriteLine("**Error Semantico** . IF incorrecto en la linea: " + getLine(Nodo.ChildNodes[0]) + " columna:" + getColumn(Nodo.ChildNodes[0]));
                                            error.Add(new Error(int.Parse(getLine(Nodo.ChildNodes[0])), int.Parse(getColumn(Nodo.ChildNodes[0])), Reservada.ErrorSemantico, "IF incorrecto"));
                                        }
                                        return new Retorno2("-", "-", "0", "0");
                                    }

                                case "while":
                                    Debug.WriteLine("WHILE 0000001");
                                    bool flag2 = true;
                                    while (flag2)
                                    {
                                        Retorno retW = Cond(Nodo.ChildNodes[1]);
                                        Debug.WriteLine("El WHILE es: " + retW.Value);

                                        if (retW != null)
                                        {
                                            if (retW.Value.Equals("True"))
                                            {
                                                Debug.WriteLine("El WHILE es True");
                                                Retorno2 ret2 = Sentencias(Nodo.ChildNodes[4]);
                                            }
                                            else
                                            {
                                                flag2 = false;
                                            }
                                        }
                                        else
                                        {
                                            Debug.WriteLine("**Error Semantico** . WHILE incorrecto en la linea: " + getLine(Nodo.ChildNodes[0]) + " columna:" + getColumn(Nodo.ChildNodes[0]));
                                            error.Add(new Error(int.Parse(getLine(Nodo.ChildNodes[0])), int.Parse(getColumn(Nodo.ChildNodes[0])), Reservada.ErrorSemantico, "WHILE incorrecto"));
                                        }
                                    }

                                    return new Retorno2("-", "-", "0", "0");
                            }
                            #endregion
                            break;

                        case 8:
                            #region
                            switch (Nodo.ChildNodes[0].Term.Name)
                            {
                                /*case "if":
                                    Debug.WriteLine("IIIIIIIIIIIIIIIIIIIIF 0");
                                    Retorno ret = Cond(Nodo.ChildNodes[1]);

                                    if (ret != null)
                                    {
                                        Debug.WriteLine("Valor retornado del IF: " + ret.Value);
                                        if (ret.Value.Equals("True"))
                                        {
                                            Debug.WriteLine("El if es True");
                                            Retorno2 ret2 = Sentencias(Nodo.ChildNodes[4]);
                                            return ret2;
                                        }
                                        else
                                        {
                                            Debug.WriteLine("El if es False");
                                            Retorno2 ret2 = Sentencias(Nodo.ChildNodes[7]);
                                            return ret2;
                                        }
                                    }
                                    else
                                    {
                                        Debug.WriteLine("**Error Semantico** . IF incorrecto en la linea: " + getLine(Nodo.ChildNodes[0]) + " columna:" + getColumn(Nodo.ChildNodes[0]));
                                        error.Add(new Error(int.Parse(getLine(Nodo.ChildNodes[0])), int.Parse(getColumn(Nodo.ChildNodes[0])), Reservada.ErrorSemantico, "IF incorrecto"));
                                    }
                                    return new Retorno2("-", "-", "0", "0");*/

                                case "if":
                                    Debug.WriteLine("IIIIIIIIIIIIIIIIIIIIF 0");
                                    Retorno ret = Cond(Nodo.ChildNodes[1]);

                                    if (ret != null)
                                    {
                                        Debug.WriteLine("Valor retornado del IF: " + ret.Value);
                                        if (ret.Value.Equals("True"))
                                        {
                                            Debug.WriteLine("El if es True");
                                            Retorno2 ret2 = Sentencias(Nodo.ChildNodes[4]);
                                            return ret2;
                                        }
                                        else
                                        {
                                            Debug.WriteLine("El if es False");
                                            Retorno2 ret2 = Sentencias(Nodo.ChildNodes[6]);
                                            return ret2;
                                        }
                                    }
                                    else
                                    {
                                        Debug.WriteLine("**Error Semantico** . IF incorrecto en la linea: " + getLine(Nodo.ChildNodes[0]) + " columna:" + getColumn(Nodo.ChildNodes[0]));
                                        error.Add(new Error(int.Parse(getLine(Nodo.ChildNodes[0])), int.Parse(getColumn(Nodo.ChildNodes[0])), Reservada.ErrorSemantico, "IF incorrecto"));
                                    }
                                    return new Retorno2("-", "-", "0", "0");


                                case "else":
                                    Debug.WriteLine("IIIIIIIIIIIIIIIIIIIIF 000000002");
                                    Retorno ret3 = Cond(Nodo.ChildNodes[2]);
                                    if (Nodo.ChildNodes[7].Equals(";"))
                                    {
                                        if (ret3 != null)
                                        {
                                            Debug.WriteLine("Valor retornado del IF: " + ret3.Value);
                                            if (ret3.Value.Equals("True"))
                                            {
                                                Debug.WriteLine("El if es True");
                                                Retorno2 ret2 = Sentencias(Nodo.ChildNodes[5]);
                                                return ret2;
                                            }
                                        }
                                        else
                                        {
                                            Debug.WriteLine("**Error Semantico** . IF incorrecto en la linea: " + getLine(Nodo.ChildNodes[0]) + " columna:" + getColumn(Nodo.ChildNodes[0]));
                                            error.Add(new Error(int.Parse(getLine(Nodo.ChildNodes[0])), int.Parse(getColumn(Nodo.ChildNodes[0])), Reservada.ErrorSemantico, "IF incorrecto"));
                                        }
                                        return new Retorno2("-", "-", "0", "0");
                                    }
                                    else
                                    {

                                        if (ret3 != null)
                                        {
                                            Debug.WriteLine("Valor retornado del IF: " + ret3.Value);
                                            if (ret3.Value.Equals("True"))
                                            {
                                                Debug.WriteLine("El if es True");
                                                Retorno2 ret2 = Sentencias(Nodo.ChildNodes[5]);
                                                return ret2;
                                            }
                                            else
                                            {
                                                Debug.WriteLine("El if es False");
                                                Retorno2 ret2 = Sentencias(Nodo.ChildNodes[7]);
                                                return ret2;
                                            }
                                        }
                                        else
                                        {
                                            Debug.WriteLine("**Error Semantico** . IF incorrecto en la linea: " + getLine(Nodo.ChildNodes[0]) + " columna:" + getColumn(Nodo.ChildNodes[0]));
                                            error.Add(new Error(int.Parse(getLine(Nodo.ChildNodes[0])), int.Parse(getColumn(Nodo.ChildNodes[0])), Reservada.ErrorSemantico, "IF incorrecto"));
                                        }
                                        return new Retorno2("-", "-", "0", "0");
                                    }
                            }
                            #endregion
                            break;

                        case 10:
                            #region
                            switch (Nodo.ChildNodes[0].Term.Name)
                            {
                                case "if":
                                    Debug.WriteLine("IIIIIIIIIIIIIIIIIIIIF 1");
                                    return new Retorno2("-", "-", "0", "0");
                            }
                            #endregion
                            break;

                        case 11:
                            #region
                            switch (Nodo.ChildNodes[0].Term.Name)
                            {
                                case "if":
                                    Debug.WriteLine("IIIIIIIIIIIIIIIIIIIIF 2:");
                                    Retorno ret = Cond(Nodo.ChildNodes[1]);

                                    if (ret != null)
                                    {
                                        Debug.WriteLine("Valor retornado del IF: " + ret.Value);
                                        if (ret.Value.Equals("True"))
                                        {
                                            Debug.WriteLine("El if es True");
                                            Retorno2 ret2 = Sentencias(Nodo.ChildNodes[4]);
                                            return ret2;
                                        }
                                        else
                                        {
                                            Debug.WriteLine("El if es False");
                                            Retorno2 ret2 = Sentencias(Nodo.ChildNodes[8]);
                                            return ret2;
                                        }
                                    }
                                    else
                                    {
                                        Debug.WriteLine("**Error Semantico** . IF incorrecto en la linea: " + getLine(Nodo.ChildNodes[0]) + " columna:" + getColumn(Nodo.ChildNodes[0]));
                                        error.Add(new Error(int.Parse(getLine(Nodo.ChildNodes[0])), int.Parse(getColumn(Nodo.ChildNodes[0])), Reservada.ErrorSemantico, "IF incorrecto"));
                                    }
                                    return new Retorno2("-", "-", "0", "0");

                                case "for":
                                    Debug.WriteLine("FOR 0000001");

                                    //ASignación
                                    string id4 = Nodo.ChildNodes[1].Token.Value.ToString();
                                    Simbolo var = RetornarSimbolo(id4);
                                    Debug.WriteLine("ID del for es: "+ var.Valor);

                                    if (var != null)
                                    {
                                        Retorno ret2 = Cond(Nodo.ChildNodes[3]);
                                        Debug.WriteLine("COND del for es: " + ret2.Value);

                                        if (ret2 != null)
                                        {
                                            if (ret2.Type.Equals(var.Tipo))
                                            {
                                                Debug.WriteLine("Se asigno variable: " + id4 + ": " + ret2.Value + " (" + ret2.Type + ")");
                                                var.Valor = ret2.Value;
                                            }
                                            else
                                            {
                                                Debug.WriteLine("**Error Semantico** Asignación inválida. Tipo de dato incorrecto en la linea: " + getLine(Nodo.ChildNodes[0]) + " columna:" + getColumn(Nodo.ChildNodes[0]));
                                                error.Add(new Error(int.Parse(getLine(Nodo.ChildNodes[0])), int.Parse(getColumn(Nodo.ChildNodes[0])), Reservada.ErrorSemantico, "Asignación inválida, tipo de dato incorrecto"));
                                            }
                                        }
                                        else
                                        {
                                            Debug.WriteLine("**Error Semantico** Asignación inválida. Expresión incorrecta en la linea: " + getLine(Nodo.ChildNodes[0]) + " columna:" + getColumn(Nodo.ChildNodes[0]));
                                            error.Add(new Error(int.Parse(getLine(Nodo.ChildNodes[0])), int.Parse(getColumn(Nodo.ChildNodes[0])), Reservada.ErrorSemantico, "Asignación inválida. Expresión incorrecta"));
                                        }
                                    }
                                    else
                                    {
                                        Debug.WriteLine("**Error Semantico** Variable: " + id4 + " no existe en la linea: " + getLine(Nodo.ChildNodes[0]) + " columna:" + getColumn(Nodo.ChildNodes[0]));
                                        error.Add(new Error(int.Parse(getLine(Nodo.ChildNodes[0])), int.Parse(getColumn(Nodo.ChildNodes[0])), Reservada.ErrorSemantico, "Variable: " + id4 + " no existe."));
                                    }


                                    //----
                                    bool flag2 = true;
                                    Retorno retW = Cond(Nodo.ChildNodes[5]);
                                    while (flag2)
                                    {
                                        Debug.WriteLine("El FOR es: " + retW.Value);

                                        if (retW != null)
                                        {
                                            if (int.Parse(var.Valor) <= int.Parse(retW.Value))
                                            {
                                                Debug.WriteLine("El FOR es True");
                                                Retorno2 ret2 = Sentencias(Nodo.ChildNodes[8]);
                                                var.Valor = (int.Parse(var.Valor) + 1) + "";
                                            }
                                            else
                                            {
                                                flag2 = false;
                                            }
                                        }
                                        else
                                        {
                                            Debug.WriteLine("**Error Semantico** . FOR incorrecto en la linea: " + getLine(Nodo.ChildNodes[0]) + " columna:" + getColumn(Nodo.ChildNodes[0]));
                                            error.Add(new Error(int.Parse(getLine(Nodo.ChildNodes[0])), int.Parse(getColumn(Nodo.ChildNodes[0])), Reservada.ErrorSemantico, "FOR incorrecto"));
                                        }
                                    }

                                    return new Retorno2("-", "-", "0", "0");

                                case "case":/*
                                    Debug.WriteLine("CASEEEEEEEEE 1:");
                                    Retorno retC = Cond(Nodo.ChildNodes[1]);

                                    if (retC != null)
                                    {
                                        Debug.WriteLine("Valor retornado del CASE: " + retC.Value);
                                        if (retC.Value.Equals("True"))
                                        {
                                            Debug.WriteLine("El if es True");
                                            Retorno2 ret2 = Sentencias(Nodo.ChildNodes[4]);
                                            return ret2;
                                        }
                                        else
                                        {
                                            Debug.WriteLine("El if es False");
                                            Retorno2 ret2 = Sentencias(Nodo.ChildNodes[8]);
                                            return ret2;
                                        }
                                    }
                                    else
                                    {
                                        Debug.WriteLine("**Error Semantico** . CASE incorrecto en la linea: " + getLine(Nodo.ChildNodes[0]) + " columna:" + getColumn(Nodo.ChildNodes[0]));
                                        error.Add(new Error(int.Parse(getLine(Nodo.ChildNodes[0])), int.Parse(getColumn(Nodo.ChildNodes[0])), Reservada.ErrorSemantico, "CASE incorrecto"));
                                    }*/
                                    return new Retorno2("-", "-", "0", "0");
                            }
                            #endregion
                            break;

                    }
                    #endregion
                    break;
            }
            return new Retorno2("-", "-", "0", "0");
        }

        private Retorno Cond(ParseTreeNode Nodo)
        {
            if (Nodo.ChildNodes.Count == 3)
            {
                Retorno ret1 = Cond(Nodo.ChildNodes[0]);
                Retorno ret2 = Cond(Nodo.ChildNodes[2]);

                switch (Nodo.ChildNodes[0].Term.Name)
                {
                    case "cond": // AND
                        #region
                        if ((ret1 != null) && (ret2 != null))
                        {
                            if (ret1.Type.Equals(Reservada.Booleano) && ret2.Type.Equals(Reservada.Booleano))
                            {
                                if (ret1.Value.Equals("True"))
                                {
                                    //flag = false;
                                }
                                if ((ret1.Value.Equals("True") && ret2.Value.Equals("True")) ||
                                    ret1.Value.Equals("true") && ret2.Value.Equals("true"))
                                {
                                    return new Retorno(Reservada.Booleano, "True", getLine(Nodo.ChildNodes[1]), getColumn(Nodo.ChildNodes[1]));
                                }
                                else
                                {
                                    return new Retorno(Reservada.Booleano, "False", getLine(Nodo.ChildNodes[1]), getColumn(Nodo.ChildNodes[1]));
                                }
                            }
                            else
                            {
                                Debug.WriteLine("***Error Semantico*** Ambas condiciones en AND no son booleanas en la linea:" +
                                    getLine(Nodo.ChildNodes[1]) + " columna:" + getColumn(Nodo.ChildNodes[1]));
                                error.Add(new Error(int.Parse(getLine(Nodo.ChildNodes[1])), int.Parse(getColumn(Nodo.ChildNodes[1])), Reservada.ErrorSemantico, "Imposible evaluar condicion AND con valores no booleanos"));
                                return null;
                            }
                        }
                        else
                        {
                            Debug.WriteLine("***Error Semantico*** Alguna condición en AND trae Null en la linea:" +
                                getLine(Nodo.ChildNodes[1]) + " columna:" + getColumn(Nodo.ChildNodes[1]));
                            error.Add(new Error(int.Parse(getLine(Nodo.ChildNodes[1])), int.Parse(getColumn(Nodo.ChildNodes[1])), Reservada.ErrorSemantico, "Imposible evaluar condicion AND, alguna condición trae null"));
                            return null;
                        }
                    #endregion

                    case "cond1": // OR
                        #region
                        if ((ret1 != null) && (ret2 != null))
                        {
                            if (ret1.Type.Equals(Reservada.Booleano) && ret2.Type.Equals(Reservada.Booleano))
                            {
                                if (ret2.Value.Equals("False") || ret2.Value.Equals("false"))
                                {
                                    //flag = false;
                                }
                                if (ret1.Value.Equals("False") && ret2.Value.Equals("False") ||
                                    ret1.Value.Equals("false") && ret2.Value.Equals("false"))
                                {
                                    return new Retorno(Reservada.Booleano, "False", getLine(Nodo.ChildNodes[1]), getColumn(Nodo.ChildNodes[1]));
                                }
                                else
                                {
                                    return new Retorno(Reservada.Booleano, "True", getLine(Nodo.ChildNodes[1]), getColumn(Nodo.ChildNodes[1]));
                                }
                            }
                            else
                            {
                                Debug.WriteLine("***Error Semantico*** Imposible evaluar la condición OR con valores no booleanos en la linea:" +
                                    getLine(Nodo.ChildNodes[1]) + " columna:" + getColumn(Nodo.ChildNodes[1]));
                                error.Add(new Error(int.Parse(getLine(Nodo.ChildNodes[1])), int.Parse(getColumn(Nodo.ChildNodes[1])), Reservada.ErrorSemantico, "Imposible evaluar condicion OR con valores no booleanos"));
                                return null;
                            }
                        }
                        else
                        {
                            Debug.WriteLine("***Error Semantico*** Alguna condición en OR trae Null en la linea:" +
                                getLine(Nodo.ChildNodes[1]) + " columna:" + getColumn(Nodo.ChildNodes[1]));
                            error.Add(new Error(int.Parse(getLine(Nodo.ChildNodes[1])), int.Parse(getColumn(Nodo.ChildNodes[1])), Reservada.ErrorSemantico, "Imposible evaluar condicion OR, alguna condición trae null"));
                            return null;
                        }
                    #endregion

                    case "cond3": // <=
                        #region
                        if ((ret1 != null) && (ret2 != null))
                        {
                            if (ret1.Type.Equals("False"))
                            {
                                //flag = false;
                            }
                            if ((ret1.Type.Equals(Reservada.Entero) && ret2.Type.Equals(Reservada.Real)) ||
                                     (ret1.Type.Equals(Reservada.Real) && ret2.Type.Equals(Reservada.Entero)) ||
                                         (ret1.Type.Equals(Reservada.Entero) && ret2.Type.Equals(Reservada.Entero)) ||
                                             (ret1.Type.Equals(Reservada.Real) && ret2.Type.Equals(Reservada.Real)))
                            {
                                double val1 = double.Parse(ret1.Value);
                                double val2 = double.Parse(ret2.Value);

                                if (val1 <= val2)
                                {
                                    return new Retorno(Reservada.Booleano, "True", getLine(Nodo.ChildNodes[1]), getColumn(Nodo.ChildNodes[1]));
                                }
                                else
                                {
                                    return new Retorno(Reservada.Booleano, "False", getLine(Nodo.ChildNodes[1]), getColumn(Nodo.ChildNodes[1]));
                                }
                            }
                            else if ((ret1.Type.Equals(Reservada.Stringg) && ret2.Type.Equals(Reservada.Stringg)))
                            {
                                int v1 = getCantVar(ret1.Value);
                                int v2 = getCantVar(ret2.Value);

                                if (v1 <= v2)
                                {
                                    return new Retorno(Reservada.Booleano, "True", getLine(Nodo.ChildNodes[1]), getColumn(Nodo.ChildNodes[1]));
                                }
                                else
                                {
                                    return new Retorno(Reservada.Booleano, "False", getLine(Nodo.ChildNodes[1]), getColumn(Nodo.ChildNodes[1]));
                                }
                            }
                            else
                            {
                                Debug.WriteLine("***Error Semantico*** condiciones incompatibles en <= en la linea:" +
                                    getLine(Nodo.ChildNodes[1]) + " columna:" + getColumn(Nodo.ChildNodes[1]));
                                error.Add(new Error(int.Parse(getLine(Nodo.ChildNodes[1])), int.Parse(getColumn(Nodo.ChildNodes[1])), Reservada.ErrorSemantico, "Imposible evaluar condicion <=, son valores diferentes no válidos"));
                                return null;
                            }
                        }
                        else
                        {
                            Debug.WriteLine("***Error Semantico*** Alguna condición en <= trae Null en la linea:" +
                                getLine(Nodo.ChildNodes[1]) + " columna:" + getColumn(Nodo.ChildNodes[1]));
                            error.Add(new Error(int.Parse(getLine(Nodo.ChildNodes[1])), int.Parse(getColumn(Nodo.ChildNodes[1])), Reservada.ErrorSemantico, "Imposible evaluar condicion <=, alguna condición trae null"));
                            return null;
                        }
                    #endregion

                    case "cond4": // >=
                        #region
                        if ((ret1 != null) && (ret2 != null))
                        {
                            if (ret2.Type.Equals("False"))
                            {
                                //flag = false;
                            }
                            if ((ret1.Type.Equals(Reservada.Entero) && ret2.Type.Equals(Reservada.Real)) ||
                                     (ret1.Type.Equals(Reservada.Real) && ret2.Type.Equals(Reservada.Entero)) ||
                                         (ret1.Type.Equals(Reservada.Entero) && ret2.Type.Equals(Reservada.Entero)) ||
                                             (ret1.Type.Equals(Reservada.Real) && ret2.Type.Equals(Reservada.Real)))
                            {
                                double val1 = double.Parse(ret1.Value);
                                double val2 = double.Parse(ret2.Value);

                                if (val1 >= val2)
                                {
                                    return new Retorno(Reservada.Booleano, "True", getLine(Nodo.ChildNodes[1]), getColumn(Nodo.ChildNodes[1]));
                                }
                                else
                                {
                                    return new Retorno(Reservada.Booleano, "False", getLine(Nodo.ChildNodes[1]), getColumn(Nodo.ChildNodes[1]));
                                }
                            }
                            else if ((ret1.Type.Equals(Reservada.Stringg) && ret2.Type.Equals(Reservada.Stringg)))
                            {
                                int v1 = getCantVar(ret1.Value);
                                int v2 = getCantVar(ret2.Value);

                                if (v1 >= v2)
                                {
                                    return new Retorno(Reservada.Booleano, "True", getLine(Nodo.ChildNodes[1]), getColumn(Nodo.ChildNodes[1])); //retorno
                                }
                                else
                                {
                                    return new Retorno(Reservada.Booleano, "False", getLine(Nodo.ChildNodes[1]), getColumn(Nodo.ChildNodes[1])); //retorno false
                                }
                            }
                            else
                            {
                                Debug.WriteLine("***Error Semantico*** condiciones incompatibles en >= en la linea:" +
                                    getLine(Nodo.ChildNodes[1]) + " columna:" + getColumn(Nodo.ChildNodes[1]));
                                error.Add(new Error(int.Parse(getLine(Nodo.ChildNodes[1])), int.Parse(getColumn(Nodo.ChildNodes[1])), Reservada.ErrorSemantico, "Imposible evaluar condicion >=, son valores diferentes no válidos"));
                                return null;
                            }
                        }
                        else
                        {
                            Debug.WriteLine("***Error Semantico*** Alguna condición en >= trae Null en la linea:" +
                                getLine(Nodo.ChildNodes[1]) + " columna:" + getColumn(Nodo.ChildNodes[1]));
                            error.Add(new Error(int.Parse(getLine(Nodo.ChildNodes[1])), int.Parse(getColumn(Nodo.ChildNodes[1])), Reservada.ErrorSemantico, "Imposible evaluar condicion >=, alguna condición trae null"));
                            return null;
                        }
                    #endregion

                    case "cond5": //<
                        #region
                        if ((ret1 != null) && (ret2 != null))
                        {
                            if (ret1.Type.Equals("False"))
                            {
                                //flag = false;
                            }
                            if ((ret1.Type.Equals(Reservada.Entero) && ret2.Type.Equals(Reservada.Real)) ||
                                     (ret1.Type.Equals(Reservada.Real) && ret2.Type.Equals(Reservada.Entero)) ||
                                         (ret1.Type.Equals(Reservada.Entero) && ret2.Type.Equals(Reservada.Entero)) ||
                                             (ret1.Type.Equals(Reservada.Real) && ret2.Type.Equals(Reservada.Real)))
                            {
                                double val1 = double.Parse(ret1.Value);
                                double val2 = double.Parse(ret2.Value);

                                if (val1 < val2)
                                {
                                    return new Retorno(Reservada.Booleano, "True", getLine(Nodo.ChildNodes[1]), getColumn(Nodo.ChildNodes[1]));
                                }
                                else
                                {
                                    return new Retorno(Reservada.Booleano, "False", getLine(Nodo.ChildNodes[1]), getColumn(Nodo.ChildNodes[1]));
                                }
                            }
                            else if ((ret1.Type.Equals(Reservada.Stringg) && ret2.Type.Equals(Reservada.Stringg)))
                            {
                                int v1 = getCantVar(ret1.Value);
                                int v2 = getCantVar(ret2.Value);

                                if (v1 < v2)
                                {
                                    return new Retorno(Reservada.Booleano, "True", getLine(Nodo.ChildNodes[1]), getColumn(Nodo.ChildNodes[1]));
                                }
                                else
                                {
                                    return new Retorno(Reservada.Booleano, "False", getLine(Nodo.ChildNodes[1]), getColumn(Nodo.ChildNodes[1]));
                                }
                            }
                            else
                            {
                                Debug.WriteLine("***Error Semantico*** condiciones incompatibles en < en la linea:" +
                                    getLine(Nodo.ChildNodes[1]) + " columna:" + getColumn(Nodo.ChildNodes[1]));
                                error.Add(new Error(int.Parse(getLine(Nodo.ChildNodes[1])), int.Parse(getColumn(Nodo.ChildNodes[1])), Reservada.ErrorSemantico, "Imposible evaluar condicion <, son valores diferentes no válidos"));
                                return null;
                            }
                        }
                        else
                        {
                            Debug.WriteLine("***Error Semantico*** Alguna condición en < trae Null en la linea:" +
                                getLine(Nodo.ChildNodes[1]) + " columna:" + getColumn(Nodo.ChildNodes[1]));
                            error.Add(new Error(int.Parse(getLine(Nodo.ChildNodes[1])), int.Parse(getColumn(Nodo.ChildNodes[1])), Reservada.ErrorSemantico, "Imposible evaluar condicion <, alguna condición trae null"));
                            return null;
                        }
                    #endregion

                    case "cond6": // >
                        #region
                        if ((ret1 != null) && (ret2 != null))
                        {
                            if (ret1.Type.Equals("False"))
                            {
                                //flag = false;
                            }
                            if ((ret1.Type.Equals(Reservada.Entero) && ret2.Type.Equals(Reservada.Real)) ||
                                     (ret1.Type.Equals(Reservada.Real) && ret2.Type.Equals(Reservada.Entero)) ||
                                         (ret1.Type.Equals(Reservada.Entero) && ret2.Type.Equals(Reservada.Entero)) ||
                                             (ret1.Type.Equals(Reservada.Real) && ret2.Type.Equals(Reservada.Real)))
                            {
                                double val1 = double.Parse(ret1.Value);
                                double val2 = double.Parse(ret2.Value);

                                if (val1 > val2)
                                {
                                    return new Retorno(Reservada.Booleano, "True", getLine(Nodo.ChildNodes[1]), getColumn(Nodo.ChildNodes[1]));
                                }
                                else
                                {
                                    return new Retorno(Reservada.Booleano, "False", getLine(Nodo.ChildNodes[1]), getColumn(Nodo.ChildNodes[1]));
                                }
                            }
                            else if ((ret1.Type.Equals(Reservada.Stringg) && ret2.Type.Equals(Reservada.Stringg)))
                            {
                                int v1 = getCantVar(ret1.Value);
                                int v2 = getCantVar(ret2.Value);

                                if (v1 > v2)
                                {
                                    return new Retorno(Reservada.Booleano, "True", getLine(Nodo.ChildNodes[1]), getColumn(Nodo.ChildNodes[1]));
                                }
                                else
                                {
                                    return new Retorno(Reservada.Booleano, "False", getLine(Nodo.ChildNodes[1]), getColumn(Nodo.ChildNodes[1]));
                                }
                            }
                            else
                            {
                                Debug.WriteLine("***Error Semantico*** condiciones incompatibles en > en la linea:" +
                                    getLine(Nodo.ChildNodes[1]) + " columna:" + getColumn(Nodo.ChildNodes[1]));
                                error.Add(new Error(int.Parse(getLine(Nodo.ChildNodes[1])), int.Parse(getColumn(Nodo.ChildNodes[1])), Reservada.ErrorSemantico, "Imposible evaluar condicion >, son valores diferentes no válidos"));
                                return null;
                            }
                        }
                        else
                        {
                            Debug.WriteLine("***Error Semantico*** Alguna condición en > trae Null en la linea:" +
                                getLine(Nodo.ChildNodes[1]) + " columna:" + getColumn(Nodo.ChildNodes[1]));
                            error.Add(new Error(int.Parse(getLine(Nodo.ChildNodes[1])), int.Parse(getColumn(Nodo.ChildNodes[1])), Reservada.ErrorSemantico, "Imposible evaluar condicion >, alguna condición trae null"));
                            return null;
                        }
                    #endregion

                    case "cond7": // =
                        #region
                        if ((ret1 != null) && (ret2 != null))
                        {
                            if (ret1.Type.Equals("False"))
                            {
                                //flag = false;
                            }
                            if ((ret1.Type.Equals(Reservada.Entero) && ret2.Type.Equals(Reservada.Real)) ||
                                     (ret1.Type.Equals(Reservada.Real) && ret2.Type.Equals(Reservada.Entero)) ||
                                         (ret1.Type.Equals(Reservada.Entero) && ret2.Type.Equals(Reservada.Entero)) ||
                                             (ret1.Type.Equals(Reservada.Real) && ret2.Type.Equals(Reservada.Real)))
                            {
                                double val1 = double.Parse(ret1.Value);
                                double val2 = double.Parse(ret2.Value);

                                if (val1 == val2)
                                {
                                    return new Retorno(Reservada.Booleano, "True", getLine(Nodo.ChildNodes[1]), getColumn(Nodo.ChildNodes[1]));
                                }
                                else
                                {
                                    return new Retorno(Reservada.Booleano, "False", getLine(Nodo.ChildNodes[1]), getColumn(Nodo.ChildNodes[1]));
                                }
                            }
                            else if ((ret1.Type.Equals(Reservada.Stringg) && ret2.Type.Equals(Reservada.Stringg)) ||
                                    (ret1.Type.Equals(Reservada.Booleano) && ret2.Type.Equals(Reservada.Booleano)))
                            {
                                int v1 = getCantVar(ret1.Value);
                                int v2 = getCantVar(ret2.Value);

                                if (v1 == v2)
                                {
                                    return new Retorno(Reservada.Booleano, "True", getLine(Nodo.ChildNodes[1]), getColumn(Nodo.ChildNodes[1]));
                                }
                                else
                                {
                                    return new Retorno(Reservada.Booleano, "False", getLine(Nodo.ChildNodes[1]), getColumn(Nodo.ChildNodes[1]));
                                }
                            }
                            else
                            {
                                Debug.WriteLine("***Error Semantico*** condiciones incompatibles en = en la linea:" +
                                    getLine(Nodo.ChildNodes[1]) + " columna:" + getColumn(Nodo.ChildNodes[1]));
                                error.Add(new Error(int.Parse(getLine(Nodo.ChildNodes[1])), int.Parse(getColumn(Nodo.ChildNodes[1])), Reservada.ErrorSemantico, "Imposible evaluar condicion =, son valores diferentes no válidos"));
                                return null;
                            }
                        }
                        else
                        {
                            Debug.WriteLine("***Error Semantico*** Alguna condición en = trae Null en la linea:" +
                                getLine(Nodo.ChildNodes[1]) + " columna:" + getColumn(Nodo.ChildNodes[1]));
                            error.Add(new Error(int.Parse(getLine(Nodo.ChildNodes[1])), int.Parse(getColumn(Nodo.ChildNodes[1])), Reservada.ErrorSemantico, "Imposible evaluar condicion =, alguna condición trae null"));
                            return null;
                        }
                    #endregion

                    case "cond8": // <>
                        #region
                        Retorno reto2 = Exp(Nodo.ChildNodes[2]);

                        if ((ret1 != null) && (reto2 != null)) // Si ambos son distintos de null entra
                        {
                            if (ret1.Type.Equals("False"))
                            {
                                //flag = false;
                            }
                            if ((ret1.Type.Equals(Reservada.Entero) && reto2.Type.Equals(Reservada.Real)) ||
                                     (ret1.Type.Equals(Reservada.Real) && reto2.Type.Equals(Reservada.Entero)) ||
                                         (ret1.Type.Equals(Reservada.Entero) && reto2.Type.Equals(Reservada.Entero)) ||
                                             (ret1.Type.Equals(Reservada.Real) && reto2.Type.Equals(Reservada.Real)))
                            {
                                double val1 = double.Parse(ret1.Value);
                                double val2 = double.Parse(reto2.Value);

                                if (val1 != val2)
                                {
                                    return new Retorno(Reservada.Booleano, "True", getLine(Nodo.ChildNodes[1]), getColumn(Nodo.ChildNodes[1])); //retorno true
                                }
                                else
                                {
                                    return new Retorno(Reservada.Booleano, "False", getLine(Nodo.ChildNodes[1]), getColumn(Nodo.ChildNodes[1])); //retorno True
                                }
                            }
                            else if ((ret1.Type.Equals(Reservada.Stringg) && reto2.Type.Equals(Reservada.Stringg)) ||      //Si ambos son String
                                    (ret1.Type.Equals(Reservada.Booleano) && reto2.Type.Equals(Reservada.Booleano)))     //Si ambos son Boolean
                            {
                                int v1 = getCantVar(ret1.Value);
                                int v2 = getCantVar(reto2.Value);

                                if (v1 != v2)
                                {
                                    return new Retorno(Reservada.Booleano, "True", getLine(Nodo.ChildNodes[1]), getColumn(Nodo.ChildNodes[1])); //retorno true
                                }
                                else
                                {
                                    return new Retorno(Reservada.Booleano, "False", getLine(Nodo.ChildNodes[1]), getColumn(Nodo.ChildNodes[1])); //retorno false
                                }
                            }
                            else // valores no numericos
                            {
                                Debug.WriteLine("***Error Semantico*** condiciones incompatibles en <> en la linea:" +
                                    getLine(Nodo.ChildNodes[1]) + " columna:" + getColumn(Nodo.ChildNodes[1]));
                                error.Add(new Error(int.Parse(getLine(Nodo.ChildNodes[1])), int.Parse(getColumn(Nodo.ChildNodes[1])), Reservada.ErrorSemantico, "Imposible evaluar condicion <>, son valores diferentes no válidos"));
                                return null;
                            }
                        }
                        else
                        {
                            Debug.WriteLine("***Error Semantico*** Alguna condición en <> trae Null en la linea:" +
                                getLine(Nodo.ChildNodes[1]) + " columna:" + getColumn(Nodo.ChildNodes[1]));
                            error.Add(new Error(int.Parse(getLine(Nodo.ChildNodes[1])), int.Parse(getColumn(Nodo.ChildNodes[1])), Reservada.ErrorSemantico, "Imposible evaluar condicion <>, alguna condición trae null"));
                            return null;
                        }
                        #endregion

                }
            }
            // ToTerm("not") + COND3
            else if (Nodo.ChildNodes.Count == 2)
            {
                #region
                Retorno condB3 = Cond(Nodo.ChildNodes[1]);

                if (condB3 != null)
                {
                    if (condB3.Type.Equals(Reservada.Booleano))
                    {
                        Debug.WriteLine("**********************El valor booleano es: {0}", condB3);
                        if (condB3.Value.Equals("True") || condB3.Value.Equals("true"))
                        {
                            return new Retorno(Reservada.Booleano, "False", getLine(Nodo.ChildNodes[0]), getColumn(Nodo.ChildNodes[0]));
                        }
                        else
                        {
                            return new Retorno(Reservada.Booleano, "True", getLine(Nodo.ChildNodes[0]), getColumn(Nodo.ChildNodes[0]));
                        }
                    }
                    else
                    {
                        Debug.WriteLine("***Error Semantico*** condiciones incompatibles en NOT en la linea:" +
                            getLine(Nodo.ChildNodes[0]) + " columna:" + getColumn(Nodo.ChildNodes[0]));
                        error.Add(new Error(int.Parse(getLine(Nodo.ChildNodes[0])), int.Parse(getColumn(Nodo.ChildNodes[0])), Reservada.ErrorSemantico, "Imposible evaluar condicion NOT, son valores diferentes no válidos"));
                        return null;
                    }
                }
                else
                {
                    Debug.WriteLine("***Error Semantico*** Alguna condición en NOT trae Null en la linea:" +
                        getLine(Nodo.ChildNodes[0]) + " columna:" + getColumn(Nodo.ChildNodes[0]));
                    error.Add(new Error(int.Parse(getLine(Nodo.ChildNodes[0])), int.Parse(getColumn(Nodo.ChildNodes[0])), Reservada.ErrorSemantico, "Imposible evaluar condicion NOT, alguna condición trae null"));
                    return null;
                }
                #endregion
            }
            else if (Nodo.ChildNodes.Count == 1)
            {
                if (Nodo.ChildNodes[0].Term.Name.Equals("exp"))
                {
                    return Exp(Nodo.ChildNodes[0]);
                }
                else // recursivo con las condiciones
                {
                    return Cond(Nodo.ChildNodes[0]);
                }
            }
            return null;
        }

        private Retorno Exp(ParseTreeNode Nodo)
        {
            if (Nodo.ChildNodes.Count == 3)
            {
                Retorno ret1 = Exp(Nodo.ChildNodes[0]);
                Retorno ret2 = Exp(Nodo.ChildNodes[2]);

                switch (Nodo.ChildNodes[0].Term.Name)
                {
                    case "exp": // +
                        #region
                        if ((ret1 != null) && (ret2 != null))
                        {
                            if (ret1.Type.Equals(ret2.Type) && !ret1.Type.Equals(Reservada.Stringg))
                            {
                                double suma = double.Parse(Conv(ret1).Value) + double.Parse(Conv(ret2).Value);

                                if (ret1.Type.Equals(Reservada.Booleano))
                                {
                                    return new Retorno(Reservada.Booleano, suma + "", getLine(Nodo.ChildNodes[1]), getColumn(Nodo.ChildNodes[1]));
                                }
                                else
                                {
                                    return new Retorno(ret1.Type, suma + "", getLine(Nodo.ChildNodes[1]), getColumn(Nodo.ChildNodes[1]));
                                }
                            }
                            else if (ret1.Type.Equals(Reservada.Stringg) || ret2.Type.Equals(Reservada.Stringg))
                            {
                                string concat = Conv(ret1).Value + Conv(ret2).Value;
                                return new Retorno(Reservada.Stringg, concat, getLine(Nodo.ChildNodes[1]), getColumn(Nodo.ChildNodes[1]));
                            }
                            else if ((ret1.Type.Equals(Reservada.Booleano) && ret2.Type.Equals(Reservada.Real)) || (ret1.Type.Equals(Reservada.Real) && ret2.Type.Equals(Reservada.Booleano))
                                        || (ret1.Type.Equals(Reservada.Real) && ret2.Type.Equals(Reservada.Entero)) || (ret1.Type.Equals(Reservada.Entero) && ret2.Type.Equals(Reservada.Real))
                                        || (ret1.Type.Equals(Reservada.Real) && ret2.Type.Equals(Reservada.Entero)) || (ret1.Type.Equals(Reservada.Entero) && ret2.Type.Equals(Reservada.Real)))
                            {
                                double suma = double.Parse(Conv(ret1).Value) + double.Parse(Conv(ret2).Value);
                                return new Retorno(Reservada.Real, suma + "", getLine(Nodo.ChildNodes[1]), getColumn(Nodo.ChildNodes[1]));
                            }
                            else if ((ret1.Type.Equals(Reservada.Booleano) && ret2.Type.Equals(Reservada.Entero)) || (ret1.Type.Equals(Reservada.Entero) && ret2.Type.Equals(Reservada.Booleano)))
                            {
                                double suma = double.Parse(Conv(ret1).Value) + double.Parse(Conv(ret2).Value);
                                return new Retorno(Reservada.Entero, suma + "", getLine(Nodo.ChildNodes[1]), getColumn(Nodo.ChildNodes[1]));
                            }
                            else
                            {
                                Debug.WriteLine("***Error Semantico*** No se puede operar la SUMA en la línea: " +
                                    getLine(Nodo.ChildNodes[1]) + " columna:" + getColumn(Nodo.ChildNodes[1]));
                                error.Add(new Error(int.Parse(getLine(Nodo.ChildNodes[1])), int.Parse(getColumn(Nodo.ChildNodes[1])), Reservada.ErrorSemantico, "No se puede operar la SUMA"));
                                return null;
                            }
                        }
                        else
                        {
                            Debug.WriteLine("***Error Semantico*** No se puede operar la SUMA porque trae nulos: " +
                                getLine(Nodo.ChildNodes[1]) + " columna:" + getColumn(Nodo.ChildNodes[1]));
                            error.Add(new Error(int.Parse(getLine(Nodo.ChildNodes[1])), int.Parse(getColumn(Nodo.ChildNodes[1])), Reservada.ErrorSemantico, "No se puede operar la SUMA porque trae nulos"));
                            return null;
                        }
                    #endregion

                    case "exp1": // -
                        #region
                        if ((ret1 != null) && (ret2 != null)) // Si ambos son distintos de null entra
                        {
                            if ((ret1.Type.Equals(Reservada.Entero) && ret2.Type.Equals(Reservada.Entero))
                                   || (ret1.Type.Equals(Reservada.Booleano) && ret2.Type.Equals(Reservada.Entero))
                                   || (ret1.Type.Equals(Reservada.Entero) && ret2.Type.Equals(Reservada.Booleano)))
                            {
                                double resta = double.Parse(Conv(ret1).Value) - double.Parse(Conv(ret2).Value);
                                return new Retorno(Reservada.Entero, resta + "", getLine(Nodo.ChildNodes[1]), getColumn(Nodo.ChildNodes[1]));
                            }
                            else if ((ret1.Type.Equals(Reservada.Real) && ret2.Type.Equals(Reservada.Real))
                                || (ret1.Type.Equals(Reservada.Booleano) && ret2.Type.Equals(Reservada.Real))
                                || (ret1.Type.Equals(Reservada.Real) && ret2.Type.Equals(Reservada.Booleano))
                                || (ret1.Type.Equals(Reservada.Real) && ret2.Type.Equals(Reservada.Entero))
                                || (ret1.Type.Equals(Reservada.Entero) && ret2.Type.Equals(Reservada.Real)))
                            {
                                double resta = double.Parse(Conv(ret1).Value) - double.Parse(Conv(ret2).Value);
                                return new Retorno(Reservada.Real, resta + "", getLine(Nodo.ChildNodes[1]), getColumn(Nodo.ChildNodes[1]));
                            }
                            else
                            {
                                Debug.WriteLine("***Error Semantico*** No se puede operar la RESTA en la línea: " +
                                getLine(Nodo.ChildNodes[1]) + " columna:" + getColumn(Nodo.ChildNodes[1]));
                                error.Add(new Error(int.Parse(getLine(Nodo.ChildNodes[1])), int.Parse(getColumn(Nodo.ChildNodes[1])), Reservada.ErrorSemantico, "No se puede operar la RESTA"));
                                return null;
                            }
                        }
                        else
                        {
                            Debug.WriteLine("***Error Semantico*** No se puede operar la RESTA porque trae nulos en la línea: " +
                                getLine(Nodo.ChildNodes[1]) + " columna:" + getColumn(Nodo.ChildNodes[1]));
                            error.Add(new Error(int.Parse(getLine(Nodo.ChildNodes[1])), int.Parse(getColumn(Nodo.ChildNodes[1])), Reservada.ErrorSemantico, "No se puede operar la RESTA porque trae nulos"));
                            return null;
                        }
                    #endregion

                    case "exp2": // *
                        #region
                        if ((ret1 != null) && (ret2 != null)) // Si ambos son distintos de null entra
                        {
                            if ((ret1.Type.Equals(Reservada.Entero) && ret2.Type.Equals(Reservada.Entero))
                                || (ret1.Type.Equals(Reservada.Booleano) && ret2.Type.Equals(Reservada.Entero))
                                || (ret1.Type.Equals(Reservada.Entero) && ret2.Type.Equals(Reservada.Booleano)))
                            {
                                double mul = double.Parse(Conv(ret1).Value) * double.Parse(Conv(ret2).Value);
                                return new Retorno(Reservada.Entero, mul + "", getLine(Nodo.ChildNodes[1]), getColumn(Nodo.ChildNodes[1]));
                            }
                            else if ((ret1.Type.Equals(Reservada.Real) && ret2.Type.Equals(Reservada.Real))
                                || (ret1.Type.Equals(Reservada.Booleano) && ret2.Type.Equals(Reservada.Real))
                                || (ret1.Type.Equals(Reservada.Real) && ret2.Type.Equals(Reservada.Booleano))
                                || (ret1.Type.Equals(Reservada.Real) && ret2.Type.Equals(Reservada.Entero))
                                || (ret1.Type.Equals(Reservada.Entero) && ret2.Type.Equals(Reservada.Real)))
                            {
                                double mul = double.Parse(Conv(ret1).Value) * double.Parse(Conv(ret2).Value);
                                return new Retorno(Reservada.Real, mul + "", getLine(Nodo.ChildNodes[1]), getColumn(Nodo.ChildNodes[1]));
                            }
                            else
                            {
                                Debug.WriteLine("***Error Semantico*** No se puede operar la MULTIPLICACION en la línea: " +
                                getLine(Nodo.ChildNodes[1]) + " columna:" + getColumn(Nodo.ChildNodes[1]));
                                error.Add(new Error(int.Parse(getLine(Nodo.ChildNodes[1])), int.Parse(getColumn(Nodo.ChildNodes[1])), Reservada.ErrorSemantico, "No se puede operar la MULTIPLICACION."));
                                return null;
                            }
                        }
                        else
                        {
                            Debug.WriteLine("***Error Semantico*** No se puede operar la MULTIPLICACION porque trae nulos en la línea: " +
                                getLine(Nodo.ChildNodes[1]) + " columna:" + getColumn(Nodo.ChildNodes[1]));
                            error.Add(new Error(int.Parse(getLine(Nodo.ChildNodes[1])), int.Parse(getColumn(Nodo.ChildNodes[1])), Reservada.ErrorSemantico, "No se puede operar la MULTIPLICACION porque trae nulos"));
                            return null;
                        }
                    #endregion

                    case "exp3": // /
                        #region
                        if ((ret1 != null) && (ret2 != null))
                        {
                            if ((ret1.Type.Equals(Reservada.Real) && ret2.Type.Equals(Reservada.Real))
                                || (ret1.Type.Equals(Reservada.Booleano) && ret2.Type.Equals(Reservada.Real))
                                || (ret1.Type.Equals(Reservada.Real) && ret2.Type.Equals(Reservada.Booleano))
                                || (ret1.Type.Equals(Reservada.Real) && ret2.Type.Equals(Reservada.Entero))
                                || (ret1.Type.Equals(Reservada.Entero) && ret2.Type.Equals(Reservada.Real))
                                || (ret1.Type.Equals(Reservada.Entero) && ret2.Type.Equals(Reservada.Entero))
                                || (ret1.Type.Equals(Reservada.Booleano) && ret2.Type.Equals(Reservada.Entero))
                                || (ret1.Type.Equals(Reservada.Entero) && ret2.Type.Equals(Reservada.Booleano)))
                            {
                                double div = double.Parse(Conv(ret1).Value) / double.Parse(Conv(ret2).Value);
                                return new Retorno(Reservada.Real, div + "", getLine(Nodo.ChildNodes[1]), getColumn(Nodo.ChildNodes[1]));
                            }
                            else
                            {
                                Debug.WriteLine("***Error Semantico*** No se puede operar la DIVISIÓN en la línea: " +
                                    getLine(Nodo.ChildNodes[1]) + " columna:" + getColumn(Nodo.ChildNodes[1]));
                                error.Add(new Error(int.Parse(getLine(Nodo.ChildNodes[1])), int.Parse(getColumn(Nodo.ChildNodes[1])), Reservada.ErrorSemantico, "No se puede operar la DIVISIÓN."));
                                return null;
                            }
                        }
                        else
                        {
                            Debug.WriteLine("***Error Semantico*** No se puede operar la DIVISIÓN porque trae nulos en la línea: " +
                                getLine(Nodo.ChildNodes[1]) + " columna:" + getColumn(Nodo.ChildNodes[1]));
                            error.Add(new Error(int.Parse(getLine(Nodo.ChildNodes[1])), int.Parse(getColumn(Nodo.ChildNodes[1])), Reservada.ErrorSemantico, "No se puede operar la DIVISIÓN porque trae nulos"));
                            return null;
                        }
                    #endregion

                    case "exp4": // %
                        #region
                        ret2 = Termianl(Nodo.ChildNodes[2]);

                        if ((ret1 != null) && (ret2 != null))
                        {
                            if ((ret1.Type.Equals(Reservada.Real) && ret2.Type.Equals(Reservada.Real))
                                || (ret1.Type.Equals(Reservada.Booleano) && ret2.Type.Equals(Reservada.Real))
                                || (ret1.Type.Equals(Reservada.Real) && ret2.Type.Equals(Reservada.Booleano))
                                || (ret1.Type.Equals(Reservada.Real) && ret2.Type.Equals(Reservada.Entero))
                                || (ret1.Type.Equals(Reservada.Entero) && ret2.Type.Equals(Reservada.Real))
                                || (ret1.Type.Equals(Reservada.Entero) && ret2.Type.Equals(Reservada.Entero))
                                || (ret1.Type.Equals(Reservada.Booleano) && ret2.Type.Equals(Reservada.Entero))
                                || (ret1.Type.Equals(Reservada.Entero) && ret2.Type.Equals(Reservada.Booleano)))
                            {
                                double mod = double.Parse(Conv(ret1).Value) % double.Parse(Conv(ret2).Value);
                                return new Retorno(Reservada.Real, mod + "", getLine(Nodo.ChildNodes[1]), getColumn(Nodo.ChildNodes[1]));
                            }
                            else
                            {
                                Debug.WriteLine("***Error Semantico*** No se puede operar el MÓDULO en la línea: " +
                                    getLine(Nodo.ChildNodes[1]) + " columna:" + getColumn(Nodo.ChildNodes[1]));
                                error.Add(new Error(int.Parse(getLine(Nodo.ChildNodes[1])), int.Parse(getColumn(Nodo.ChildNodes[1])), Reservada.ErrorSemantico, "No se puede operar el MÓDULO."));
                                return null;
                            }
                        }
                        else
                        {
                            Debug.WriteLine("***Error Semantico*** No se puede operar el MÓDULO porque trae nulos en la línea: " +
                                getLine(Nodo.ChildNodes[1]) + " columna:" + getColumn(Nodo.ChildNodes[1]));
                            error.Add(new Error(int.Parse(getLine(Nodo.ChildNodes[1])), int.Parse(getColumn(Nodo.ChildNodes[1])), Reservada.ErrorSemantico, "No se puede operar el MÓDULO porque trae nulos"));
                            return null;
                        }
                        #endregion

                }
            }
            else if (Nodo.ChildNodes.Count == 1)
            {
                if (Nodo.ChildNodes[0].Term.Name.Equals("terminal"))
                {
                    return Termianl(Nodo.ChildNodes[0]);
                }
                else
                {   //Igual que las condiciones
                    return Exp(Nodo.ChildNodes[0]);
                }
            }
            return null;
        }

        private Retorno Termianl(ParseTreeNode Nodo)
        {
            switch (Nodo.ChildNodes.Count)
            {
                case 1:
                    #region
                    switch (Nodo.ChildNodes[0].Term.Name)
                    {
                        case "numero":
                            return new Retorno(Reservada.Entero, Nodo.ChildNodes[0].Token.Value.ToString(), getLine(Nodo.ChildNodes[0]), getColumn(Nodo.ChildNodes[0]));

                        case "real":
                            return new Retorno(Reservada.Real, Nodo.ChildNodes[0].Token.Value.ToString(), getLine(Nodo.ChildNodes[0]), getColumn(Nodo.ChildNodes[0]));

                        case "cadena":
                            return new Retorno(Reservada.Stringg, Nodo.ChildNodes[0].Token.Value.ToString(), getLine(Nodo.ChildNodes[0]), getColumn(Nodo.ChildNodes[0]));

                        case "true":
                            return new Retorno(Reservada.Booleano, Nodo.ChildNodes[0].Token.Value.ToString(), getLine(Nodo.ChildNodes[0]), getColumn(Nodo.ChildNodes[0]));

                        case "false":
                            return new Retorno(Reservada.Booleano, Nodo.ChildNodes[0].Token.Value.ToString(), getLine(Nodo.ChildNodes[0]), getColumn(Nodo.ChildNodes[0]));

                        case "id": //Esto puede ser una VARIABLE o un ARREGLO
                            string id = Nodo.ChildNodes[0].Token.Value.ToString();
                            Simbolo sim = RetornarSimbolo(id);
                            //Debug.WriteLine("Simbolo retornado: {0} , {1}", sim.Nombre, sim.Valor);

                            if (sim != null)
                            {
                                return new Retorno(sim.Tipo, sim.Valor, getLine(Nodo.ChildNodes[0]), getColumn(Nodo.ChildNodes[0]));
                            }
                            else
                            {
                                Debug.WriteLine("Error Semantico-->Variable " + id + " no Existente linea:" + getLine(Nodo.ChildNodes[0]) + " columna:" + getColumn(Nodo.ChildNodes[0]));
                                error.Add(new Error(int.Parse(getLine(Nodo.ChildNodes[0])), int.Parse(getColumn(Nodo.ChildNodes[0])), Reservada.ErrorSemantico, "Variable " + id + " no existente"));
                                return null;
                            }

                    }
                    #endregion
                    break;
                case 2:
                    #region
                    switch (Nodo.ChildNodes[1].Term.Name)
                    {
                        case "numero":
                            return new Retorno(Reservada.Entero, Nodo.ChildNodes[0].Term.Name + Nodo.ChildNodes[1].Token.Value.ToString(), getLine(Nodo.ChildNodes[0]), getColumn(Nodo.ChildNodes[0]));
                        case "real":
                            return new Retorno(Reservada.Real, Nodo.ChildNodes[0].Term.Name + Nodo.ChildNodes[1].Token.Value.ToString(), getLine(Nodo.ChildNodes[0]), getColumn(Nodo.ChildNodes[0]));
                    }
                    #endregion
                    break;
                case 3:
                    #region
                    switch (Nodo.ChildNodes[0].Term.Name)
                    {
                        case "id":
                            return null;

                        case "(":
                            Retorno ret = Cond(Nodo.ChildNodes[1]);

                            if (ret != null)
                            {
                                return new Retorno(ret.Type, ret.Value, getLine(Nodo.ChildNodes[0]), getColumn(Nodo.ChildNodes[0]));
                            }
                            else
                            {
                                Debug.WriteLine("***Error Semantico*** Error en los Paréntesis en la línea: " +
                                getLine(Nodo.ChildNodes[0]) + " columna:" + getColumn(Nodo.ChildNodes[0]));
                                error.Add(new Error(int.Parse(getLine(Nodo.ChildNodes[0])), int.Parse(getColumn(Nodo.ChildNodes[0])), Reservada.ErrorSemantico, "Error en el retorno de los paréntesis"));
                            }
                            break;

                    }
                    #endregion
                    break;
                case 4:
                    break;

                default:
                    return null;

            }
            return null;
        }

        private Retorno Conv(Retorno ret)
        {
            switch (ret.Type)
            {
                case "Char":
                    ret.Value = Encoding.ASCII.GetBytes(ret.Value)[0] + "";
                    return ret;

                case "Boolean":
                    if (ret.Value.Equals("True"))
                    {
                        ret.Value = "1";
                        return ret;
                    }
                    else
                    {
                        ret.Value = "0";
                        return ret;
                    }

                default:
                    return ret;
            }
        }

        private string getPrint(ParseTreeNode Nodo)
        {
            switch (Nodo.Term.Name)
            {
                case "list_cond":
                    switch (Nodo.ChildNodes.Count)
                    {
                        case 3:
                            return getPrint(Nodo.ChildNodes[0]) + getPrint(Nodo.ChildNodes[2]);

                        case 1:
                            return getPrint(Nodo.ChildNodes[0]);

                    }
                    break;
                case "cond":
                    Retorno ret = Cond(Nodo);
                    if (ret != null)
                    {
                        return ret.Value + "";
                    }
                    return "";
                default:
                    return "";
            }
            return "";
        }

        private void PdfTS()
        {
            try
            {
                int no = 1;
                StreamWriter sw;
                string fileNameerror = @"ts.html";
                sw = File.CreateText(fileNameerror);
                sw.WriteLine("<html>");
                sw.WriteLine("<title>Tabla de símbolos</title>");
                sw.WriteLine("<center><head><font color=white><h1>TABLA DE SÍMBOLOS</h1></font></head></center>");
                sw.WriteLine("<body background=noche.jpg bgcolor= black>");
                sw.WriteLine("<center><table border= 5>");
                sw.WriteLine("<tr><td><font color=white><center>No.</center></font></td><td><font color=white><center>LÍNEA</center></font></td>" +
                    "<td><font color= white><center>COLUMNA</center></font></td><td><font color= white><center>NOMBRE</center></font></td>" +
                    "<td><font color= white><center>TIPO</center></font></td><td><font color= white><center>ÁMBITO</center></font></td></tr>");


                foreach (var s in entG)
                {
                    foreach (var simbol in s.var)
                    {
                        if (simbol.flag)
                        {
                            sw.WriteLine("<tr><td><font color=white><center>" + no + "</center></font></td><td><font color=white><center>" +
                                simbol.Linea + "</center></font></td><td><font color= white><center>" + simbol.Columna + "</center></font></td><td><font color=white><center>"
                                + simbol.Nombre + "</center></font></td><td><font color= white><center>" + simbol.Tipo
                                + "</center></font></td><td><font color= white><center>" + simbol.Ambito + "</center></font></td></tr>");
                            no += 1;
                        }
                    }
                }


                sw.WriteLine("</table></center>");
                sw.WriteLine("</body>");
                sw.WriteLine("</html>");
                sw.Flush();
                sw.Close();
                //Process.Start(fileNameerror);
            }
            catch (Exception)
            {
                Debug.WriteLine("No se pudo graficar :(");
            }
        }


        private Simbolo RetornarSimbolo(string nombre)
        {
            Debug.WriteLine("Retornando símbolo: {0}", nombre);
            Debug.WriteLine("cantidad {0}", cimaEnt.var.Count);

            foreach (var item in cimaEnt.var)
            {
                Debug.WriteLine("Variables reconocidas: {0}, de tipo: {1}, de valor: {2}", item.Nombre, item.Tipo, item.Valor);
            }
            if (cimaEnt.var.Count != 0)
            {
                foreach (Simbolo simbol in cimaEnt.var)
                {
                    if (simbol.Nombre.ToLower().Equals(nombre.ToLower()))
                    {
                        return simbol;
                    }
                }
            }

            return RetornarSimboloAST(nombre, entG);
        }

        private Simbolo RetornarSimboloAST(string nombre, List<Entorno> ent)
        {
            Debug.WriteLine("Buscando simbolo del AST: {0}, el nombre; {1}", ent.Count, nombre);
            foreach (Entorno e in ent)
            {
                if (e.Flag)
                {
                    if (e.ent != null)
                    {
                        foreach (Entorno eAux in e.ent)
                        {
                            Simbolo sim = RetornarSimboloAST(nombre, eAux.ent);
                            if (sim != null)
                            {
                                return sim;
                            }
                        }
                    }

                    //Verificar esto, sino cambiarlo de lugar
                    if (e.var != null)
                    {
                        foreach (Simbolo sim in e.var)
                        {
                            if (sim.Valor.ToLower().Equals(nombre.ToLower()))
                            {
                                return sim;
                            }
                        }
                    }
                }
            }

            foreach(var item2 in entG)
            {
                foreach (var item in item2.var)
                {
                    Debug.WriteLine("Buscando globalmente: {0}, {1}", item.Nombre, item.Valor);
                    if (nombre.ToLower().Equals(item.Nombre.ToLower()))
                    {
                        Debug.WriteLine("Si está globalmente");
                        return item;
                    }
                }
            }

            return null;
            //return null;
        }

        private Entorno getEntornoL(string ambito, string ambitoPadre)
        {
            Debug.WriteLine("------------ Entorno de funcion: {0}, {1}", ambito, ambitoPadre);
            if (cimaEnt.ent != null)
            {
                foreach (Entorno en in cimaEnt.ent)
                {
                    Debug.WriteLine("------------ Entorno de funcion0000: {0}, {1}", en.Ambito, ambito);
                    if (en.Ambito.Equals(ambito))
                    {
                        en.Flag = true;
                        return en;
                    }
                }
            }
            return getEntorno(ambito, ambitoPadre, entG);
        }

        private Entorno getEntorno(string ambito, string ambitoPadre, List<Entorno> entorn)
        {
            Debug.WriteLine("------------ Entorno de funcion1111: {0}, {1}", ambito, ambitoPadre);
            foreach (Entorno ent in entorn)
            {
                foreach (Entorno en in ent.Entornos)
                {
                    Debug.WriteLine("------------ Entorno de funcion2: {0}, {1}", en.Ambito, en.AmbitoPadre);
                    //if (en.Ambito.Equals(ambito) && en.AmbitoPadre.Equals(ambitoPadre) && (en.Flag == false))
                    if (en.Ambito.Equals(ambito) && en.AmbitoPadre.Equals(ambitoPadre))
                    {
                        en.Flag = true;
                        flag = true;
                        return en;
                    }
                    Entorno ret = getEntorno(ambito, ambitoPadre, en.ent);
                    if (ret != null)
                    {
                        return ret;
                    }
                }

                Debug.WriteLine("------------ Entorno de funcion3: {0}, {1}", ent.Ambito, ent.AmbitoPadre);
                if (ent.Ambito.Equals(ambito) && ent.AmbitoPadre.Equals(ambitoPadre))
                {
                    ent.Flag = true;
                    return ent;
                }
            }


            return null;
        }
        private int getCantVar(String cadena)
        {
            char[] character = cadena.ToCharArray();
            int count = 0;

            for (int i = 0; i < character.Length; i++)
            {
                count += Encoding.ASCII.GetBytes(character[i] + "")[0];
            }
            return count;
        }

        private string getLine(ParseTreeNode Nodo)
        {
            return (Nodo.Token.Location.Line + 1) + "";
        }

        private string getColumn(ParseTreeNode Nodo)
        {
            return (Nodo.Token.Location.Column + 1) + "";
        }
    }
}
