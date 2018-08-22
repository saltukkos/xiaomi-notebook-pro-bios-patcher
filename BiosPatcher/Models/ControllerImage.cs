using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using BiosPatcher.Models.Exceptions;
using JetBrains.Annotations;

namespace BiosPatcher.Models
{
    internal sealed class ControllerImage
    {
        private const int DataLengthOffset = 0x18;
        private const int CheckSumOffset = 0x44;
        private const int HeaderLength = 0x100;
        private const int ChecksumLittleByte = 0x40;
        private const int CopyrightOffset = 0x564;
        private const int TablesCount = 2;
        private const int TableLength = 10;

        [NotNull]
        private static readonly byte[] TableEnding = 
        {
            0x7F, 0x00, 0x00, 0x0F, 0x00, 0x31, 0x00, 0x52, 0x00, 0x73, 0x00, 0x98,
            0x00, 0x00
        };

        //Copyright 1996-1999, all rights reserved Insyde Software Corp.
        [NotNull]
        private static readonly byte[] CopyrightString =
        {
            0x43, 0x6F, 0x70, 0x79, 0x72, 0x69, 0x67, 0x68, 0x74, 0x20, 0x31, 0x39,
            0x39, 0x36, 0x2D, 0x31, 0x39, 0x39, 0x39, 0x2C, 0x20, 0x61, 0x6C, 0x6C,
            0x20, 0x72, 0x69, 0x67, 0x68, 0x74, 0x73, 0x20, 0x72, 0x65, 0x73, 0x65,
            0x72, 0x76, 0x65, 0x64, 0x0A, 0x0D, 0x49, 0x6E, 0x73, 0x79, 0x64, 0x65,
            0x20, 0x53, 0x6F, 0x66, 0x74, 0x77, 0x61, 0x72, 0x65, 0x20, 0x43, 0x6F,
            0x72, 0x70, 0x2E, 0x00
        };

        [NotNull]
        private readonly byte[] _image;

        static ControllerImage()
        {
            Debug.Assert(DataLengthOffset + 4 < HeaderLength);
        }

        public ControllerImage([NotNull] IEnumerable<byte> data)
        {
            _image = data.ToArray();
            CheckSignatures();
            DetectFanTables();
        }

        public IReadOnlyList<byte> Image => _image;

        private void CheckSignatures()
        {
            if (_image.Length < HeaderLength)
            {
                throw new IncorrectSignatureException("Header is not present");
            }

            var dataLength = 2 * BitConverterLittleEndian.ReadInt(_image, DataLengthOffset);
            if (_image.Length < dataLength + HeaderLength)
            {
                throw new IncorrectSignatureException("Data length is bigger than file size");
            }

            var actualChecksum = CheckSumCalculator.Calculate(_image, HeaderLength, dataLength);
            var writtenChecksum = BitConverterLittleEndian.ReadInt(_image, CheckSumOffset);

            if (actualChecksum != writtenChecksum || actualChecksum % 256 != ChecksumLittleByte)
            {
                throw new IncorrectSignatureException("Checksum fault");
            }

            if (_image.Length < CopyrightString.Length + CopyrightOffset)
            {
                throw new IncorrectSignatureException("Copyright is not present");
            }

            for (var i = 0; i < CopyrightString.Length; ++i)
            {
                if (_image[CopyrightOffset + i] == CopyrightString[i])
                {
                    continue;
                }

                if (i == CopyrightString.Length - 2 && _image[CopyrightOffset + i] == 0)
                {
                    throw new IncorrectSignatureException("Image was already patched");
                }

                throw new IncorrectSignatureException("Can't find copyright string");
            }
        }

        private void DetectFanTables()
        {
            var indices = _image.FindSubArrayWithUniqueLeadingElement(TableEnding);
            if (indices.Count != TablesCount)
            {
                throw new TemperatureTablesException("Can't determine position of tables");
            }

            for (var i = 1; i < TablesCount; ++i)
            {
                if (indices[i] - indices[i - 1] != TableEnding.Length + TableLength)
                {
                    throw new TemperatureTablesException("Offset between tables is incorrect");
                }

                if (!_image.IsSubArrayEquals(0, TableLength, _image, indices[i]))
                {
                    throw new TemperatureTablesException("Temperature tables are not equal");
                }
            }

            if (!_image.IsStrictlyIncreasing(indices[0], TableLength))
            {
                throw new TemperatureTablesException("Temperature tables have incorrect values");
            }
        }
    }
}