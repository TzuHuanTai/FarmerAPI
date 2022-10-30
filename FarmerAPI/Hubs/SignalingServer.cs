using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;
using FarmerAPI.Models.SignalingServer;

namespace FarmerAPI.Hubs
{
    public class SignalingServer : Hub
    {
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, SignalingGroup.Client);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task JoinAsServer()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, SignalingGroup.Server);
        }

        public async Task JoinAsClient()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, SignalingGroup.Client);
            await ConnectedClient(Context.ConnectionId);
        }

        public async Task ConnectedClient(string connectionId)
        {
            await Clients.Group(SignalingGroup.Server).SendAsync(SignalingMethod.ConnectedClient, connectionId);
        }

        public async Task OfferSDP(RtcSessionDescription description)
        {
            await Clients.Group(SignalingGroup.Server).SendAsync(SignalingMethod.OfferSDP, description);
        }

        public async Task AnswerSDP(string connectionId, RtcSessionDescription description)
        {
            await Clients.Client(connectionId).SendAsync(SignalingMethod.AnswerSDP, description);
        }

        public async Task OfferICE(RtcIceCandidate candidate)
        {
            await Clients.Group(SignalingGroup.Server).SendAsync(SignalingMethod.OfferICE, candidate);
        }

        public async Task AnswerICE(string connectionId, RtcIceCandidate candidate)
        {
            await Clients.Client(connectionId).SendAsync(SignalingMethod.AnswerICE, candidate);
        }
    }
}