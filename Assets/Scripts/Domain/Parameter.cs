using System;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Amatib.ObjViewer.Domain
{
    public class Parameter
    {
        public Parameter(string url)
        {
            var uri = new Uri(url);

            var host = uri.Host;

            Service = ServiceExtensions.FromHost(host);
            Brand = host.Split('.').First();

            var queries = HttpUtility.ParseQueryString(uri.Query);

            Key = queries["key"];

            Token = queries["token"];

            MeasurementNumber = queries["measurement_number"];

            Platform = PlatformExtensions.FromQueryParameter(queries["sp"]);

            IsShowPointCloud = "on".Equals(queries["pointcloud"], StringComparison.OrdinalIgnoreCase);

            Language = LanguageExtensions.FromQueryParameter(queries["language"]);

            IsShowPreview = "on".Equals(queries["preview"], StringComparison.OrdinalIgnoreCase);

            Port = queries["port"];

            ServerId = queries["server_id"];

            IsShowPanel = "on".Equals(queries["panel"], StringComparison.OrdinalIgnoreCase);

            Gender = queries["gender"];

            Place = queries["place"];

            string createAt = queries["created_at"];

            if (!string.IsNullOrWhiteSpace(createAt))
            {
                createAt = createAt.Replace(" ", "+");

                DateTimeOffset dateTimeOffset = DateTimeOffset.Parse(createAt, null, DateTimeStyles.AssumeUniversal);

                CreatedAt = dateTimeOffset.ToString("yyyy年MM月dd日(ddd) HH:mm:ss");
            }

            FitAndShapeServiceType = FitAndShapeServiceTypeExtensions.GetType(queries["type"]);

            CustomerId = queries["customer_id"];

            int.TryParse(queries["height"], out int height);

            Height = height;
        }

        public Service Service { get; }
        public string Brand { get; }
        public string ApiHost
        {
            get
            {
                if (Service.Equals(Service.AutoMeasure))
                {
                    return $"{Brand}.api.{Service.ToString().ToLower()}.jp";
                }

                if (Service.Equals(Service.FitAndShape))
                {
                    return $"api.{Service.ToString().Replace("And", "-").ToLower()}.jp";
                }

                return $"api.{Service.ToString().ToLower()}.jp";
            }
        }
        public string MeasurementNumber { get; }
        public string Key { get; }
        public string Token { get; }
        public Platform Platform { get; }
        public bool IsShowPointCloud { get; }
        public Language Language { get; }
        public bool IsShowPreview { get; }
        public string Port { get; }
        public string ServerId { get; }
        public string WebSocketUrl => $"wss://ws.{Service.ToString().ToLower()}.jp:{Port}/{ServerId}/";
        public bool IsShowPanel { get; }
        public string Gender { get; }
        public string Place { get; }
        public string CreatedAt { get; }
        public string CustomerId { get; }
        public FitAndShapeServiceType FitAndShapeServiceType { get; }
        public int Height { get; }
    }
}