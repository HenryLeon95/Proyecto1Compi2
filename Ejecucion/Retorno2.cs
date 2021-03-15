using System;
using System.Collections.Generic;
using System.Text;

namespace Proyecto1.Ejecucion
{
    class Retorno2
    {
        public string Type;
        public string Value;
        public string Line;
        public string Column;
        public bool ReturnVal;
        public bool Stop;
        public bool Return;

        public Retorno2(string Type, string Value, string Line, string Column)
        {
            this.Type = Type;
            this.Value = Value;
            this.Line = Line;
            this.Column = Column;
            this.ReturnVal = false;
            this.Stop = false;
            this.Return = false;
        }
    }
}
