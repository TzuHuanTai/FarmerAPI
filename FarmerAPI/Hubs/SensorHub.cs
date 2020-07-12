using FarmerAPI.ViewModels;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace FarmerAPI.Hubs
{
	public class SensorHub : Hub
	{
		public async Task SensorDetected(VmRealtime DetectedData)
		{
			await Clients.All.SendAsync("SensorDetected", DetectedData);
		}
	}
}