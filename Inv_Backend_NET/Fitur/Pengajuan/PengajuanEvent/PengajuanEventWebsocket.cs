using System.Net.WebSockets;
using System.Text;
using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Database.Models;
using Inventory_Backend_NET.Fitur._Constants;
using Inventory_Backend_NET.Fitur._Logic.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Inventory_Backend_NET.Fitur.Pengajuan.PengajuanEvent;

[Route("ws/pengajuan/event")]
public class PengajuanEventWebsocket(
    MyDbContext db,
    IJwtTokenService jwtService,
    IMemoryCache memoryCache)
    : Controller
{
    [HttpGet]
    public async Task Index()
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            using var websocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            var user = await LoadCurrentUser(websocket);

            if (user != null)
            {
                await SendPengajuanTableVersionPeriodically(
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
        var jwt = await webSocket.ReceiveTextAsync();
        if (jwt is not null)
        {
            var username = jwtService.GetUsernameFromJwt(jwt);
            return db.Users.FirstOrDefault(user => user.Username == username);
        }

        return null;
    }

    private async Task SendPengajuanTableVersionPeriodically(
        WebSocket webSocket,
        User user
    )
    {
        while (true)
        {
            int message = memoryCache.Get<int>(
                user.IsAdmin ? 
                    MyConstants.CacheKeys.PengajuanTableVersionForAdmin :
                    MyConstants.CacheKeys.PengajuanTableVersionByUser(user));
            
            if (webSocket.State == WebSocketState.Aborted || webSocket.State == WebSocketState.Closed)
                break;
            await webSocket.SendTextAsync(message: message.ToString());

            Thread.Sleep(5000);
        }
    }
}


public static class PengajuanEventWebsocketExtensions
{
    public static async Task SendTextAsync(this WebSocket webSocket, string message)
    {
        var messageByte = Encoding.UTF8.GetBytes(message);
        var arraySegment = new ArraySegment<byte>(
            array: messageByte,
            offset: 0,
            count: messageByte.Length
        );
        await webSocket.SendAsync(
            buffer: arraySegment,
            messageType: WebSocketMessageType.Text,
            endOfMessage: true,
            cancellationToken: CancellationToken.None
        );
    }

    public static async Task<string?> ReceiveTextAsync(this WebSocket webSocket)
    {
        var buffer = new byte[1024 * 50];
        var result = await webSocket.ReceiveAsync(
            buffer: new ArraySegment<byte>(buffer),
            cancellationToken: CancellationToken.None
        );
        if (result.MessageType == WebSocketMessageType.Text)
            return Encoding.UTF8.GetString(buffer, 0, result.Count);
        return null;
    }
}