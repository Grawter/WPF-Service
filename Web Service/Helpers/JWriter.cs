using System.Text;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Web_Service.Models;

namespace Web_Service.Helpers
{
    static public class JWriter<T>
    {
        static public string Write(in T collection, string current_data = null)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);

            try
            {
                using (Newtonsoft.Json.JsonWriter writer = new JsonTextWriter(sw))
                {
                    writer.Formatting = Formatting.Indented;

                    writer.WriteStartArray();

                    foreach (var item in (System.Collections.IList)collection)
                    {
                        writer.WriteStartObject();

                        writer.WritePropertyName("Name");
                        writer.WriteValue((item as ParkingInfo).Name);

                        writer.WritePropertyName("AdmArea");
                        writer.WriteValue((item as ParkingInfo).AdmArea);

                        writer.WritePropertyName("District");
                        writer.WriteValue((item as ParkingInfo).District);

                        writer.WritePropertyName("Address");
                        writer.WriteValue((item as ParkingInfo).Address);

                        writer.WritePropertyName("CarCapacity");
                        writer.WriteValue((item as ParkingInfo).CarCapacity);

                        writer.WritePropertyName("Mode");
                        writer.WriteValue((item as ParkingInfo).Mode);

                        writer.WriteEndObject();
                    }

                    writer.WriteEnd();

                    if (!string.IsNullOrEmpty(current_data))
                    {
                        JArray current_doc = JArray.Parse(current_data);

                        JArray new_data = JArray.Parse(sb.ToString());
                        var child_new_data = new_data.Children();

                        current_doc.Add(child_new_data);

                        return current_doc.ToString();

                    }

                    return sb.ToString();
                }
            }
            catch (System.Exception ex)
            {
                throw new System.Exception(ex.Message);
            }

        }
    }
}