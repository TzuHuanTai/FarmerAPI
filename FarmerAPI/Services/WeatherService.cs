using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FarmerAPI.Models.MongoDB;
using MongoDB.Bson;

namespace FarmerAPI.Services
{
    public class WeatherService
    {
		private readonly IMongoCollection<Climate> _weather;

		public WeatherService(IConfiguration config)
		{
			var client = new MongoClient(config.GetConnectionString("WeatherDb"));
			var database = client.GetDatabase("Weather");
			_weather = database.GetCollection<Climate>("Climate");
		}

		public List<Climate> Get()
		{
			return _weather.Find(book => true).ToList();
		}

		public Climate Get(string id)
		{
			var docId = new ObjectId(id);

			return _weather.Find(book => book.Id == docId).FirstOrDefault();
		}

		public async Task<Climate> Create(Climate book)
		{
			await _weather.InsertOneAsync(book);
			return book;
		}

		public void Update(string id, Climate bookIn)
		{
			var docId = new ObjectId(id);

			_weather.ReplaceOne(book => book.Id == docId, bookIn);
		}

		public void Remove(Climate bookIn)
		{
			_weather.DeleteOne(book => book.Id == bookIn.Id);
		}

		public void Remove(ObjectId id)
		{
			_weather.DeleteOne(book => book.Id == id);
		}
	}

}