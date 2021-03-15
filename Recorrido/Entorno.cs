using System;
using System.Collections.Generic;
using Proyecto1.Ejecucion;
using Proyecto1.Analizador;
using Irony.Parsing;
using System.Diagnostics;
using System.Text;

namespace Proyecto1.Recorrido
{
    class Entorno
    {
        public List<Simbolo> var = null;
        public List<Entorno> ent = null;
        public List<Error> error = new List<Error>();
        private ParseTreeNode Nodo = null;
        private ParseTreeNode Nodo2 = null;
        private ParseTreeNode NodoAux;
        private string ambitoPadre;
        private string ambitoNormal;
        private bool flag;


        public Entorno(ParseTreeNode nodo, string ambito)
        {
            this.var = new List<Simbolo>();
            this.ent = new List<Entorno>();
            this.Nodo = nodo;   //Agregar, nodo2 si no deja obtener el entorno de las funciones
            this.ambitoPadre = ambito;
            this.NodoAux = null;
            this.flag = false;
        }

        public string Ambito
        {
            get => ambitoNormal;
            set => ambitoNormal = value;
        }

        public string AmbitoPadre
        {
            get => ambitoPadre;
            set => ambitoPadre = value;
        }

        public ParseTreeNode nodoAux
        {
            get => NodoAux;
            set => NodoAux = value;
        }

        public bool Flag
        {
            get => flag;
            set => flag = value;
        }

        public List<Simbolo> Var
        {
            get => var;
            set => var = value;
        }

        public List<Entorno> Entornos
        {
            get => ent;
            set => ent = value;
        }

        //------------------------------------------------------------ START
        public void Inicio()
        {
            switch (Nodo.Term.Name)
            {
                case "inicio": 
                    Debug.WriteLine("++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
                    Debug.WriteLine("Nuevo entorno(" + Nodo.Term.Name + "): " + Nodo.ChildNodes[1].Token.Text);
                    Debug.WriteLine("++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
                    this.ambitoNormal = Nodo.ChildNodes[1].Token.Text;
                    this.NodoAux = Nodo.ChildNodes[5];
                    this.flag = true;
                    Cuerpo(Nodo.ChildNodes[3]);
                    break;
                case "funcion":
                    switch (Nodo.ChildNodes.Count)
                    {
                        case 13: 
                            Debug.WriteLine("================================================================================");
                            Debug.WriteLine("Nuevo entorno de funcion(" + Nodo.Term.Name + "): " + Nodo.ChildNodes[1].Token.Text);
                            Debug.WriteLine("================================================================================");
                            this.ambitoNormal = Nodo.ChildNodes[1].Token.Text;
                            this.NodoAux = Nodo.ChildNodes[10];
                            Cuerpo(Nodo.ChildNodes[8]);
                            break;
                        case 8: 
                            Debug.WriteLine("================================================================================");
                            Debug.WriteLine("Nuevo entorno de funcion(" + Nodo.Term.Name + "): " + Nodo.ChildNodes[1].Token.Text);
                            Debug.WriteLine("================================================================================");
                            Form1.Salida_Inst.AppendText("Prueba");
                            this.ambitoNormal = Nodo.ChildNodes[1].Token.Text;
                            this.NodoAux = Nodo.ChildNodes[7];
                            Cuerpo(Nodo.ChildNodes[5]);
                            break;
                    }
                    break;
                case "procedimiento":
                    switch (Nodo.ChildNodes.Count)
                    {
                        case 11: 
                            Debug.WriteLine("================================================================================");
                            Debug.WriteLine("Nuevo entorno de procedimiento(" + Nodo.Term.Name + "): " + Nodo.ChildNodes[1].Token.Text);
                            Debug.WriteLine("================================================================================");
                            this.ambitoNormal = Nodo.ChildNodes[1].Token.Text;
                            this.NodoAux = Nodo.ChildNodes[8];
                            Cuerpo(Nodo.ChildNodes[6]);
                            break;
                        case 8: 
                            Debug.WriteLine("================================================================================");
                            Debug.WriteLine("Nuevo entorno de procedimiento(" + Nodo.Term.Name + "): " + Nodo.ChildNodes[1].Token.Text);
                            Debug.WriteLine("================================================================================");
                            this.ambitoNormal = Nodo.ChildNodes[1].Token.Text;
                            this.NodoAux = Nodo.ChildNodes[5];
                            Cuerpo(Nodo.ChildNodes[3]);
                            break;
                    }
                    break;
                default:
                    Debug.WriteLine("Error AST-->Nodo " + Nodo.Term.Name + " es empty/null");
                    break;
            }
        }

        public void Cuerpo(ParseTreeNode Nodo)
        {
            if (Nodo != null)
            {
                switch (Nodo.Term.Name)
                {
                    case "cuerpo":
                        #region
                        foreach (ParseTreeNode hijo in Nodo.ChildNodes)
                        {
                            Cuerpo(hijo);
                        }
                        #endregion
                        break;
                    case "interno":
                        #region                        
                        switch (Nodo.ChildNodes[0].Term.Name)
                        {
                            case "var1":
                                Debug.WriteLine("Agregando nueva variable a tabla de simbolos");
                                var1(Nodo.ChildNodes[0]); 
                                break;
                            case "funcion":
                                Debug.WriteLine("** Creando un nuevo entorno de funcion");
                                Entorno entVar = new Entorno(Nodo.ChildNodes[0], ambitoNormal);
                                entVar.Inicio();
                                this.ent.Add(entVar);
                                break;
                            case "procedimiento":
                                Debug.WriteLine("** Creando un nuevo entorno de procedimiento");
                                Entorno entProc = new Entorno(Nodo.ChildNodes[0], ambitoNormal);
                                entProc.Inicio();
                                this.ent.Add(entProc);
                                break;
                            default:
                                Debug.WriteLine("***ERROR GRAVE, ****" + Nodo.Term.Name + " es empty/null");
                                break;
                        }
                        #endregion
                        break;
                    case "list_sentencias":
                        break;
                }
            }
            else
            {
                Debug.WriteLine("***Error Semántico*** Nodo en funcion CUERPO no existente/detectado/null");
            }
        }

        public void var1(ParseTreeNode Nodo)
        {
            switch (Nodo.ChildNodes[0].Term.Name)
            {
                case "type":
                    break;
                case "var":
                    list_vars(Reservada.var, Nodo.ChildNodes[1]);
                    break;
                case "const":
                    //Probar si funciona igual que var, el de antes
                    break;
                case "id":
                    break;
                default:
                    Debug.WriteLine("**Error Semántico en var1** " + Nodo.ChildNodes[0].Term.Name + ": Variable no declarada");
                    break;
            }
        }

        private void list_vars(string tipoObj, ParseTreeNode Nodo)
        {
            switch (Nodo.Term.Name)
            {
                case "list_vars":
                    foreach (ParseTreeNode hijo in Nodo.ChildNodes)
                    {
                        list_vars(tipoObj, hijo);
                    }
                    if (tipoObj == "var")
                    {
                        //Debug.WriteLine("Es tipo VAR");
                    }
                    break;
                case "var2":
                    var2(tipoObj, Nodo);
                    break;
                default:
                    Debug.WriteLine("**Error Semántico en list_vars**: Variable no declarada");
                    break;
            }
        }

        private void var2(string tipoObj, ParseTreeNode Nodo)
        {
            switch (Nodo.Term.Name)
            {
                case "var2":
                    if (Nodo.ChildNodes.Count == 4)
                    {
                        if (Nodo.ChildNodes[1].Term.Name == ":")
                        {
                            Debug.WriteLine("Entro a list_id + DOSP + tipo + PYC");
                            string dato = getTipo(Nodo.ChildNodes[2]);
                            Retorno ret = new Retorno(dato, InicializacionDato(dato), getLine(Nodo.ChildNodes[1]), getColumn(Nodo.ChildNodes[1]));
                            DeclararYAsginar(tipoObj, dato, ret, Nodo.ChildNodes[0]);
                        }
                        else
                        {
                            Debug.WriteLine("Entro a list_id + IGUAL + tipo + PYC");
                            string dato = getTipo(Nodo.ChildNodes[2]);
                            Retorno ret = new Retorno(dato, InicializacionDato(dato), getLine(Nodo.ChildNodes[1]), getColumn(Nodo.ChildNodes[1]));
                            DeclararYAsginar(tipoObj, dato, ret, Nodo.ChildNodes[0]);
                        }
                    }

                    else if (Nodo.ChildNodes.Count == 6)
                    {
                        string td = getTipo(Nodo.ChildNodes[2]);
                        Retorno ret = Cond(Nodo.ChildNodes[4]);
                        DeclararYAsginar(tipoObj, td, ret, Nodo.ChildNodes[0]);
                    }
                    break;
                default:
                    Debug.WriteLine("Error Semántico*** Nodo en funcion var2 no existente/detectado");
                    break;
            }
        }

        private void DeclararYAsginar(string tipoObj, string tipodato, Retorno ret, ParseTreeNode Nodo)
        {
            if (Nodo != null)
            {
                switch (Nodo.Term.Name)
                {
                    case "list_id":
                        foreach (ParseTreeNode hijo in Nodo.ChildNodes)
                        {
                            DeclararYAsginar(tipoObj, tipodato, ret, hijo);
                        }
                        break;

                    case "id":
                        string id = Nodo.Token.Value.ToString();

                        Debug.WriteLine("---------BIEEEEEEEEEEEEN: Variable reconocida. Nombre variable: " + id + 
                            " tipo objeto: " + tipoObj + " tipo dato: " + tipodato + " valor asignado: " + ret.Value.ToString());

                        if (!SimbolExist(id))
                        {
                            if (ret != null)
                            {
                                if (ret.Type.Equals(tipodato)) 
                                {
                                    Debug.WriteLine("----------- SIIIIIIIIUUUUUU. Se creo la variable: " + id + ". Con valor: " + ret.Value +
                                        " y de tipo: " + ret.Type);
                                    var.Add(new Simbolo(getLine(Nodo), getColumn(Nodo), Reservada.varL, id, ret.Value, tipodato, Reservada.var, true, null));
                                }
                                else
                                {
                                    Debug.WriteLine("**Error Semantico** Asignacion no valida, tipo de dato incorrecto en la linea: " + getLine(Nodo) + " columna:" + getColumn(Nodo));
                                    error.Add(new Error(int.Parse(getLine(Nodo)), int.Parse(getColumn(Nodo)), Reservada.ErrorSemantico, "Asignacion no valida, tipo de dato incorrecto"));
                                }
                            }
                            else
                            {
                                Debug.WriteLine("**Error Semantico** Asignacion no valida, expresion trajo null en la linea: " + getLine(Nodo) + " columna:" + getColumn(Nodo));
                                error.Add(new Error(int.Parse(getLine(Nodo)), int.Parse(getColumn(Nodo)), Reservada.ErrorSemantico, "Asignacion no valida, expresion incorrecta"));
                            }
                        }
                        else
                        {
                            Debug.WriteLine("**Error Semantico** Asignacion no valida, variable ya existe en la linea: " + getLine(Nodo) + " columna:" + getColumn(Nodo));
                            error.Add(new Error(int.Parse(getLine(Nodo)), int.Parse(getColumn(Nodo)), Reservada.ErrorSemantico, "Asignacion no valida, variable ya existe"));
                        }
                        break;

                    case ",": //No hace nada
                        break;

                    default:
                        Debug.WriteLine("***Error Semántico*** Nodo en DeclararYAsignar no existe/detectado");
                        break;
                }
            }
            else
                Debug.WriteLine("***Error Semántico*** Nodo en funcion DeclararYAsignar no existente/detectado/null");
        }

        private Retorno Cond(ParseTreeNode Nodo)
        {
            if (Nodo.ChildNodes.Count == 3)
            {
                switch (Nodo.ChildNodes[0].Term.Name)
                {
                    case "cond": //AND
                        #region
                        Retorno cond = Cond(Nodo.ChildNodes[0]);
                        Retorno cond1 = Cond(Nodo.ChildNodes[2]);

                        if ((cond != null) && (cond1 != null))
                        {
                            if (cond.Type.Equals(Reservada.Booleano) && cond1.Type.Equals(Reservada.Booleano))
                            {
                                if (cond.Type.Equals("True"))
                                {
                                    //flag = false;
                                }
                                if (cond.Type.Equals("True") && cond1.Type.Equals("True"))
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

                    case "cond1": //OR
                        #region
                        Retorno condA1 = Cond(Nodo.ChildNodes[0]);
                        Retorno condA2 = Cond(Nodo.ChildNodes[2]);

                        if ((condA1 != null) && (condA2 != null))
                        {
                            if (condA1.Type.Equals(Reservada.Booleano) && condA2.Type.Equals(Reservada.Booleano))
                            {
                                if (condA2.Type.Equals("False"))
                                {
                                    //flag = false;
                                }
                                if (condA1.Value.Equals("False") && condA2.Value.Equals("False"))
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
                        Retorno condC3 = Cond(Nodo.ChildNodes[0]);
                        Retorno condC4 = Cond(Nodo.ChildNodes[2]);

                        if ((condC3 != null) && (condC4 != null))
                        {
                            if (condC3.Type.Equals("False"))
                            {
                                //flag = false;
                            }
                            if ((condC3.Type.Equals(Reservada.Entero) && condC4.Type.Equals(Reservada.Real)) ||
                                     (condC3.Type.Equals(Reservada.Real) && condC4.Type.Equals(Reservada.Entero)) ||
                                         (condC3.Type.Equals(Reservada.Entero) && condC4.Type.Equals(Reservada.Entero)) ||
                                             (condC3.Type.Equals(Reservada.Real) && condC4.Type.Equals(Reservada.Real))) 
                            {
                                double val1 = double.Parse(condC3.Value);
                                double val2 = double.Parse(condC4.Value);

                                if (val1 <= val2)
                                {
                                    return new Retorno(Reservada.Booleano, "True", getLine(Nodo.ChildNodes[1]), getColumn(Nodo.ChildNodes[1]));
                                }
                                else
                                {
                                    return new Retorno(Reservada.Booleano, "False", getLine(Nodo.ChildNodes[1]), getColumn(Nodo.ChildNodes[1]));
                                }
                            }
                            else if ((condC3.Type.Equals(Reservada.Stringg) && condC4.Type.Equals(Reservada.Stringg)))
                            {
                                int v1 = getCantVar(condC3.Value);
                                int v2 = getCantVar(condC4.Value);

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
                        Retorno condD4 = Cond(Nodo.ChildNodes[0]);
                        Retorno condD5 = Cond(Nodo.ChildNodes[2]);

                        if ((condD4 != null) && (condD5 != null))
                        {
                            if (condD5.Type.Equals("False"))
                            {
                                //flag = false;
                            }
                            if ((condD4.Type.Equals(Reservada.Entero) && condD5.Type.Equals(Reservada.Real)) || 
                                     (condD4.Type.Equals(Reservada.Real) && condD5.Type.Equals(Reservada.Entero)) ||
                                         (condD4.Type.Equals(Reservada.Entero) && condD5.Type.Equals(Reservada.Entero)) || 
                                             (condD4.Type.Equals(Reservada.Real) && condD5.Type.Equals(Reservada.Real))) 
                            {
                                double val1 = double.Parse(condD4.Value);
                                double val2 = double.Parse(condD5.Value);

                                if (val1 >= val2)
                                {
                                    return new Retorno(Reservada.Booleano, "True", getLine(Nodo.ChildNodes[1]), getColumn(Nodo.ChildNodes[1])); 
                                }
                                else
                                {
                                    return new Retorno(Reservada.Booleano, "False", getLine(Nodo.ChildNodes[1]), getColumn(Nodo.ChildNodes[1]));
                                }
                            }
                            else if ((condD4.Type.Equals(Reservada.Stringg) && condD5.Type.Equals(Reservada.Stringg)))
                            {
                                int v1 = getCantVar(condD4.Value);
                                int v2 = getCantVar(condD5.Value);

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
                        Retorno condE5 = Cond(Nodo.ChildNodes[0]);
                        Retorno condE6 = Cond(Nodo.ChildNodes[2]);

                        if ((condE5 != null) && (condE6 != null))
                        {
                            if (condE5.Type.Equals("False"))
                            {
                                //flag = false;
                            }
                            if ((condE5.Type.Equals(Reservada.Entero) && condE6.Type.Equals(Reservada.Real)) ||  
                                     (condE5.Type.Equals(Reservada.Real) && condE6.Type.Equals(Reservada.Entero)) ||   
                                         (condE5.Type.Equals(Reservada.Entero) && condE6.Type.Equals(Reservada.Entero)) ||
                                             (condE5.Type.Equals(Reservada.Real) && condE6.Type.Equals(Reservada.Real)))  
                            {
                                double val1 = double.Parse(condE5.Value);
                                double val2 = double.Parse(condE6.Value);

                                if (val1 < val2)
                                {
                                    return new Retorno(Reservada.Booleano, "True", getLine(Nodo.ChildNodes[1]), getColumn(Nodo.ChildNodes[1]));
                                }
                                else
                                {
                                    return new Retorno(Reservada.Booleano, "False", getLine(Nodo.ChildNodes[1]), getColumn(Nodo.ChildNodes[1]));
                                }
                            }
                            else if ((condE5.Type.Equals(Reservada.Stringg) && condE6.Type.Equals(Reservada.Stringg)))
                            {
                                int v1 = getCantVar(condE5.Value);
                                int v2 = getCantVar(condE6.Value);

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
                        Retorno condF6 = Cond(Nodo.ChildNodes[0]);
                        Retorno condF7 = Cond(Nodo.ChildNodes[2]);

                        if ((condF6 != null) && (condF7 != null))
                        {
                            if (condF6.Type.Equals("False"))
                            {
                                //flag = false;
                            }
                            if ((condF6.Type.Equals(Reservada.Entero) && condF7.Type.Equals(Reservada.Real)) || 
                                     (condF6.Type.Equals(Reservada.Real) && condF7.Type.Equals(Reservada.Entero)) ||  
                                         (condF6.Type.Equals(Reservada.Entero) && condF7.Type.Equals(Reservada.Entero)) ||
                                             (condF6.Type.Equals(Reservada.Real) && condF7.Type.Equals(Reservada.Real))) 
                            {
                                double val1 = double.Parse(condF6.Value);
                                double val2 = double.Parse(condF7.Value);

                                if (val1 > val2)
                                {
                                    return new Retorno(Reservada.Booleano, "True", getLine(Nodo.ChildNodes[1]), getColumn(Nodo.ChildNodes[1]));
                                }
                                else
                                {
                                    return new Retorno(Reservada.Booleano, "False", getLine(Nodo.ChildNodes[1]), getColumn(Nodo.ChildNodes[1]));
                                }
                            }
                            else if ((condF6.Type.Equals(Reservada.Stringg) && condF7.Type.Equals(Reservada.Stringg)))
                            {
                                int v1 = getCantVar(condF6.Value);
                                int v2 = getCantVar(condF7.Value);

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
                        Retorno condG7 = Cond(Nodo.ChildNodes[0]);
                        Retorno condG8 = Cond(Nodo.ChildNodes[2]);

                        if ((condG7 != null) && (condG8 != null)) 
                        {
                            if (condG7.Type.Equals("False"))
                            {
                                //flag = false;
                            }
                            if ((condG7.Type.Equals(Reservada.Entero) && condG8.Type.Equals(Reservada.Real)) || 
                                     (condG7.Type.Equals(Reservada.Real) && condG8.Type.Equals(Reservada.Entero)) || 
                                         (condG7.Type.Equals(Reservada.Entero) && condG8.Type.Equals(Reservada.Entero)) || 
                                             (condG7.Type.Equals(Reservada.Real) && condG8.Type.Equals(Reservada.Real)))   
                            {
                                double val1 = double.Parse(condG7.Value);
                                double val2 = double.Parse(condG8.Value);

                                if (val1 == val2)
                                {
                                    return new Retorno(Reservada.Booleano, "True", getLine(Nodo.ChildNodes[1]), getColumn(Nodo.ChildNodes[1])); 
                                }
                                else
                                {
                                    return new Retorno(Reservada.Booleano, "False", getLine(Nodo.ChildNodes[1]), getColumn(Nodo.ChildNodes[1]));
                                }
                            }
                            else if ((condG7.Type.Equals(Reservada.Stringg) && condG8.Type.Equals(Reservada.Stringg)) || 
                                    (condG7.Type.Equals(Reservada.Booleano) && condG8.Type.Equals(Reservada.Booleano)))  
                            {
                                int v1 = getCantVar(condG7.Value);
                                int v2 = getCantVar(condG8.Value);

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
                        Retorno condH8 = Cond(Nodo.ChildNodes[0]);
                        Retorno exp = Exp(Nodo.ChildNodes[2]);

                        if ((condH8 != null) && (exp != null)) // Si ambos son distintos de null entra
                        {
                            if (condH8.Type.Equals("False"))
                            {
                                //flag = false;
                            }
                            if ((condH8.Type.Equals(Reservada.Entero) && exp.Type.Equals(Reservada.Real)) ||
                                     (condH8.Type.Equals(Reservada.Real) && exp.Type.Equals(Reservada.Entero)) ||
                                         (condH8.Type.Equals(Reservada.Entero) && exp.Type.Equals(Reservada.Entero)) ||
                                             (condH8.Type.Equals(Reservada.Real) && exp.Type.Equals(Reservada.Real))) 
                            {
                                double val1 = double.Parse(condH8.Value);
                                double val2 = double.Parse(exp.Value);

                                if (val1 != val2)
                                {
                                    return new Retorno(Reservada.Booleano, "True", getLine(Nodo.ChildNodes[1]), getColumn(Nodo.ChildNodes[1])); //retorno true
                                }
                                else
                                {
                                    return new Retorno(Reservada.Booleano, "False", getLine(Nodo.ChildNodes[1]), getColumn(Nodo.ChildNodes[1])); //retorno True
                                }
                            }
                            else if ((condH8.Type.Equals(Reservada.Stringg) && exp.Type.Equals(Reservada.Stringg)) ||      //Si ambos son String
                                    (condH8.Type.Equals(Reservada.Booleano) && exp.Type.Equals(Reservada.Booleano)))     //Si ambos son Boolean
                            {
                                int v1 = getCantVar(condH8.Value);
                                int v2 = getCantVar(exp.Value);

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
            else if (Nodo.ChildNodes.Count == 2)    //NOT Verificar si está bien, o agregar la producción comentada
            {
                #region
                Retorno condB3 = Cond(Nodo.ChildNodes[1]);

                if (condB3 != null)
                {
                    if (condB3.Type.Equals(Reservada.Booleano))
                    {
                        if (condB3.Type.Equals("True"))
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
                                    return new Retorno(Reservada.Booleano, suma + "", getLine(Nodo.ChildNodes[0]), getColumn(Nodo.ChildNodes[0]));
                                }
                                else
                                {
                                    return new Retorno(ret1.Type, suma + "", getLine(Nodo.ChildNodes[0]), getColumn(Nodo.ChildNodes[0]));
                                }
                            }
                            else if (ret1.Type.Equals(Reservada.Stringg) || ret2.Type.Equals(Reservada.Stringg))
                            {
                                string concat = Conv(ret1).Value + Conv(ret2).Value;
                                return new Retorno(Reservada.Stringg, concat, getLine(Nodo.ChildNodes[0]), getColumn(Nodo.ChildNodes[0]));
                            }
                            else if ((ret1.Type.Equals(Reservada.Booleano) && ret2.Type.Equals(Reservada.Real)) || (ret1.Type.Equals(Reservada.Real) && ret2.Type.Equals(Reservada.Booleano))
                                        || (ret1.Type.Equals(Reservada.Real) && ret2.Type.Equals(Reservada.Entero)) || (ret1.Type.Equals(Reservada.Entero) && ret2.Type.Equals(Reservada.Real))
                                        || (ret1.Type.Equals(Reservada.Real) && ret2.Type.Equals(Reservada.Entero)) || (ret1.Type.Equals(Reservada.Entero) && ret2.Type.Equals(Reservada.Real)))
                            {
                                double suma = double.Parse(Conv(ret1).Value) + double.Parse(Conv(ret2).Value);
                                return new Retorno(Reservada.Real, suma + "", getLine(Nodo.ChildNodes[0]), getColumn(Nodo.ChildNodes[0]));
                            }
                            else if ((ret1.Type.Equals(Reservada.Booleano) && ret2.Type.Equals(Reservada.Entero)) || (ret1.Type.Equals(Reservada.Entero) && ret2.Type.Equals(Reservada.Booleano)))
                            {
                                double suma = double.Parse(Conv(ret1).Value) + double.Parse(Conv(ret2).Value);
                                return new Retorno(Reservada.Entero, suma + "", getLine(Nodo.ChildNodes[0]), getColumn(Nodo.ChildNodes[0]));
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
                                return new Retorno(Reservada.Entero, resta + "", getLine(Nodo.ChildNodes[0]), getColumn(Nodo.ChildNodes[0]));
                            }
                            else if ((ret1.Type.Equals(Reservada.Real) && ret2.Type.Equals(Reservada.Real)) 
                                || (ret1.Type.Equals(Reservada.Booleano) && ret2.Type.Equals(Reservada.Real))
                                || (ret1.Type.Equals(Reservada.Real) && ret2.Type.Equals(Reservada.Booleano))
                                || (ret1.Type.Equals(Reservada.Real) && ret2.Type.Equals(Reservada.Entero))
                                || (ret1.Type.Equals(Reservada.Entero) && ret2.Type.Equals(Reservada.Real)))
                            {
                                double resta = double.Parse(Conv(ret1).Value) - double.Parse(Conv(ret2).Value);
                                return new Retorno(Reservada.Real, resta + "", getLine(Nodo.ChildNodes[0]), getColumn(Nodo.ChildNodes[0]));
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
                                return new Retorno(Reservada.Entero, mul + "", getLine(Nodo.ChildNodes[0]), getColumn(Nodo.ChildNodes[0]));
                            }
                            else if ((ret1.Type.Equals(Reservada.Real) && ret2.Type.Equals(Reservada.Real))
                                || (ret1.Type.Equals(Reservada.Booleano) && ret2.Type.Equals(Reservada.Real))
                                || (ret1.Type.Equals(Reservada.Real) && ret2.Type.Equals(Reservada.Booleano))
                                || (ret1.Type.Equals(Reservada.Real) && ret2.Type.Equals(Reservada.Entero))
                                || (ret1.Type.Equals(Reservada.Entero) && ret2.Type.Equals(Reservada.Real)))
                            {
                                double mul = double.Parse(Conv(ret1).Value) * double.Parse(Conv(ret2).Value);
                                return new Retorno(Reservada.Real, mul + "", getLine(Nodo.ChildNodes[0]), getColumn(Nodo.ChildNodes[0]));
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
                                return new Retorno(Reservada.Real, div + "", getLine(Nodo.ChildNodes[0]), getColumn(Nodo.ChildNodes[0]));
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
                                return new Retorno(Reservada.Real, mod + "", getLine(Nodo.ChildNodes[0]), getColumn(Nodo.ChildNodes[0]));
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
                            return null;

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
                                getLine(Nodo.ChildNodes[1]) + " columna:" + getColumn(Nodo.ChildNodes[1]));
                                error.Add(new Error(int.Parse(getLine(Nodo.ChildNodes[1])), int.Parse(getColumn(Nodo.ChildNodes[1])), Reservada.ErrorSemantico, "Error en el retorno de los paréntesis"));
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

        private string getTipo(ParseTreeNode Nodo)
        {
            switch (Nodo.ChildNodes[0].Term.Name)
            {
                case "integer":
                    return "Integer";

                case "real":
                    return "Real";

                case "boolean":
                    return "Boolean";

                case "string":
                    return "String";

                default:
                    return "null";

            }
        }
        private string InicializacionDato(string tipodato)
        {
            if (tipodato.Equals(Reservada.Entero))
            {
                return "0";
            }
            else if (tipodato.Equals(Reservada.Real))
            {
                return "0.0";
            }
            else if (tipodato.Equals(Reservada.Stringg))
            {
                return "\"\"";
            }
            else if (tipodato.Equals(Reservada.Booleano))
            {
                return "False";
            }
            return "";
        }


        //------------------------------------------------------Funciones del Recorrido con retornos
        private string getLine(ParseTreeNode Nodo)
        {
            return (Nodo.Token.Location.Line + 1) + "";
        }

        private string getColumn(ParseTreeNode Nodo)
        {
            return (Nodo.Token.Location.Column + 1) + "";
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

        private bool SimbolExist(string name)
        {
            if(this.var.Count != 0)
            {
                foreach(Simbolo aux in this.var)
                {
                    if (aux.Nombre.Equals(name))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
