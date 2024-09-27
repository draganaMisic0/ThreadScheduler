using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OPOS_Consumer.RabbitMQ
{
    public class BitmapConverter : JsonConverter<Bitmap>
    {
        public override Bitmap Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var base64String = reader.GetString();
            if (string.IsNullOrEmpty(base64String))
                return null;

            byte[] bytes = Convert.FromBase64String(base64String);
            using (var ms = new MemoryStream(bytes))
            {
                return new Bitmap(ms);
            }
        }

        public override void Write(Utf8JsonWriter writer, Bitmap value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNullValue();
                return;
            }

            using (var ms = new MemoryStream())
            {
                value.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                var base64String = Convert.ToBase64String(ms.ToArray());
                writer.WriteStringValue(base64String);
            }
        }
    }
}
