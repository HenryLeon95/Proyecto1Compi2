using System;
using System.Collections.Generic;
using System.Text;

namespace Proyecto1.Ejecucion
{
    class Simbolo
    {
        public string Ambito;
        public string Nombre;
        public string Valor; //PARA UN ARREGLO EL VALOR SERAN LAS DIMENSIONES
        public string Tipo;
        public string TipoObjeto; //VARIABLE || ARREGLO
        public string Linea;
        public string Columna;
        public bool flag;
        public List<ARR> Arreglo;

        public Simbolo(string linea, string columna, string ambito, string nombre, string valor, string tipo, string tipoobjeto, bool flag, List<ARR> Arreglo)
        {
            Linea = linea;
            Columna = columna;
            Ambito = ambito;
            Nombre = nombre;
            Valor = valor;
            Tipo = tipo;
            TipoObjeto = tipoobjeto;
            this.flag = flag;
            this.Arreglo = Arreglo;
        }
    }
}
