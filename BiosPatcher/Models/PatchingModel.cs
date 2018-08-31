using System;
using System.IO;
using System.Linq;
using BiosPatcher.Models.Exceptions;
using JetBrains.Annotations;

namespace BiosPatcher.Models
{
    public sealed class PatchingModel
    {
        [CanBeNull]
        private string _outFileName;

        [CanBeNull]
        private ControllerImage _image;

        [NotNull]
        public int[] OffLevels { get; } = new int[5];

        [NotNull]
        public int[] OnLevels { get; } = new int[5];

        [NotNull]
        public string CurrentStateDescription { get; private set; } = string.Empty;

        public PatchingModel()
        {
            Array.Copy(DefaultValues.OffLevels, OffLevels, 5);
            Array.Copy(DefaultValues.OnLevels, OnLevels, 5);
        }

        public int CustomOffset { get; set; }

        public void SetInputFilePath([NotNull] string filename)
        {
            _image = null;
            if (!File.Exists(filename))
            {
                CurrentStateDescription = "Input file is not exist";
                return;
            }

            try
            {
                var data = File.ReadAllBytes(filename);
                _image = new ControllerImage(data);
                CurrentStateDescription = string.Empty;
            }
            catch (IncorrectSignatureException e)
            {
                CurrentStateDescription = $"Incorrect file signature: {e.Message}";
            }
            catch (TemperatureTablesException e)
            {
                CurrentStateDescription = $"Error while parsing tables: {e.Message}";
            }
            catch (Exception)
            {
                CurrentStateDescription = "Unable to read input file";
            }
        }

        public void SetOutputFilePath([NotNull] string filename)
        {
            _outFileName = filename;
        }

        public bool CheckCorrect()
        {
            return CheckCorrectInternal(_image);
        }

        [ContractAnnotation("image:null=>false")]
        private bool CheckCorrectInternal([CanBeNull] ControllerImage image)
        {
            if (image == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(_outFileName))
            {
                CurrentStateDescription = "Output path is not specified";
                return false;
            }

            for (var i = 0; i < 5; ++i)
            {
                if (OffLevels[i] >= OnLevels[i])
                {
                    CurrentStateDescription = $"Level {i + 1} activate threshold is not bigger than deactivate";
                    return false;
                }
            }

            for (var i = 1; i < 5; ++i)
            {
                if (OffLevels[i] <= OnLevels[i - 1])
                {
                    CurrentStateDescription =
                        $"Level {i} deactivate threshold is not bigger than level {i - 1} activate threshold";
                    return false;
                }
            }

            CurrentStateDescription = string.Empty;
            return true;
        }

        [NotNull]
        public string Patch()
        {
            if (!CheckCorrectInternal(_image))
            {
                return "Invalid settings!";
            }

            var newImage = _image.Image.ToArray();
            var diff = 0;

            for (var i = 0; i < 10; ++i)
            {
                var currentArray = i % 2 == 0 ? OnLevels : OffLevels;
                var newValue = currentArray[i / 2];

                foreach (var offset in _image.TableOffsets)
                {
                    diff += newValue - newImage[offset + i];
                    newImage[offset + i] = checked((byte) newValue);
                }
            }

            var alignmentOffset = ImageMarkup.CopyrightOffset + ImageMarkup.CopyrightString.Count - 2;
            
            diff -= newImage[alignmentOffset];
            diff -= newImage[alignmentOffset + 1];
            newImage[alignmentOffset] = 0;

            var modulo = (byte)(256 - unchecked((uint) diff) % 256);
            newImage[alignmentOffset + 1] = modulo;
            diff += modulo;

            BitConverterLittleEndian.WriteInt(_image.Checksum + diff, newImage, ImageMarkup.CheckSumOffset);

            try
            {
                var unused = new ControllerImage(newImage);
            }
            catch
            {
                return "Internal error";
            }

            try
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                File.WriteAllBytes(_outFileName, newImage);
            }
            catch (Exception e)
            {
                return $"Unable to write patched image: {e}";
            }

            return "Image patched successfully";
        }
    }
}