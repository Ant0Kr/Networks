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
            string message = "wzjqdffg";
            List<bool> binaryCode = coder.EncodeAsBinary(message);
            List<bool> hamingCode = coder.EncodeAsHaming(binaryCode);
            List<bool> hamingWithError = coder.ToDoError(hamingCode);
            printer.PrintMes("Binary code:");
            printer.PrintHamingCode(binaryCode,16);
            printer.PrintMes("Haming code:");
            printer.PrintHamingCode(hamingCode, 21);
            printer.PrintMes("Haming code with error:");
            printer.PrintHamingCode(hamingWithError, 21);
            printer.PrintMes("Source message:"+coder.Decode(hamingWithError));
        }
    }
}
