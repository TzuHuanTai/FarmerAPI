using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace FarmerAPI.Hubs
{
    public class WeatherHub : Hub
	{
		public async Task TempRhSensorData(int StationID, decimal Temperature, decimal Humidity)
		{
			await Clients.All.SendAsync("TempRhSensorReceived", StationID, Temperature, Humidity);
		}
	}
}
