using System;
using System.Collections.Generic;
using System.Text;
using Irony.Parsing;

namespace Proyecto1.Recorrido
{
    class RecorridoAST
    {
        private List<Entorno> list_entornos;
        private string ambito = "";

        public RecorridoAST()
        {
            this.list_entornos = new List<Entorno>();
            this.ambito = "raiz";
        }

        public List<Entorno> Recorriendo(ParseTreeNode Nodo)
        {
            if (Nodo != null)
            {
                switch (Nodo.Term.Name)
                {
                    case "inicio":
                        Entorno ent = new Entorno(Nodo, ambito);
                        ent.Inicio();
                        list_entornos.Add(ent);
                        break;
                    default:
                        Form1.Consola_Inst.AppendText("***Error Fatal*** " + Nodo.Term.Name + " no existente/detectado" + "\n");
                        break;
                }
            }
            return list_entornos;
        }
    }
}
