using System;
using System.Collections.Generic;
using System.Text;

namespace Proyecto1.Proyecto2.Compilador
{
    class Symbol
    {
        public string Nombre;
        public string Tipo;
        public string TipoObjeto; //VARIABLE || ARREGLO
        public string Ambito;
        public string Valor; //PARA UN ARREGLO EL VALOR SERAN LAS DIMENSIONES
        public string Linea;
        public string Columna;
        public string Rol;
        public int Apuntador;
        public bool flag;
        //public List<ARR> Arreglo;

        public Symbol(string nombre, string tipo, string tipoobjeto, string ambito, string valor, string linea, string columna, string rol, int ap, bool flag)
        {
            Linea = linea;
            Columna = columna;
            Ambito = ambito;
            Nombre = nombre;
            Valor = valor;
            Tipo = tipo;
            TipoObjeto = tipoobjeto;
            this.flag = flag;
            Rol = rol;
            Apuntador = ap;
        }
    }
}
