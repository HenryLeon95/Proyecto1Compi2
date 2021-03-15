using System;
using System.Collections.Generic;
using System.Text;

namespace Proyecto1.Analizador
{
    class Error
    {
        public int line;    //aumentar en 1 en lexico y sintactica
        public int column;  //aumentar en 1 en lexico y sintactica
        public string type;
        public string description;

        public Error()
        {
            this.line = 0;
            this.column = 0;
            this.type = "";
            this.description = "";
        }

        public Error(int line, int column, string type, string description)
        {
            this.line = line;
            this.column = column;
            this.type = type;
            this.description = description;
        }
    }
}
