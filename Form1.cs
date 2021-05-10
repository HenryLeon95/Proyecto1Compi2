using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Proyecto1.Analizador;
using Proyecto1.Recorrido;
using Proyecto1.Proyecto2.Compilador;
using Irony.Ast;
using Irony.Parsing;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace Proyecto1
{
    public partial class Form1 : Form
    {
        ParseTree arbol = null;
        ParseTreeNode raiz = null;
        private List<Error> error_list = new List<Error>();
        public static RichTextBox Salida_Inst;
        public static RichTextBox Consola_Inst;
        private List<Entorno> ent;
        Ejecucionn ejec;

        //PROYECTO2
        private List<EntornoC> enviroment;

        private bool analizar(string text)
        {
            bool flag = false;
            Gramatica gramatica = new Gramatica();
            LanguageData lenguaje = new LanguageData(gramatica);
            Parser parser = new Parser(lenguaje);
            arbol = parser.Parse(text);
            raiz = arbol.Root;
            error_list.Clear();
            Salida.Clear();
            SalidaErrores.Clear();

            for (int i = 0; i < arbol.ParserMessages.Count(); i++)
            {
                if (arbol.ParserMessages.ElementAt(i).Message.Contains("Invalid character"))
                {
                    Error error = new Error(
                    arbol.ParserMessages.ElementAt(i).Location.Line + 1,
                    arbol.ParserMessages.ElementAt(i).Location.Column + 1, "Léxico",
                    arbol.ParserMessages.ElementAt(i).Message.Replace("Invalid character:", "Caracter No Reconocido: "));
                    error_list.Add(error);
                }
                else if (arbol.ParserMessages.ElementAt(i).Message.Contains("Syntax error"))
                {
                    if (!(arbol.ParserMessages.ElementAt(i).Message.Contains(",, $")))
                    {
                        Error error = new Error(
                        arbol.ParserMessages.ElementAt(i).Location.Line + 1,
                        arbol.ParserMessages.ElementAt(i).Location.Column + 1, "Sintáctico",
                        arbol.ParserMessages.ElementAt(i).Message.Replace("Syntax error, expected:", "Se esperaba: "));
                        error_list.Add(error);
                    }
                }
            }

            if (raiz != null)
            {
                flag = true;
            }

            return flag;
        }

        private void ViewErrors()
        {
            SalidaErrores.AppendText("================== ERRORES LEXICOS, SINTACTICOS ==================" + "\n");
            SalidaErrores.AppendText("Linea" + "\t" + "Columna" + "\t\t" + "Tipo" + "\t\t" + "Descripcion"  + "\n");
            foreach (Error error in error_list)
            {
                SalidaErrores.AppendText(error.line + "\t" + error.column + "\t\t" + error.type + "\t" + error.description + "\n");
            }

            SalidaErrores.AppendText("\n======================= SEMÁNTICOS =======================" + "\n");
            SalidaErrores.AppendText("Linea" + "\t" + "Columna" + "\t\t" + "Tipo" + "\t\t" + "Descripcion" + "\n");
            //foreach (Error error in this.ejec.error)
            //{
            //    SalidaErrores.AppendText(error.line + "\t" + error.column + "\t\t" + error.type + "\t" + error.description + "\n");
            //}
            foreach (Error error in this.ejec.error)
            {
                SalidaErrores.AppendText(error.line + "\t" + error.column + "\t\t" + error.type + "\t" + error.description + "\n");
            }
        }

        private void ViewSymbolTable()
        {
            SalidaErrores.AppendText("\n======================= TABLA DE SÍMBOLOS =======================" + "\n");
            SalidaErrores.AppendText("Ambito" + "\t\t" + "Ambito Padre" + "\n");
            foreach (var table in this.ent)
            {
                foreach (var simbol in table.Entornos)
                {
                    SalidaErrores.AppendText(simbol.Ambito + "\t" + simbol.AmbitoPadre + "\n");
                }
            }
            SalidaErrores.AppendText("\nAmbito" + "\t\t" + "Nombre" + "\t" + "Valor" + "\t\t" + "Tipo" + "\t\t" + "Tipo de Objeto" + "\n");
            foreach (var table in this.ent)
            {
                foreach (var simbol in table.var)
                {
                    SalidaErrores.AppendText(simbol.Ambito + "\t" + simbol.Nombre + "\t" + simbol.Valor + "\t\t" + simbol.Tipo + "\t\t"
                        + simbol.TipoObjeto + "\n");
                }
            }
        }

            public void GraficarArbol(ParseTreeNode root)
        {
            try
            {
                string grafica = "digraph Arbol_Sintactico{\n\n" + GraficaNodos(root, "0") + "\n\n}";
                FileStream stream = new FileStream("Arbol.dot", FileMode.Create, FileAccess.Write);
                StreamWriter writer = new StreamWriter(stream);
                writer.WriteLine(grafica);
                writer.Close();
                //Ejecuta el codigo
                var command = string.Format("dot -Tjpg Arbol.dot -o Arbol.jpg");
                var procStartInfo = new ProcessStartInfo("cmd", "/C " + command);
                var proc = new Process();
                proc.StartInfo = procStartInfo;
                proc.Start();
                proc.WaitForExit();

                //Thread.Sleep(2000);
                //Process.Start(@"Arbol.jpg");
            }
            catch (Exception x)
            {
                MessageBox.Show("Error inesperado cuando se intento graficar: " + x.ToString(), "error");
            }
        }

        private string GraficaNodos(ParseTreeNode nodo, string i)
        {
            int k = 0;
            string r = "";
            string nodoTerm = nodo.Term.ToString();
            nodoTerm = nodoTerm.Replace("\"", "");
            nodoTerm = nodoTerm.Replace("\\", "\\\\");
            r = "node" + i + "[label = \"" + nodoTerm + "\"];\n";

            for (int j = 0; j <= nodo.ChildNodes.Count() - 1; j++)
            {  // Nodos padres
                r = r + "node" + i + " -> node" + i + k + "\n";
                r = r + GraficaNodos(nodo.ChildNodes[j], "" + i + k);
                k++;
            }

            if (nodo.Token != null)
            {
                string nodoToken = nodo.Token.Text;
                nodoToken = nodoToken.Replace("\"", "");
                nodoToken = nodoToken.Replace("\\", "\\\\");
                if (nodo.ChildNodes.Count() == 0 && nodoTerm != nodoToken)
                {  // Nodos Hojas
                    r += "node" + i + "c[label = \"" + nodoToken + "\"];\n";
                    r += "node" + i + " -> node" + i + "c\n";
                }
            }

            return r;
        }

        private void PdfErrors()
        {
            try
            {
                int no = 1;
                StreamWriter sw;
                string fileNameerror = @"errores.html";
                sw = File.CreateText(fileNameerror);
                sw.WriteLine("<html>");
                sw.WriteLine("<title>Reporte de Errores</title>");
                sw.WriteLine("<center><head><font color=white><h1>REPORTE DE ERRORES</h1></font></head></center>");
                sw.WriteLine("<body background=noche.jpg bgcolor= black>");
                sw.WriteLine("<center><table border= 5>");
                sw.WriteLine("<tr><td><font color=white><center>No.</center></font></td><td><font color=white><center>LÍNEA</center></font></td>" + 
                    "<td><font color= white><center>COLUMNA</center></font></td><td><font color= white><center>TIPO</center></font></td>" + 
                    "<td><font color= white><center>DESCRIPCIÓN</center></font></td></tr>");


                foreach(Error error in error_list)
                {
                    sw.WriteLine("<tr><td><font color=white><center>" + no + "</center></font></td><td><font color=white><center>" +
                        error.line + "</center></font></td><td><font color= white><center>" + error.column + "</center></font></td><td><font color=white><center>" 
                        + error.type + "</center></font></td><td><font color= white><center>" + error.description + "</center></font></td></tr>");
                    no += 1;
                }

                foreach (Error error in this.ejec.error)
                {
                    sw.WriteLine("<tr><td><font color=white><center>" + no + "</center></font></td><td><font color=white><center>" +
                        error.line + "</center></font></td><td><font color= white><center>" + error.column + "</center></font></td><td><font color=white><center>"
                        + error.type + "</center></font></td><td><font color= white><center>" + error.description + "</center></font></td></tr>");
                    no += 1;
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
                MessageBox.Show("Ha ocurrido un error.Verifique....", "ERROR",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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


                foreach (var s in this.ent)
                {
                    foreach (var simbol in s.var)
                    {
                        sw.WriteLine("<tr><td><font color=white><center>" + no + "</center></font></td><td><font color=white><center>" +
                            simbol.Linea + "</center></font></td><td><font color= white><center>" + simbol.Columna + "</center></font></td><td><font color=white><center>"
                            + simbol.Nombre + "</center></font></td><td><font color= white><center>" + simbol.Tipo
                            + "</center></font></td><td><font color= white><center>" + simbol.Ambito + "</center></font></td></tr>");
                        no += 1;
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
                MessageBox.Show("Ha ocurrido un error.Verifique....", "ERROR",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EJECCUIONTOTAL()
        {
            RecorridoAST pasada = new RecorridoAST();
            ent = pasada.Recorriendo(raiz);

            if (ent != null)
            {
                Debug.WriteLine("Iniciando Recorrido del AST, Entorno 0\n");
                Debug.WriteLine(ent.Count());
                foreach(var i in ent)
                {
                    foreach (var item in i.var)
                    {
                        Debug.WriteLine("Variables reconocidas: {0}, de tipo: {1}, de valor: {2}", item.Nombre, item.Tipo, item.Valor);
                    }
                }
                ejec = new Ejecucionn(ent);
                ejec.Procedure();

                ViewErrors();
            }
            else
            {
                SalidaErrores.AppendText("**ERROR FATAL** Arbol retornó nulo\n");
            }
        }

        public Form1()
        {
            InitializeComponent();
            Salida_Inst = Salida;
            Consola_Inst = SalidaErrores;
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Name == "toolStripMenuItem1")
            {
                MessageBox.Show("Optimizando...");
            }
            else if (e.ClickedItem.Name == "toolStripMenuItem2")
            {
                if (EDITOR.Text != "")
                {
                    if (analizar(EDITOR.Text))
                    {
                        Salida.AppendText("Análisis Exitoso\n");
                        //EJECCUIONTOTAL(); //P1
                        Salida.AppendText(COMPI2());
                    }
                    else
                    {
                        ViewErrors();
                    }
                }
                else
                {
                    MessageBox.Show("No hay nada que analizar");
                }
            }
            else if (e.ClickedItem.Name == "toolStripMenuItem3")
            {
                ViewSymbolTable();
                PdfTS();
            }
            else if (e.ClickedItem.Name == "toolStripMenuItem4")
            {
                EDITOR.Clear();
                Salida.Clear();
                SalidaErrores.Clear();
            }
            else if (e.ClickedItem.Name == "toolStripMenuItem5")
            {
                EDITOR.Clear();
                Salida.Clear();
                SalidaErrores.Clear();
            }
            else if (e.ClickedItem.Name == "toolStripMenuItem7")
            {
                if(raiz == null)
                {
                    MessageBox.Show("Error al crear la gráfica del AST");
                }
                else
                {
                    GraficarArbol(raiz);
                }
            }
            else if (e.ClickedItem.Name == "toolStripMenuItem8")
            {
                SalidaErrores.Clear();
                ViewErrors();
                PdfErrors();
            }
        }

        // ---------------------------------------- PROYECTO 2 ---------------------------------------------------
        private string COMPI2()
        {
            RecorriendoC3D AST = new RecorriendoC3D();
            enviroment = AST.Rec(raiz);

            if (enviroment != null)
            {
                Debug.WriteLine("Iniciando recorrrido del AST, Entorno 0\n");
            }
            else
            {
                SalidaErrores.AppendText("**ERROR FATAL** Arbol retornó nulo\n");
            }

            //    Debug.WriteLine("Iniciando Recorrido del AST, Entorno 0\n");
            //    Debug.WriteLine(ent.Count());
            //    foreach (var i in ent)
            //    {
            //        foreach (var item in i.var)
            //        {
            //            Debug.WriteLine("Variables reconocidas: {0}, de tipo: {1}, de valor: {2}", item.Nombre, item.Tipo, item.Valor);
            //        }
            //    }
            //    ejec = new Ejecucionn(ent);
            //    ejec.Procedure();

            //    ViewErrors();
            C3D generator = C3D.getGenerator();
            string output = generator.getCode();
            return output;
        }
    }
}
