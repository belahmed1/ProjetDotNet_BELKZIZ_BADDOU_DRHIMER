using Gauniv.WebServer.Data;
using Gauniv.WebServer.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

public class OnlineStatus
{
    public User User { get; set; }
    public int Count { get; set; }
}

namespace Gauniv.WebServer.Websocket
{
    public class OnlineHub : Hub
    {
        // Initialize the dictionary properly.
        public static Dictionary<string, OnlineStatus> ConnectedUsers = new Dictionary<string, OnlineStatus>();

        private readonly UserManager<User> _userManager;
        private readonly RedisService _redisService;

        public OnlineHub(UserManager<User> userManager, RedisService redisService)
        {
            _userManager = userManager;
            _redisService = redisService;
        }

        public async override Task OnConnectedAsync()
        {
            // You could get the user info here if needed.
            var userId = Context.UserIdentifier; // Or however you want to identify the user.
            // For example, add the user to the ConnectedUsers dictionary:
            if (!ConnectedUsers.ContainsKey(Context.ConnectionId))
            {
                // Here you might load the user from the database
                // For now, we'll just initialize with a new OnlineStatus with Count 1.
                ConnectedUsers.Add(Context.ConnectionId, new OnlineStatus { Count = 1 });
            }
            await base.OnConnectedAsync();
        }

        public async override Task OnDisconnectedAsync(Exception? exception)
        {
            // Remove the user from the dictionary when disconnected.
            if (ConnectedUsers.ContainsKey(Context.ConnectionId))
            {
                ConnectedUsers.Remove(Context.ConnectionId);
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}
