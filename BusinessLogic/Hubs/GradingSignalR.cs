using Common.Helpers;
using Common.Models;
using Common.Models.Chat;
using DataAccess.DatabaseContext;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using static Common.Enumeration.Enumeration;

namespace BusinessLogic.Hubs
{
    public class UserConnection
    {
        public string? ConnectionId { get; set; }
        public UserInfo? User { get; set; }
    }
    public class GradingSignalR : Hub
    {
        private IJwtHelper _jwtHelper;
        private string _accessTokenFactory;
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContext;
        private UserInfo? _user;
        public GradingSignalR(IJwtHelper jwtHelper, IHttpContextAccessor httpContext, AppDbContext context)
        {
            _jwtHelper = jwtHelper;
            _httpContext = httpContext;
            _context = context;
            _accessTokenFactory = httpContext.HttpContext.Request.Query["access_token"];
            if (!string.IsNullOrEmpty(_accessTokenFactory))
            {
                _user = _jwtHelper.ValidateToken(_accessTokenFactory);
            }
        }
        private static Dictionary<string, List<UserConnection>> examGroups = new Dictionary<string, List<UserConnection>>();
        public override async Task OnConnectedAsync()
        {
            try
            {
                if (_user != null)
                {
                    var groupId = Context.GetHttpContext().Request.Query["groupId"];
                    var screen = Context.GetHttpContext().Request.Query["screen"];
                    if (!string.IsNullOrEmpty(groupId) && !string.IsNullOrEmpty(screen))
                    {
                        await Groups.AddToGroupAsync(Context.ConnectionId, groupId);
                        var userConnection = new UserConnection() { ConnectionId = Context.ConnectionId, User = _user };
                        switch (Convert.ToString(screen).ToLower())
                        {
                            case "examdetail":
                                if (examGroups.ContainsKey(groupId))
                                {
                                    examGroups[groupId].Add(userConnection);
                                }
                                else
                                {
                                    examGroups.Add(groupId, new List<UserConnection>() { userConnection });
                                }
                                break;
                            default:
                                break;
                        }
                        await Clients.Group(groupId.ToString()).SendAsync("StudentJoinGroup", JsonConvert.SerializeObject(userConnection), screen);
                    }
                }
            }
            catch (System.Exception)
            {
                throw;
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {

            try
            {
                if (_user != null)
                {
                    var groupId = Context.GetHttpContext().Request.Query["groupId"];
                    var screen = Context.GetHttpContext().Request.Query["screen"];
                    var userConnection = new UserConnection() { ConnectionId = Context.ConnectionId, User = _user };
                    if (!string.IsNullOrEmpty(groupId))
                    {
                        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupId);
                        if (examGroups.ContainsKey(groupId))
                        {
                            examGroups[groupId].Remove(examGroups[groupId].Find(x => x.ConnectionId == Context.ConnectionId));
                        }
                    }
                    await Clients.Group(groupId.ToString()).SendAsync("StudentLeaveGroup", JsonConvert.SerializeObject(userConnection), screen);
                }
            }
            catch (System.Exception)
            {


            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task<string> GetListUserConnection(int groupId)
        {
            if (_user != null && _user.Role == (int)Role.Teacher)
            {
                if (examGroups.ContainsKey(groupId.ToString()))
                {
                    return JsonConvert.SerializeObject(examGroups[groupId.ToString()]);
                }
            }
            return JsonConvert.SerializeObject(new List<UserConnection>());

        }

        // send message to all user in group
        public async Task SendMessageToGroup(int groupId, string message)
        {
            if (_user != null && _user.Role == (int)Role.Teacher)
            {
                NoticeMessage noticeMessage = new NoticeMessage()
                {
                    GroupId = groupId,
                    Message = message,
                    SenderId = _user.AccountId,
                    SenderName = _user.Email,
                    Type = "text"
                };
                _context.NoticeMessages.Add(noticeMessage);
                _context.SaveChanges();
                await Clients.Caller.SendAsync("ReceiveAdminMessage", JsonConvert.SerializeObject(noticeMessage));
                await Clients.Group(groupId.ToString()).SendAsync("ReceiveMessage", JsonConvert.SerializeObject(noticeMessage));
            }
        }

        // get message from group
        public string GetMessageFromGroup(int groupId)
        {
            if (_user != null)
            {
                var messages = _context.NoticeMessages.Where(x => x.GroupId == groupId).ToList();
                return JsonConvert.SerializeObject(messages);
            }
            return JsonConvert.SerializeObject(new List<NoticeMessage>());
        }

        public string DeleteMessage(int messageId)
        {
            if (_user != null && _user.Role == (int)Role.Teacher)
            {
                var message = _context.NoticeMessages.Find(messageId);
                if (message != null)
                {
                    _context.NoticeMessages.Remove(message);
                    _context.SaveChanges();
                }
            }
            return "success";
        }
    }
}
