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
            await ClientDisonnected(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task ServerJoin(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task ClientJoin(string groupName, string cameraId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await ClientConnected(Context.ConnectionId, cameraId);
        }

        public async Task ClientDisonnected(string connectionId)
        {
            await Clients.Group(SignalingGroup.Server).SendAsync(SignalingMethod.ClientDisonnected, connectionId);
        }

        public async Task ClientConnected(string connectionId, string cameraId)
        {
            await Clients.Group(SignalingGroup.Server).SendAsync(SignalingMethod.ClientConnected, connectionId, cameraId);
        }

        public async Task OfferSDP(string connectionId, RtcSessionDescription description)
        {
            await Clients.Client(connectionId).SendAsync(SignalingMethod.OfferSDP, description);
        }

        public async Task AnswerSDP(RtcSessionDescription desc)
        {
            await Clients.Group(SignalingGroup.Server).SendAsync(SignalingMethod.AnswerSDP, desc);
        }

        public async Task OfferICE(string connectionId, RtcIceCandidate candidate)
        {
            await Clients.Client(connectionId).SendAsync(SignalingMethod.OfferICE, candidate);
        }

        public async Task AnswerICE(RtcIceCandidate candidate)
        {
            await Clients.Group(SignalingGroup.Server).SendAsync(SignalingMethod.AnswerICE, candidate);
        }
    }
}