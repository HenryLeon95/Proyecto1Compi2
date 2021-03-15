using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Proyecto1.Ejecucion
{
    class TablaSimbolos
    {
        public int Level;
        public string Type;
        public bool ReturnVal;
        public bool Stop;
        public bool Return;
        public List<Simbolo> ts = new List<Simbolo>();

        public TablaSimbolos(int Level, string Type, bool ReturnVal, bool Stop, bool Return)
        {
            this.Level = Level;
            this.Type = Type;
            this.ReturnVal = ReturnVal;
            this.Stop = Stop;
            this.Return = Return;
        }

        public void Add(string linea, string columna, string ambito, string nombre, string valor, string tipo, string tipoobjeto, bool visibilidad, List<ARR> arreglo)
        {
            ts.Add(new Simbolo(linea, columna, ambito, nombre, valor, tipo, tipoobjeto, visibilidad, arreglo));
        }

        public bool empty()
        {
            if (!ts.Any())
            {
                return true;
            }

            return false;
        }

        public List<Simbolo> Get()
        {
            if (!empty())
            {
                return ts;
            }

            return null;
        }

        public bool exist(string name)
        {
            if (!empty())
            {
                foreach (Simbolo simbolo in ts)
                {
                    if (simbolo.Nombre.Equals(name))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public Simbolo ReturnSymbol(string name)
        {
            if (!empty())
            {
                foreach (Simbolo simbolo in ts)
                {
                    if (simbolo.Nombre.Equals(name))
                    {
                        return simbolo;
                    }
                }
            }
            return null;
        }

        public void Clean()
        {
            ts.Clear();
        }
    }
}
