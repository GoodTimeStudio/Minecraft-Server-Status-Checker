using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minecraft_Server_Status_Checker.Status.Motd
{
    public class TextComponentBase
    {
        [JsonConverter(typeof(ColorConverter))]
        public ColorCode color;

        public bool? bold;
        public bool? italic;
        public bool? underlined;
        public bool? strikethrough;
        public bool? obfuscated;

        public void Reset()
        {
            bold = false;
            italic = false;
            underlined = false;
            strikethrough = false;
            obfuscated = false;

            color = ColorCode.DefaultColor;
        }
    }

    public class ColorConverter : CustomCreationConverter<ColorCode>
    {
        public override ColorCode Create(Type objectType)
        {
            return null;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value != null && reader.ValueType == typeof(string))
            {
                return ColorCode.GetColorCodeFromColorName(reader.Value.ToString());
            }
            return base.ReadJson(reader, objectType, existingValue, serializer);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((ColorCode)value).name);
        }
    }
}
