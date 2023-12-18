using System.Net.WebSockets;
using Inventory_Backend_NET.Constants;
using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Extension;
using Inventory_Backend_NET.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;

namespace Inventory_Backend_NET.Controllers.Pengajuan;

[Route("pengajuan/event")]
[Authorize(policy: MyConstants.Policies.AllUsers)]
public class PengajuanEventWebsocket : Controller
{
    private readonly IDistributedCache _cache;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly MyDbContext _db;
    
    public PengajuanEventWebsocket(
        IDistributedCache cache,
        IHttpContextAccessor httpContextAccessor,
        MyDbContext db
    )
    {
        _cache = cache;
        _httpContextAccessor = httpContextAccessor;
        _db = db;
    }
    
    [HttpGet]
    public async Task Index()
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            var user = _db.GetCurrentUserFrom(_httpContextAccessor)!;
            using var websocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            
            ReceiveAcknowledgment(
                webSocket: websocket,
                user: user
            );
            SendNotification(
                webSocket: websocket,
                user: user
            );
        }
        else
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }

    private async Task ReceiveAcknowledgment(
        WebSocket webSocket,
        User user
    )
    {
        var buffer = new byte[1024 * 4];

        while (webSocket.State == WebSocketState.Open)
        {
            var result = await webSocket.ReceiveAsync(
                buffer: new ArraySegment<byte>(buffer),
                CancellationToken.None
            );
            if (result.MessageType == WebSocketMessageType.Text)
            {
                // var message = Encoding.UTF8.GetString(
                //     buffer, 0, result.Count
                // );
                
                // Remove cache yang sudah di acknowledge
                _cache.Remove(user.Username);
            }
            else if (result.MessageType == WebSocketMessageType.Close)
                await webSocket.CloseAsync(
                    closeStatus: WebSocketCloseStatus.NormalClosure, 
                    statusDescription: string.Empty, 
                    cancellationToken: CancellationToken.None
                );
        }
    }

    private async Task SendNotification(
        WebSocket webSocket,
        User user
    )
    {
        while (true)
        {
            var message = _cache.Get(user.Username);
            if (message != null && !message.IsNullOrEmpty())
            {
                var arraySegment = new ArraySegment<byte>(
                    array: message,
                    offset: 0,
                    count: message.Length
                );
                if (webSocket.State == WebSocketState.Aborted || webSocket.State == WebSocketState.Closed)
                    break;
                await webSocket.SendAsync(
                    buffer: arraySegment,
                    messageType: WebSocketMessageType.Text,
                    endOfMessage: true,
                    cancellationToken: CancellationToken.None
                );
            }

            Thread.Sleep(7000);
        }
    }

}