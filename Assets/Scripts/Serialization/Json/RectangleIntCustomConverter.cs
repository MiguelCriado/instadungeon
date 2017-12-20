using Newtonsoft.Json;
using System;

namespace InstaDungeon.Serialization
{
	public class RectangleIntCustomConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return typeof(RectangleInt).IsAssignableFrom(objectType);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			RectangleInt rect = value as RectangleInt;

			writer.WriteStartObject();
			writer.WritePropertyName("width");
			serializer.Serialize(writer, rect.width);
			writer.WriteEndObject();

			writer.WriteStartObject();
			writer.WritePropertyName("height");
			serializer.Serialize(writer, rect.height);
			writer.WriteEndObject();
		}
	}
}
