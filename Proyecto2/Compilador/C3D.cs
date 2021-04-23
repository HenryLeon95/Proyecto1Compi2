using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;

namespace Proyecto1.Proyecto2.Compilador
{
    class C3D
    {
        private static C3D generator;
        private int Ntemporary;
        private int NLabel;
        private LinkedList<string> code;
        private LinkedList<string> temporaries;
        private LinkedList<string> tempList;
        public string isFunc;

        public C3D()
        {
            Clear();
        }

        public void Clear()
        {
            Ntemporary = NLabel = 0;
            temporaries = new LinkedList<string>();
            code = new LinkedList<string>();
            tempList = new LinkedList<string>();
        }

        public static C3D getGenerator()
        {
            if (generator == null)
            {
                generator = new C3D();
            }
            return generator;
        }

        public LinkedList<string> getTempAux()
        {
            return tempList;
        }

        public void clearAux()
        {
            tempList = new LinkedList<string>();
        }

        public void setTempaux(LinkedList<string> temp)
        {
            tempList = temp;
        }

        public void addCode(string code)
        {
            this.code.AddLast(isFunc + code);
        }

        public string generateTemps()
        {
            string ret = "";

            if (temporaries.Count > 0)
            {
                ret += "double ";
            }
            else
            {
                return "";
            }
            for (int i = 0; i < temporaries.Count; i++)
            {
                ret += temporaries.ElementAt(i);

                if (i <= temporaries.Count)
                {
                    ret += ",";
                }
            }

            ret += ";";
            return ret;
        }

        public string getHeader()
        {
            string ret =
@"#include <stdio.h>
double heap[30101999];
double stack[30101999];
double HP;
double SP;
" + generateTemps() + "\n";
            return ret;
            //return "#include <stdio.h>\ndouble heap[30101999];\ndouble stack[30101999];\ndouble HP;\ndouble SP;\n" + generateTemps() + "\n";
        }

        public string getCode()
        {
            string ret = getHeader() + "\nvoid main(){\n\tSP=0; HP=0;\n\n";

            foreach (string item in code)
            {
                ret += "\t" + item + "\n";
            }

            ret += "\n\treturn;\n}";
            return ret;
        }

        public void addSpace()
        {
            code.AddLast("\n");
        }

        public string newTemp()
        {
            string temp = "t" + Ntemporary++;
            tempList.AddLast(temp);
            temporaries.AddLast(temp);
            return temp;
        }

        public string newLabel()
        {
            return "L" + NLabel++;
        }

        public void addLabel(string label)
        {
            code.AddLast(isFunc + label + ":");
        }

        public void addExp(string target, string left, string rigth = "", string op = "")
        {
            code.AddLast(isFunc + target + " = " + left + op + rigth + ";");
        }

        public void addGoto(string label)
        {
            code.AddLast(isFunc + "goto " + label + ";");
        }

        public void addIf(string left, string right, string op, string label)
        {
            code.AddLast(isFunc + "if (" + left + op + right + ") goto " + label + ";");
        }

        public void addPrint(string format, string value)
        {
            code.AddLast(isFunc + "printf(\"%" + format + "\", " + value + ");");
        }

        public void nextHeap()
        {
            code.AddLast(isFunc + "HP = HP + 1;");
        }

        public void addGetHeap(string target, string index)
        {
            code.AddLast(isFunc + target + " = heap[" + index + "];");
        }

        public void addSetHeap(string index, string value)
        {
            code.AddLast(isFunc + "heap[" + index + "] = " + value + ";");
        }

        public void addGetStack(string target, string index)
        {
            code.AddLast(isFunc + target + " = stack[" + index + "];");
        }

        public void addSetStack(string index, string value)
        {
            code.AddLast(isFunc + "stack[" + index + "] = " + value + ";");
        }

        public void addNextEnv(string size)
        {
            code.AddLast(isFunc + "sp = sp + " + size); //p
        }

        public void addAntEnv(string size)
        {
            code.AddLast(this.isFunc + "sp = sp - " + size); //sp = p
        }

        public void addCallFunc(string id)
        {
            code.AddLast(id + "();");
        }

        public void addBeginFunc(string id)
        {
            if (isFunc == "")
            {
                isFunc = "\t";
                code.AddLast("void " + id + "(){");
            }
        }

        public void addEndFunc()
        {
            if (isFunc != "")
            {
                isFunc = "";
                code.AddLast("return;\n}");
            }
        }

        public void printTrue()
        {
            addPrint("c", "(char)116");
            addPrint("c", "(char)114");
            addPrint("c", "(char)117");
            addPrint("c", "(char)101");
        }

        public void printFalse()
        {
            addPrint("c", "(char)102");
            addPrint("c", "(char)97");
            addPrint("c", "(char)108");
            addPrint("c", "(char)115");
            addPrint("c", "(char)101");
        }
    }
}
