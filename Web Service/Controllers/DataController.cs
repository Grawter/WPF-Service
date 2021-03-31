using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_Service.Models;
using Web_Service.Helpers;

namespace Web_Service.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {
        private readonly ParkingdbMainContext db;

        public DataController(ParkingdbMainContext context)
        {
            db = context;
        }

        [HttpGet("api/[controller]/Show")]
        public async Task<IActionResult> Show()
        {
            return Ok(DBPrint<List<ParkingInfo>>.Show(await db.Parks.ToListAsync()));
        }

        [HttpGet("api/[controller]/Create")]
        public async Task<IActionResult> Create() 
        {
            try
            {
                db.Parks.RemoveRange(db.Parks);
                await db.SaveChangesAsync();

                string data = Req_h.Resp("https://apidata.mos.ru/v1/datasets/621/rows/?api_key=1793510af47c955bb257b714d4524388");

                var Deser_Obj = JDeserializer<List<Parking>>.Deser(data);

                foreach (var item in Deser_Obj)
                {
                    await db.AddAsync(item.Cells);
                }
                
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            
            return Ok("Данные записаны");
        }

        [HttpGet("api/[controller]/Update")]
        public async Task<IActionResult> Update()
        {
            int current_count = await db.Parks.CountAsync(), count = 0;

            try
            {
                count = int.Parse(Req_h.Resp("https://apidata.mos.ru/v1/datasets/621/count/?api_key=1793510af47c955bb257b714d4524388"));

                if (current_count < count)
                {
                    string data = Req_h.Resp("https://apidata.mos.ru/v1/datasets/621/rows/?api_key=1793510af47c955bb257b714d4524388&$orderby=global_id%20desc&$top=" + (count - current_count));

                    var Deser_Obj = JDeserializer<List<Parking>>.Deser(data);

                    foreach (var item in Deser_Obj)
                    {
                        await db.AddAsync(item.Cells);
                    }

                    await db.SaveChangesAsync();

                    return Ok("Данные обновлены");
                }
                
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
           
            return Ok("Данные не требуют обновления");
        }

        [HttpGet("api/[controller]/Export")]
        public async Task<IActionResult> Export()
        {
            try
            {
                using (StreamWriter sw = new StreamWriter("DataFile.json", false, System.Text.Encoding.Default))
                {
                    sw.WriteLine(JWriter<List<ParkingInfo>>.Write(await db.Parks.ToListAsync()));
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return Ok("Данные экспортированны в формат .json");
        }

    }
}