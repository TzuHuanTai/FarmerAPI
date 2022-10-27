using FarmerAPI.Models.SQLite;
using System.Collections.Concurrent;

namespace FarmerAPI.Services
{
	public class SharedService
	{
		public ConcurrentQueue<Climate> climate_buffer;

		public SharedService()
		{
			climate_buffer = new ConcurrentQueue<Climate>();
		}
	}
}
