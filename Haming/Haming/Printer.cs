using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haming
{
    class Printer
    {
        
        public void PrintHamingCode(List<bool> hamingCode,int size)
        {
            for (int m = 0; m < hamingCode.Count; m++)
            {
                if (hamingCode[m])
                {
                    Console.Write("1");
                }
                else
                {
                    Console.Write("0");
                }
                if ((m + 1) % size == 0)
                {
                    Console.WriteLine();
                }                
            }           
        }

        public void ErrorMesBit(int bit, int packet)
        {
            Console.WriteLine("Error in " + bit + " bit, in "+packet+" packet.");
        }

        public void PrintMes(string mes)
        {
            Console.WriteLine(mes);
        }
    }
}
