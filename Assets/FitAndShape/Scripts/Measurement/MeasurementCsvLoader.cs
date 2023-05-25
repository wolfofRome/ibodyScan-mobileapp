using System.Collections.Generic;

namespace FitAndShape
{
    public interface IMeasurementCsvLoader
    {
        int GetValue(MeasurementPart measurementPart);
    }

    public sealed class MeasurementCsvLoader : IMeasurementCsvLoader
    {
        private readonly Dictionary<MeasurementPart, int> _csvDictionary;

        public MeasurementCsvLoader(Dictionary<string, string> csvRow)
        {
            _csvDictionary = new Dictionary<MeasurementPart, int>();

            int index = 0;

            foreach (var (key, value) in csvRow)
            {
                int.TryParse(value, out int convertValue);

                _csvDictionary.Add((MeasurementPart)index, convertValue);

                index++;
            }
        }

        int IMeasurementCsvLoader.GetValue(MeasurementPart measurementPart)
        {
            return _csvDictionary[measurementPart];
        }
    }
}