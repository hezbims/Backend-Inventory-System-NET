using System.Net.WebSockets;
using System.Text;
using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Models;
using Inventory_Backend_NET.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;

namespace Inventory_Backend_NET.Controllers.Pengajuan;

[Route("ws/pengajuan/event")]
public class PengajuanEventWebsocket : Controller
{
    private readonly IDistributedCache _cache;
    private readonly MyDbContext _db;
    private readonly IJwtTokenService _jwtService;
    
    public PengajuanEventWebsocket(
        IDistributedCache cache,
        MyDbContext db,
        IJwtTokenService jwtService
    )
    {
        _cache = cache;
        _db = db;
        _jwtService = jwtService;
    }
    
    [HttpGet]
    public async Task Index()
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            using var websocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            var user = await LoadCurrentUser(websocket);

            if (user != null)
            {
                await SendText("Authenticated", websocket);
                ListenAcknowledgment(
                    webSocket: websocket,
                    user: user
                );
                SendNotification(
                    webSocket: websocket,
                    user: user
                );
            }
            else
                await websocket.CloseAsync(
                    closeStatus: WebSocketCloseStatus.NormalClosure, 
                    statusDescription: string.Empty, 
                    cancellationToken: CancellationToken.None
                );
        }
        else
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
    }

    // Autentikasi manual, user diharuskan mengirim JWT ke websocket channel
    private async Task<User?> LoadCurrentUser(WebSocket webSocket)
    {
        var buffer = new byte[1024 * 50];
        var result = await webSocket.ReceiveAsync(
            buffer: new ArraySegment<byte>(buffer),
            cancellationToken: CancellationToken.None
        );
        if (result.MessageType == WebSocketMessageType.Text)
        {
            var jwt = Encoding.UTF8.GetString(buffer, 0, result.Count);
            var username = _jwtService.GetUsernameFromJwt(jwt);
            
            return _db.Users.FirstOrDefault(user => user.Username == username);
        }

        return null;
    }

    private async Task ListenAcknowledgment(
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
            // Kalo dapat pesan ready dari user
            if (result.MessageType == WebSocketMessageType.Text &&
                Encoding.UTF8.GetString(buffer, 0, result.Count) == "Ready")
            {
                // Remove cache yang sudah di acknowledge
                _cache.Remove(user.Username);
                // nandai kalo cache udah di remove, dan user boleh ngehasilin notifikasi
                await SendText("Acknowledged", webSocket);
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
            var message = _cache.Get(user.Username) ?? new byte[]{};
            if (!message.IsNullOrEmpty())
            {
                if (webSocket.State == WebSocketState.Aborted || webSocket.State == WebSocketState.Closed)
                    break;
                // Nyuruh user untuk ngekonfirmasi bahwa user siap nerima notifikasi
                await SendText("Please Confirm", webSocket);
            }

            Thread.Sleep(5000);
        }
    }

    private async Task SendText(byte[] message , WebSocket webSocket)
    {
        var arraySegment = new ArraySegment<byte>(
            array: message,
            offset: 0,
            count: message.Length
        );
        await webSocket.SendAsync(
            buffer: arraySegment,
            messageType: WebSocketMessageType.Text,
            endOfMessage: true,
            cancellationToken: CancellationToken.None
        );
    }
    
    private async Task SendText(string message, WebSocket webSocket)
    {
        await SendText(Encoding.UTF8.GetBytes(message), webSocket);
    }

}