using System;
using System.Collections.Generic;

namespace FitAndShape
{
    public interface IMeasurement
    {
        int UserId { get; }
        int ResourceId { get; }
        long CreateTime { get; }
        long ModifiedTime { get; }
        string Type { get; }
        int Height { get; }
        int Inseam { get; }
        int Neck { get; }
        int Chest { get; }
        int MinWaist { get; }
        int MaxWaist { get; }
        int Hip { get; }
        int LeftThigh { get; }
        int RightThigh { get; }
        int LeftShoulder { get; }
        int RightShoulder { get; }
        int LeftSleeve { get; }
        int RightSleeve { get; }
        int LeftArm { get; }
        int RightArm { get; }
        int LeftCalf { get; }
        int RightCalf { get; }
        DateTime CreateTimeAsDateTime { get; }
        DateTime ModifiedTimeAsDateTime { get; }
        float correctionValue { get; }
        float correctedHeight { get; }
        float correctedInseam { get; }
    }

    public sealed class Measurement : IMeasurement
    {
        public readonly static string TypeSpaceVision = "space_vision";
        public readonly static string Type3DBodyLab = "3d_body_lab";

        public int UserId { get; }
        public int ResourceId { get; }
        public long CreateTime { get; }
        public long ModifiedTime { get; }
        public string Type { get; }
        public int Height { get; }
        public int Inseam { get; }
        public int Neck { get; }
        public int Chest { get; }
        public int MinWaist { get; }
        public int MaxWaist { get; }
        public int Hip { get; }
        public int LeftThigh { get; }
        public int RightThigh { get; }
        public int LeftShoulder { get; }
        public int RightShoulder { get; }
        public int LeftSleeve { get; }
        public int RightSleeve { get; }
        public int LeftArm { get; }
        public int RightArm { get; }
        public int LeftCalf { get; }
        public int RightCalf { get; }
        public DateTime CreateTimeAsDateTime => DateTimeExtension.UnixTimestamp2LocalTime(CreateTime);
        public DateTime ModifiedTimeAsDateTime => DateTimeExtension.UnixTimestamp2LocalTime(ModifiedTime);
        public float correctionValue { get; }
        public float correctedHeight => Height + correctionValue;
        public float correctedInseam => Inseam + correctionValue;

        public Measurement(Dictionary<string, string> csvRow)
        {

            ResourceId = ParseCsvValue(csvRow, MeasurementItem.ResourceId, 0);
            Height = ParseCsvValue(csvRow, MeasurementItem.Height, 0);
            Inseam = ParseCsvValue(csvRow, MeasurementItem.Inseam, 0);
            Neck = ParseCsvValue(csvRow, MeasurementItem.Neck, 0);
            Chest = ParseCsvValue(csvRow, MeasurementItem.Chest, 0);
            MinWaist = ParseCsvValue(csvRow, MeasurementItem.MinWaist, 0);
            MaxWaist = ParseCsvValue(csvRow, MeasurementItem.MaxWaist, 0);
            Hip = ParseCsvValue(csvRow, MeasurementItem.Hip, 0);
            LeftThigh = ParseCsvValue(csvRow, MeasurementItem.LeftThigh, 0);
            RightThigh = ParseCsvValue(csvRow, MeasurementItem.RightThigh, 0);
            LeftShoulder = ParseCsvValue(csvRow, MeasurementItem.LeftShoulder, 0);
            RightShoulder = ParseCsvValue(csvRow, MeasurementItem.RightShoulder, 0);
            LeftSleeve = ParseCsvValue(csvRow, MeasurementItem.LeftSleeve, 0);
            RightSleeve = ParseCsvValue(csvRow, MeasurementItem.RightSleeve, 0);
            LeftArm = ParseCsvValue(csvRow, MeasurementItem.LeftArm, 0);
            RightArm = ParseCsvValue(csvRow, MeasurementItem.RightArm, 0);
            LeftCalf = ParseCsvValue(csvRow, MeasurementItem.LeftCalf, 0);
            RightCalf = ParseCsvValue(csvRow, MeasurementItem.RightCalf, 0);
            Type = ParseCsvValue(csvRow, MeasurementItem.Type, TypeSpaceVision);
        }

        public override string ToString()
        {
            return $"[Measurement:  UserId={UserId}, ResourceId={ResourceId}, CreateTime={CreateTime}, ModifiedTime={ModifiedTime}, " +
                   $"Height={Height}, Inseam={Inseam}, Neck={Neck}, Chest={Chest}, MinWaist={MinWaist}, MaxWaist={MaxWaist}, Hip={Hip}, " +
                   $"LeftThigh={LeftThigh}, RightThigh={RightThigh}, LeftShoulder={LeftShoulder}, RightShoulder={RightShoulder}, LeftSleeve={LeftSleeve}, " +
                   $"RightSleeve={RightSleeve}, LeftArm={LeftArm}, RightArm={RightArm}, LeftCalf={LeftCalf}, RightCalf={RightCalf}]";
        }

        T ParseCsvValue<T>(IReadOnlyDictionary<string, string> csvRow, MeasurementItem part, T defaultValue)
        {
            T value = defaultValue;

            string key = GetCsvDataKey(part);

            try
            {
                if (csvRow.ContainsKey(key) && !string.IsNullOrEmpty(csvRow[key]))
                {
                    value = (T)Convert.ChangeType(csvRow[key], value.GetType());
                }
            }
            catch
            {
                throw new Exception("ParseCsvValue failed -> " + key + " type -> " + value.GetType() + " value -> [" + csvRow[key] + "]");
            }

            return value;
        }

        string GetCsvDataKey(MeasurementItem part)
        {
            return CsvKeys[part];
        }

        readonly Dictionary<MeasurementItem, string> CsvKeys = new Dictionary<MeasurementItem, string>()
        {
            { MeasurementItem.ResourceId, "resource_id"},
            { MeasurementItem.Height, "身長"},
            { MeasurementItem.Inseam, "股下高"},
            { MeasurementItem.Neck, "頚囲"},
            { MeasurementItem.Chest, "胸囲"},
            { MeasurementItem.MinWaist, "ウエスト囲(水平)"},
            { MeasurementItem.MaxWaist, "ウエスト囲(最大)"},
            { MeasurementItem.Hip, "ヒップ"},
            { MeasurementItem.LeftThigh, "太もも(左)"},
            { MeasurementItem.RightThigh, "太もも(右)"},
            { MeasurementItem.LeftShoulder, "肩幅(左)"},
            { MeasurementItem.RightShoulder, "肩幅(右)"},
            { MeasurementItem.LeftSleeve, "袖丈(左)"},
            { MeasurementItem.RightSleeve, "袖丈(右)"},
            { MeasurementItem.LeftArm, "上腕最大囲(左)"},
            { MeasurementItem.RightArm, "上腕最大囲(右)"},
            { MeasurementItem.LeftCalf, "ふくらはぎ(左)"},
            { MeasurementItem.RightCalf, "ふくらはぎ(右)"},
            { MeasurementItem.Type, "measurement_type"},
        };
    }
}
