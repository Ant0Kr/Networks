using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace COMChat
{
    class Packer
    {
        private byte StartStopByte = 0x7E;
        private byte SourceByte;
        private byte DestinationByte;

        public Packer(byte sourceByte, byte destinationByte)
        {
            SourceByte = sourceByte;
            DestinationByte = destinationByte;
        }

        public byte[] Packing(byte[] data)
        {
            List<bool> package = CreatePackageOfBool(data);
            return BoolToBytePackage(package);
        }

        private List<bool> CreatePackageOfBool(byte[] data)
        {
            List<bool> package = new List<bool>();
            package.AddRange(ByteToListOfBool(StartStopByte));
            package.AddRange(ByteToListOfBool(DestinationByte));
            package.AddRange(ByteToListOfBool(SourceByte));
            package.AddRange(EncodeData(ByteArrayToListOfBool(data)));
            package.AddRange(ByteToListOfBool(StartStopByte));
            return package;
        }

        private byte[] BoolToBytePackage(List<bool> package)
        {
            MemoryStream stream = new MemoryStream();
            new BinaryFormatter().Serialize(stream, package);
            return stream.ToArray();
        }

        private List<bool> EncodeData(List<bool> data)
        {
            for (int i = 0; i < data.Count; i += 8)
            {
                if (IsBitToAdd(data.GetRange(i, 8)))
                {
                    data.Insert(i + 7, true);
                    i++;
                }
            }
            return data;
        }

        private bool IsBitToAdd(List<bool> data)
        {
            return !data[0]
                    && data[1] && data[2]
                    && data[3] && data[4]
                    && data[5] && data[6];
        }

        private List<bool> ByteArrayToListOfBool(byte[] array)
        {
            List<bool> list = new List<bool>();
            foreach (var _byte in array)
            {
                list.AddRange(ByteToListOfBool(_byte));
            }
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

        public byte[] Unpacking(byte[] data)
        {
            List<bool> boolListData = GetListOfBoolFromByteArray(data);
            if (ListOfBoolToByte(boolListData.GetRange(8, 16)) != SourceByte)
                return null;
            else
                return ListOfBoolToByteArray(DecodeData(boolListData.GetRange(24, boolListData.Count - 32)));
        }

        private List<bool> GetListOfBoolFromByteArray(byte[] array)
        {
            MemoryStream stream = new MemoryStream();
            stream.Write(array, 0, array.Length);
            stream.Position = 0;
            return new BinaryFormatter().Deserialize(stream) as List<bool>;
        }

        private List<bool> DecodeData(List<bool> data)
        {
            for (int i = 0; i < data.Count; i += 8)
            {
                if (IsBitDelete(data.GetRange(i, 8)))
                {
                    data.RemoveAt(i + 7); 
                }
            }
            return data;
        }

        private bool IsBitDelete(List<bool> data)
        {
            return !data[0] && data[1]
                 && data[2] && data[3]
                 && data[4] && data[5]
                 && data[6] && data[7];
        }

        private byte[] ListOfBoolToByteArray(List<bool> list)
        {
            byte[] array = new byte[list.Count / 8];
            for (int i = 0; i < list.Count; i += 8)
            {
                array[i / 8] = ListOfBoolToByte(list.GetRange(i, 8));
            }
            return array;
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
