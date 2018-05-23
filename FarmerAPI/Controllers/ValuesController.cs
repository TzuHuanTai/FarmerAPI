﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FarmerAPI.Models;
using FarmerAPI.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Cors;
using FarmerAPI.Filters;

namespace FarmerAPI.Controllers
{    
    [Route("api/[controller]")]
    //[EnableCors("AllowAllOrigins")] //在Startup.cs做全域設定
    public class ValuesController : Controller
    {

        private readonly WeatherContext _context;
        public ValuesController(WeatherContext context)
        {
            _context = context;
        }

        // /api/values/Realtime/1
        //[AuthorizationFilter]
        [HttpGet("[action]/{StationId}")]
        public async Task<IActionResult> Realtime(int StationId)
        {
            if (StationExists(StationId))
            {
                //DB抓資料出來
                RealTime DbRealtimeData = await _context.RealTime.SingleOrDefaultAsync(x => x.Id == StationId);
                string DbStationName = await _context.StationInfo.Where(x => x.Id == StationId).Select(x => x.Name).FirstOrDefaultAsync();

                vmRealtime ReturnRealtimeData = new vmRealtime()
                {
                    DateFormatted = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"),
                    StationId = StationId,
                    StationName = DbStationName,
                    RecTemp = DbRealtimeData.Temperature,
                    RecRH = DbRealtimeData.Rh
                };
                return Ok(ReturnRealtimeData);               
            }
            else
            {
                //return new vmRealtime();
                return NotFound();
            }            
        }

        // This action at /api/values/Realtime/5 can bind form data (set individual parameters in body)
        // Content-Type: application/json, x-www-form-urlencoded is working
        [HttpPut("[action]/{StationId}")]
        public void Realtime(int StationId, decimal RecTemp = -1, decimal RecRH = -1)
        {
            if (StationExists(StationId))
            {
                //Update database realtime table through the recieved data on Raspberry pi DHT22 sensor.
                RealTime TargetStation = _context.RealTime.SingleOrDefault(x => x.Id == StationId);
                TargetStation.Temperature = RecTemp;
                TargetStation.Rh = RecRH;
                _context.Entry(TargetStation).State = EntityState.Modified;
                try
                {
                    _context.SaveChanges();
                }
                catch(DbUpdateConcurrencyException)
                {
                    throw;
                }
            }            
        }

        // This action at /api/values/Realtime/5 can bind type of JSON in body directly because of [FromBody]
        // Content-Type: application/json
        // Using Query String Parameters will show error: 415 Unsupported Media Type
        [HttpPost("[action]/{StationId}")]
        public void Realtime([FromBody]vmRealtime realtime)
        {
            if (StationExists(realtime.StationId))
            {
                //Update database realtime table through the recieved data on Raspberry pi DHT22 sensor.
                RealTime TargetStation = _context.RealTime.SingleOrDefault(x => x.Id == realtime.StationId);
                TargetStation.Temperature = realtime.RecTemp;
                TargetStation.Rh = realtime.RecRH;
                _context.Entry(TargetStation).State = EntityState.Modified;
                try
                {
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
            }
        }

        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value", "value" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        private bool StationExists(int StationId)
        {
            return _context.RealTime.Any(x => x.Id == StationId);
        }
    }
}
