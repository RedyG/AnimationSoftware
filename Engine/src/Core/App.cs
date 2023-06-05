using Engine.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Drawing;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Engine.Core
{
    public static class App
    {
        public static Project Project { get; set; }
        

        public static void SaveProject()
        {
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                Converters =
                {
                    new PointFConverter()
                }
            };
            string json = JsonConvert.SerializeObject(Project, Formatting.Indented, settings);
            Console.WriteLine(json);
            Project = JsonConvert.DeserializeObject<Project>(json, settings);
        }
    }

    public class ParameterConverter : JsonConverter<Parameter>
    {
        public override Parameter? ReadJson(JsonReader reader, Type objectType, Parameter? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return serializer.Deserialize<Parameter>(reader);
        }

        public override void WriteJson(JsonWriter writer, Parameter? value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }

    public class SizeFConverter : JsonConverter<SizeF>
    {
        public override SizeF ReadJson(JsonReader reader, Type objectType, SizeF existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var jo = JObject.Load(reader);
            return new SizeF((float)jo["Width"], (float)jo["Height"]);
        }

        public override void WriteJson(JsonWriter writer, SizeF value, JsonSerializer serializer)
        {
            JObject jo = new JObject();
            jo.Add("Width", value.Width);
            jo.Add("Height", value.Height);
            jo.WriteTo(writer);
        }
    }

    /*public class PointFConverter : JsonConverter<PointF>
    {


        public override PointF ReadJson(JsonReader reader, Type objectType, PointF existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var jo = JObject.Load(reader);
            return new PointF((float)jo["X"], (float)jo["Y"]);
        }

        public override void WriteJson(JsonWriter writer, PointF value, JsonSerializer serializer)
        {
            JObject jo = new JObject(); 
            
            jo.Add("X", value.X);
            jo.Add("Y", value.Y);
            jo.WriteTo(writer);
        }

    }*/

    public class ConverterContractResolver : DefaultContractResolver
    {
        public new static readonly ConverterContractResolver Instance = new ConverterContractResolver();

        protected override JsonContract CreateContract(Type objectType)
        {
            JsonContract contract = base.CreateContract(objectType);

            // this will only be called once and then cached
            if (objectType == typeof(PointF))
            {
                contract.Converter = new PointFConverter();
            }

            return contract;
        }
    }

    public class PointFConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(PointF) == objectType;
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            var jo = JObject.Load(reader);
            
            
                return new PointF((float)jo["X"], (float)jo["Y"]);
            
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value is PointF point)
            {
                JObject jo = new JObject();
                jo.Add("X", point.X);
                jo.Add("Y", point.Y);
                jo.WriteTo(writer);
            }
            else
                serializer.Serialize(writer, value);
        }
    }
}
