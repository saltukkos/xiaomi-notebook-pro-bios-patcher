using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using BiosPatcher.Model.Exceptions;
using JetBrains.Annotations;

namespace BiosPatcher.Model
{
    [Component]
    internal sealed class ControllerImage : IControllerImage
    {
        [NotNull]
        private readonly byte[] _image;

        [NotNull]
        private readonly IBitConverterLittleEndian _bitConverter;

        [NotNull]
        private readonly ICheckSumCalculator _checkSumCalculator;

        static ControllerImage()
        {
            Debug.Assert(ImageMarkup.DataLengthOffset + 4 < ImageMarkup.HeaderLength);
        }

        // ReSharper disable once NotNullMemberIsNotInitialized -- incorrect inspection
        public ControllerImage(
            [NotNull] IEnumerable<byte> data,
            [NotNull] IBitConverterLittleEndian bitConverter,
            [NotNull] ICheckSumCalculator checkSumCalculator)
        {
            _bitConverter = bitConverter;
            _checkSumCalculator = checkSumCalculator;
            _image = data.ToArray();

            CheckSignatures();
            DetectFanTables();
        }

        [NotNull]
        public IReadOnlyList<byte> Image => _image;

        [NotNull]
        public IReadOnlyList<int> TableOffsets { get; private set; }

        public int Checksum { get; private set; }

        private void CheckSignatures()
        {
            if (_image.Length < ImageMarkup.HeaderLength)
            {
                throw new IncorrectSignatureException("Header is not present");
            }

            var dataLength = 2 * _bitConverter.ReadInt(_image, ImageMarkup.DataLengthOffset);
            if (_image.Length < dataLength + ImageMarkup.HeaderLength)
            {
                throw new IncorrectSignatureException("Data length is bigger than file size");
            }

            Checksum = _bitConverter.ReadInt(_image, ImageMarkup.CheckSumOffset);
            var actualChecksum = _checkSumCalculator.Calculate(_image, ImageMarkup.HeaderLength, dataLength);

            if (actualChecksum != Checksum || actualChecksum % 256 != ImageMarkup.ChecksumLittleByte)
            {
                throw new IncorrectSignatureException("Checksum fault");
            }

            if (_image.Length < ImageMarkup.CopyrightString.Count + ImageMarkup.CopyrightOffset)
            {
                throw new IncorrectSignatureException("Copyright is not present");
            }

            for (var i = 0; i < ImageMarkup.CopyrightString.Count; ++i)
            {
                if (_image[ImageMarkup.CopyrightOffset + i] == ImageMarkup.CopyrightString[i])
                {
                    continue;
                }

                if (i == ImageMarkup.CopyrightString.Count - 2 && _image[ImageMarkup.CopyrightOffset + i] == 0)
                {
                    //Image was already patched
                    break;
                }

                throw new IncorrectSignatureException("Can't find copyright string");
            }
        }

        private void DetectFanTables()
        {
            var indices = _image
                .FindSubListWithUniqueLeadingElement(ImageMarkup.TableEnding)
                .Select(index => index - ImageMarkup.TableLength)
                .ToList();

            if (indices.Count != ImageMarkup.TablesCount)
            {
                throw new TemperatureTablesException("Can't determine position of tables");
            }

            for (var i = 1; i < ImageMarkup.TablesCount; ++i)
            {
                if (indices[i] - indices[i - 1] != ImageMarkup.TableEnding.Count + ImageMarkup.TableLength)
                {
                    throw new TemperatureTablesException("Offset between tables is incorrect");
                }

                if (!_image.IsSubListEquals(indices[i - 1], ImageMarkup.TableLength, _image, indices[i]))
                {
                    throw new TemperatureTablesException("Temperature tables are not equal");
                }
            }

            if (!_image.IsAlternatingStrictlyIncreasing(indices[0], ImageMarkup.TableLength))
            {
                throw new TemperatureTablesException("Temperature tables have incorrect values");
            }

            TableOffsets = indices;
        }
    }
}