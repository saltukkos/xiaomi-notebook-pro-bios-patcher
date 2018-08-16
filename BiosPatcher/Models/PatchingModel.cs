using System;
using System.Diagnostics;
using System.IO;
using JetBrains.Annotations;

namespace BiosPatcher.Models
{
    public sealed class PatchingModel
    {
        private const int FirstTableOffset = 0x155AF;
        private const int SecondTableOffset = 0x155C7;
        private const int DataLengthOffset = 0x18;
        private const int CheckSumOffset = 0x44;
        private const int HeaderLength = 0x100;
        private const int ChecksumDivider = 0x40;
        private const int CopyrightOffset = 0x564;

        //Copyright 1996-1999, all rights reserved Insyde Software Corp.
        private static readonly byte[] CopyrightString =
        {
            0x43, 0x6F, 0x70, 0x79, 0x72, 0x69, 0x67, 0x68, 0x74, 0x20, 0x31, 0x39,
            0x39, 0x36, 0x2D, 0x31, 0x39, 0x39, 0x39, 0x2C, 0x20, 0x61, 0x6C, 0x6C,
            0x20, 0x72, 0x69, 0x67, 0x68, 0x74, 0x73, 0x20, 0x72, 0x65, 0x73, 0x65,
            0x72, 0x76, 0x65, 0x64, 0x0A, 0x0D, 0x49, 0x6E, 0x73, 0x79, 0x64, 0x65,
            0x20, 0x53, 0x6F, 0x66, 0x74, 0x77, 0x61, 0x72, 0x65, 0x20, 0x43, 0x6F,
            0x72, 0x70, 0x2E, 0x00
        };

        static PatchingModel()
        {
            Debug.Assert(SecondTableOffset > FirstTableOffset);
            Debug.Assert(FirstTableOffset > HeaderLength);
            Debug.Assert(CheckSumOffset + 4 < HeaderLength);
            Debug.Assert(DataLengthOffset + 4 < HeaderLength);
        }

        [NotNull]
        private const string FileSignatureDiffersFromSupportedBios = "File signature differs from supported bios";

        [CanBeNull]
        private byte[] _image;

        [CanBeNull]
        private string _outFileName;

        [NotNull]
        public int[] OffLevels { get; } = new int[5];

        [NotNull]
        public int[] OnLevels { get; } = new int[5];

        [NotNull]
        public string LastError { get; private set; } = string.Empty;

        private int _currentFirstTableOffset = FirstTableOffset;
        private int _currentSecondTableOffset = SecondTableOffset;
        private bool _inputCorrect;

        public PatchingModel()
        {
            Array.Copy(DefaultValues.OffLevels, OffLevels, 5);
            Array.Copy(DefaultValues.OnLevels, OnLevels, 5);
            CheckInputCorrect();
        }

        public int CustomOffset
        {
            get { return _currentFirstTableOffset; }
            set
            {
                _currentFirstTableOffset = value;
                _currentSecondTableOffset = value + (SecondTableOffset - FirstTableOffset);
                _inputCorrect = CheckInputCorrect();
            }
        }

        public void SetInputFilePath([NotNull] string filename)
        {
            _image = null;
            if (File.Exists(filename))
            {
                _image = File.ReadAllBytes(filename);
            }

            _inputCorrect = CheckInputCorrect();
        }

        public void SetOutputFilePath([NotNull] string filename)
        {
            _outFileName = filename;
        }

        public bool CheckCorrect()
        {
            if (!_inputCorrect)
            {
                return false;
            }

            if (string.IsNullOrEmpty(_outFileName))
            {
                LastError = "Output path is not specified";
                return false;
            }

            for (int i = 0; i < 5; ++i)
            {
                if (OffLevels[i] >= OnLevels[i])
                {
                    LastError = $"Level {i + 1} activate threshold is not bigger than deactivate";
                    return false;
                }
            }

            for (int i = 1; i < 5; ++i)
            {
                if (OffLevels[i] <= OnLevels[i - 1])
                {
                    LastError = $"Level {i} deactivate threshold is not bigger than level {i - 1} activate threshold";
                    return false;
                }
            }

            LastError = string.Empty;
            return true;
        }

        public string Patch()
        {
            if (!CheckCorrect())
            {
                return "Invalid settings!";
            }

            // ReSharper disable once PossibleNullReferenceException
            var newImage = new byte[_image.Length];
            Array.Copy(_image, newImage, newImage.Length);


            for (int i = 0; i < 10; ++i)
            {
                var currentArray = i % 2 == 0 ? OnLevels : OffLevels;
                var newValue = currentArray[i / 2];

                newImage[_currentFirstTableOffset + i] = (byte) newValue;
                newImage[_currentSecondTableOffset + i] = (byte) newValue;
            }

            var dataLength = 2 * BitConverterLittleEndian.ReadInt(newImage, DataLengthOffset);
            var checksum = CheckSumCalculator.Calculate(newImage, HeaderLength, dataLength);

            var modulo = checksum % ChecksumDivider;
            if (modulo != 0)
            {
                newImage[CopyrightOffset + CopyrightString.Length - 2] = 0;
                newImage[CopyrightOffset + CopyrightString.Length - 1] =
                    (byte) (CopyrightString[CopyrightString.Length - 2] + ChecksumDivider - modulo);
            }

            var correctedChecksum = CheckSumCalculator.Calculate(newImage, HeaderLength, dataLength);
            if (correctedChecksum % ChecksumDivider != 0)
            {
                return "Internal error";
            }

            BitConverterLittleEndian.WriteInt(correctedChecksum, newImage, CheckSumOffset);

            // ReSharper disable once AssignNullToNotNullAttribute
            try
            {
                File.WriteAllBytes(_outFileName, newImage);
            }
            catch (Exception e)
            {
                return $"Unable to write patched image: {e}";
            }

            return "Image patched successfully";
        }

        private bool CheckInputCorrect()
        {
            if (_image is null)
            {
                LastError = "Can't read input file";
                return false;
            }

            if (_image.Length < _currentSecondTableOffset + 10)
            {
                LastError = "File length is smaller than offset to fan speed tables";
                return false;
            }

            var dataLength = 2 * BitConverterLittleEndian.ReadInt(_image, DataLengthOffset);
            if (_image.Length < dataLength + HeaderLength)
            {
                LastError = FileSignatureDiffersFromSupportedBios;
                return false;
            }

            var actualChecksum = CheckSumCalculator.Calculate(_image, HeaderLength, dataLength);
            var writtenChecksum = BitConverterLittleEndian.ReadInt(_image, CheckSumOffset);

            if (actualChecksum != writtenChecksum || actualChecksum % ChecksumDivider != 0)
            {
                LastError = FileSignatureDiffersFromSupportedBios;
                return false;
            }

            if (_image.Length < CopyrightString.Length + CopyrightOffset)
            {
                LastError = FileSignatureDiffersFromSupportedBios;
                return false;
            }

            for (int i = 0; i < CopyrightString.Length; ++i)
            {
                if (_image[CopyrightOffset + i] != CopyrightString[i])
                {
                    LastError = (i == CopyrightString.Length - 2 && _image[CopyrightOffset + i] == 0) ?
                        "Image was patched" :
                        FileSignatureDiffersFromSupportedBios;
                    return false;
                }
            }

            for (int i = 0; i < 10; ++i)
            {
                var currentArray = i % 2 == 0 ? DefaultValues.OnLevels : DefaultValues.OffLevels;
                var defaultValue = currentArray[i / 2];
                if (_image[_currentFirstTableOffset + i] != defaultValue ||
                    _image[_currentSecondTableOffset + i] != defaultValue)
                {
                    LastError = "Speed tables have incorrect values (image was patched or offset is incorrect)";
                    return false;
                }
            }

            return true;
        }
    }
}