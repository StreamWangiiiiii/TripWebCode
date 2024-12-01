using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TripWebAPI.Models
{
    /// <summary>
    /// 时间转换器 格式：yyyy-MM-dd HH:mm:ss
    /// </summary>
    public class DateConverter : JsonConverter<DateTime> // using System.Text.Json.Serialization
    { 
        public override DateTime Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            var strDate = reader.GetString();
            if (!string.IsNullOrWhiteSpace(strDate) && !strDate.Contains(" "))
            {
                strDate += " 00:00:00";
            }
            return DateTime.ParseExact(string.IsNullOrWhiteSpace(strDate) ? "1991-01-01":strDate,
            "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
        }
        public override void Write(Utf8JsonWriter writer, DateTime value,
        JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("yyyy-MM-dd HH:mm:ss",
            CultureInfo.InvariantCulture));
        }
    }
}
