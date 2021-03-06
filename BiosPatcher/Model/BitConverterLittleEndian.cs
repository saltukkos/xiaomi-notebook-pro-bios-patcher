﻿using System;

namespace BiosPatcher.Model
{
    [Component]
    internal sealed class BitConverterLittleEndian : IBitConverterLittleEndian
    {
        public int ReadInt(byte[] bytes, int offset)
        {
            var data = new byte[4];
            Array.Copy(bytes, offset, data, 0, 4);
            
            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(data);
            }

            return BitConverter.ToInt32(data, 0);
        }

        public void WriteInt(int value, byte[] bytes, int offset)
        {
            var data = BitConverter.GetBytes(value);            
            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(data);
            }

            Array.Copy(data, 0, bytes, offset, data.Length);
        }
    }
}