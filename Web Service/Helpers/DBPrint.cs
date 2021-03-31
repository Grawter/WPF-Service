using Web_Service.Models;

namespace Web_Service.Helpers
{
    static public class DBPrint<T>
    {
        static public string Show(in T list)
        {
            if (list == null || ((System.Collections.IList)list).Count == 0)
                return "Нет данных";

            string data = "";

            try
            {
                foreach (var item in (System.Collections.IList)list)
                {
                    data += "Name:" + (item as ParkingInfo).Name + 
                        "\nAdmArea:" + (item as ParkingInfo).AdmArea + 
                        "\nDistrict:" + (item as ParkingInfo).District + 
                        "\nAddress:" + (item as ParkingInfo).Address + 
                        "\nCarCapacity:" + (item as ParkingInfo).CarCapacity + 
                        "\nMode:" + (item as ParkingInfo).Mode + "\n\n";
                }
            }
            catch (System.Exception ex)
            {
                throw new System.Exception(ex.Message);
            }

            return data;
        }
        
    }
}