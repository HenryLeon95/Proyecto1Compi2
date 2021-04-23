using System;
using System.Collections.Generic;
using System.Text;
using Irony.Parsing;

namespace Proyecto1.Proyecto2.Compilador
{
    class RecorriendoC3D
    {
        private List<EntornoC> list_entornos;
        private string ambito = "";

        public RecorriendoC3D()
        {
            this.list_entornos = new List<EntornoC>();
            this.ambito = "raiz";
        }

        public List<EntornoC> Rec(ParseTreeNode Nodo)
        {
            if (Nodo != null)
            {
                switch (Nodo.Term.Name)
                {
                    case "inicio":
                        EntornoC ent = new EntornoC(Nodo, ambito);
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
