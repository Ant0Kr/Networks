using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haming
{
    class Program
    {
        static void Main(string[] args)
        {
            HamingCoder coder = new HamingCoder();
            Printer printer = new Printer();
            string message = "wzjq";
            List<bool> binaryCode = coder.EncodeAsBinary(message);
            List<bool> hamingCode = coder.EncodeAsHaming(binaryCode);
            List<bool> binaryWithError = coder.ToDoError(binaryCode);
            List<bool> hamingWithError = coder.EncodeAsHaming(binaryWithError);
            printer.PrintMes("Binary code:");
            printer.PrintHamingCode(binaryCode,16);
            printer.PrintMes("Binary code with error:");
            printer.PrintHamingCode(binaryWithError, 16);
            printer.PrintMes("Haming code:");
            printer.PrintHamingCode(hamingCode,21);
            printer.PrintMes("Haming code with error:");
            printer.PrintHamingCode(hamingWithError, 21);
            printer.PrintMes("Source message:"+coder.Decode(hamingCode,hamingWithError));
        }
    }
}
