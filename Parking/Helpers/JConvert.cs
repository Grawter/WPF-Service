using Newtonsoft.Json;

namespace Parking.Helpers
{
    static public class JConvert <T>
    {
        static public T Deser(in string data)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(data);
            }
            catch (System.Exception ex)
            {
                throw new System.Exception(ex.Message);
            }           
        }

        static public string Ser(T data)
        {
            try
            {
                return JsonConvert.SerializeObject(data, Formatting.Indented);
            }
            catch (System.Exception ex)
            {
                throw new System.Exception(ex.Message);
            }
        }

    }
}