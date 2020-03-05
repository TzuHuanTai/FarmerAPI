using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using FarmerAPI.ViewModels;

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
