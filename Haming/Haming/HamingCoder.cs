using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haming
{
    class HamingCoder
    {
        public const int PacketSize = 2;
        public const int Binary = 2;
        public const int MaxInfBitNumber = 16;
        public const int MaxPacketLength = 21;
        public const int BitInBye = 8;

        public List<bool> EncodeAsBinary(string message)
        {
            List<bool> hamingCode = new List<bool>();
            for (int i = 0; i < message.Length; i++)
            {
                hamingCode.AddRange(CharAsListOfBool(message[i]));
            }
            return hamingCode;
        }

        private List<bool> CharAsListOfBool(char c)
        {
            List<bool> list = new List<bool>();
            list.AddRange(ByteToListOfBool(Convert.ToByte(c)));
            return list;
        }

        private List<bool> ByteToListOfBool(byte data)
        {
            return new List<bool>{
                      (byte)(data & 0x80) == 128,
                      (byte)(data & 0x40) == 64,
                      (byte)(data & 0x20) == 32,
                      (byte)(data & 0x10) == 16,
                      (byte)(data & 0x08) == 8,
                      (byte)(data & 0x04) == 4,
                      (byte)(data & 0x02) == 2,
                      (byte)(data & 0x01) == 1
            };
        }

        public List<bool> EncodeAsHaming(List<bool> binaryCode)
        {
            List<bool> hamingCode = new List<bool>();
            for (int i = 0; i < binaryCode.Count; i += MaxInfBitNumber)
            {
                hamingCode.AddRange(EncodePacket(AddInfoBit(binaryCode.GetRange(i, MaxInfBitNumber))));             
            }
            return hamingCode;
        }

        private List<bool> AddInfoBit(List<bool> binaryCode)
        {
            List<bool> hamingCode = new List<bool>();
            hamingCode.AddRange(binaryCode);
            for(int i = 1; i<= MaxInfBitNumber;i*= Binary)
            {
                hamingCode.Insert(i - 1, false);
            }
            return hamingCode;
        }

        private List<bool> EncodePacket(List<bool> hamingCode)
        {
            for (int k = 1; k <= MaxInfBitNumber; k *= Binary)
            {
                int counter = 0;
                for (int l = k-1; l < MaxPacketLength; l += k)
                {
                    int a = k;
                    for (int n = l; a > 0 && n < MaxPacketLength; n++, a--, l++)
                    {
                        if (hamingCode[n])
                        {
                            counter++;
                        }
                    }
                }
                 hamingCode[k - 1] = counter % 2 == 1;
            }
            return hamingCode;
        }
        
        public List<bool> ToDoError(List<bool> hamingCode)
        {
            List<bool> errorHamingCode = new List<bool>();
            errorHamingCode.AddRange(hamingCode);
            Random random = new Random();
            for (int i = 0;i< hamingCode.Count;i+= MaxPacketLength)
            {
                int index = random.Next(MaxPacketLength);
                errorHamingCode[i + index] = !errorHamingCode[i + index];
            }
            return errorHamingCode;
        }

        public string Decode(List<bool> hamingErrorCode)
        {
            Printer printer = new Printer();
            
            List<bool> hamingControlCode = new List<bool>();
            for (int i = 0;i< hamingErrorCode.Count;i+= MaxPacketLength)
            {
                hamingControlCode.AddRange(EncodePacket(hamingErrorCode.GetRange(i, MaxPacketLength)));
            }
            for (int i = 0; i < hamingControlCode.Count / MaxPacketLength; i++)
            {
                int errorCounter = 0;
                for (int j = 1;j<= MaxInfBitNumber; j*=Binary)
                {
                    if(hamingControlCode[i* MaxPacketLength+j-1])
                    {
                        errorCounter +=  j;
                    }
                }
                printer.ErrorMesBit(errorCounter, i);
                hamingErrorCode[errorCounter - 1 + i* MaxPacketLength] = hamingErrorCode[errorCounter - 1+ i * MaxPacketLength] ? false : true;
            }
            return DecodeHaming(hamingErrorCode);
        }

        private string DecodeHaming(List<bool> hamingCode)
        {
            List<bool> binaryCode = new List<bool>();
            for (int i = 0; i < hamingCode.Count; i += MaxPacketLength)
            {
                binaryCode.AddRange(RemoveInfoBit(hamingCode.GetRange(i, MaxPacketLength)));
            }
            string st = "";
            for(int i = 0; i < binaryCode.Count; i += BitInBye)
            {
                st += (char)ListOfBoolToByte(binaryCode.GetRange(i, BitInBye));
            }
            return st;
        }

        private List<bool> RemoveInfoBit(List<bool> hamingCode)
        {
            List<bool> code = new List<bool>();
            code.AddRange(hamingCode);
            for (int i = MaxInfBitNumber; i>=1; i /= Binary)
            {
                code.RemoveAt(i - 1);
            }
            return code;
        }

        private byte ListOfBoolToByte(List<bool> list)
        {
            byte data = 0;
            data = list[0] ? (byte)(data | 0x80) : data;
            data = list[1] ? (byte)(data | 0x40) : data;
            data = list[2] ? (byte)(data | 0x20) : data;
            data = list[3] ? (byte)(data | 0x10) : data;
            data = list[4] ? (byte)(data | 0x08) : data;
            data = list[5] ? (byte)(data | 0x04) : data;
            data = list[6] ? (byte)(data | 0x02) : data;
            data = list[7] ? (byte)(data | 0x01) : data;
            return data;
        }
    }
}
