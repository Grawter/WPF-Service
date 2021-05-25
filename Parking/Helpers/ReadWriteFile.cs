using Newtonsoft.Json.Linq;
using Parking.Models;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Parking.Helpers
{
    public class ReadWriteFile<T>
    {
        static public string Read(string path, bool CheckFileExists = false)
        {
            string data;

            try
            {
                if (CheckFileExists)
                {
                    if (!File.Exists(path))
                    {
                        File.Create(path);
                    }
                }      

                using (StreamReader sr = new StreamReader(path))
                {
                    data = sr.ReadToEnd();
                }

                return data;
            }
            catch (System.Exception ex)
            {
                throw new System.Exception(ex.Message);
            }
        }

        static public void Write(in T data, string path, int mode)
        {
            try
            {
                string DataToWrite, current_data;

                switch (mode)
                {
                    case 1:
                        DataToWrite = JConvert<List<ParkingInfo>>.Ser((data as List<ParkingInfo>));
                        Wr(DataToWrite, path);
                        break;
                
                    case 2:
                        DataToWrite = JConvert<List<ParkingInfo>>.Ser((data as List<ParkingInfo>));
                        current_data = File.ReadAllText(path, Encoding.Default);

                        if (current_data != "\r\n" && !string.IsNullOrEmpty(current_data))
                        {
                            JArray current_doc = JArray.Parse(current_data);

                            JArray new_data = JArray.Parse(DataToWrite.ToString());
                            var child_new_data = new_data.Children();

                            current_doc.Add(child_new_data);

                            Wr(current_doc.ToString(), path);
                            break;                     
                        }

                        Wr(DataToWrite, path);
                        break;
                
                    case 3:
                        Wr(null, path);
                        break;

                    case 4:
                        if (data is string) 
                        {
                            Wr(data as string, path);
                        }
                        break;
                    default:
                        throw new System.Exception("Указан неизвестный режим");
                }              
            }
            catch (System.Exception ex)
            {
                throw new System.Exception(ex.Message);
            }
        }

        private static void Wr(in string data, string path)
        {
            using (StreamWriter sw = new StreamWriter(path, false, System.Text.Encoding.Default))
            {
                sw.WriteLine(data);
            }
        }

    }
}