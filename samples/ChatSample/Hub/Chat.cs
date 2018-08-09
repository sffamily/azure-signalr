// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace ChatSample
{
    public class Chat : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var request = Context.GetHttpContext().Request;
            if (request != null)
            {
                var sb = new StringBuilder("Headers:\r\n");
                foreach (var header in request.Headers)
                {
                    sb.Append(header.ToString());
                    sb.Append("\r\n");
                }

                sb.Append("Query: ").Append(request.QueryString)
                    .Append("\r\n");
                await Clients.Caller.SendAsync("echo", "SYSTEM", sb.ToString());
            }

            await base.OnConnectedAsync();
        }

        public void BroadcastMessage(string name, string message)
        {
            Clients.All.SendAsync("broadcastMessage", name, message);
        }

        //[Authorize]
        public async Task Echo(string name, string message)
        {
            //Clients.Client(Context.ConnectionId).SendAsync("echo", name, message + " (echo from server)");
            const string groupName = "some-group";
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("echo", name, message + "(echo from server)");
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }
    }
}
