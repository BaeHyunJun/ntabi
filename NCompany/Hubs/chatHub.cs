using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using NCompany.Models;
using System.Threading.Tasks;

namespace NCompany.Hubs
{
    [HubName("chat")]
    public class chatHub : Hub
    {
        DataBase DB = new DataBase();
        NtabiDB nDB = new NtabiDB();

        string corp_code = "NTB";

        public void JoinRoom(string roomName)
        {
            Groups.Add(Context.ConnectionId, roomName);
        }

        public void LeaveRoom(string roomName)
        {
            Groups.Remove(Context.ConnectionId, roomName);
        }

        public void Send(int roomNo, string user, string message, string nickname)
        {
            setChatDB(roomNo, user, message, nickname);
        }

        public void Send(int roomNo, string user, string message, string nickname, string password)
        {
            setChatDB(roomNo, user, message, nickname, password);
        }

        public void setChatDB(int roomNo, string user, string message, string nickname, string password = "")
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd"),
                   time = DateTime.Now.ToString("HH:mm:ss"),
                   outputDate = DateTime.Now.ToString("MM월 dd일 HH:mm"); ;

            string[] userKey = getUserKey(user);

            int idx = 0;

            int ChatNum = getChatNum(userKey[1], Convert.ToInt32(userKey[2]));

            if (roomNo > 0)
            {
                ChatNum = roomNo;
            }

            string titMessage = message.Length > 10 ? message.Substring(0, 10) : message;

            if (ChatNum < 1 && userKey[1] != "ADMN")
            {
                var cntRoomData = DB.CHAT_Room.GroupBy(g => new { g.CHAT_Room_ID }).Select(s => new { cnt = s.Max(max => max.CHAT_Room_ID) });

                try
                {
                    idx = cntRoomData.Single().cnt + 1;
                }
                catch
                {
                    idx = 1;
                }

                ChatNum = idx;

                CHAT_Room RoomData = new CHAT_Room();

                RoomData.CHAT_Room_ID   = ChatNum;
                RoomData.CHAT_Title     = titMessage;
                RoomData.CHAT_Name      = nickname;
                RoomData.CHAT_Password  = password;
                RoomData.CHAT_isNew     = "Y";

                DB.CHAT_Room.Add(RoomData);

                CHAT_User_Room userRoom = new CHAT_User_Room();

                userRoom.CHAT_Room_ID = ChatNum;
                userRoom.CHAT_User_YY = userKey[1];
                userRoom.CHAT_User_Seq = Convert.ToInt32(userKey[2]);

                DB.CHAT_User_Room.Add(userRoom);

                CHAT_User_Room userRoom2 = new CHAT_User_Room();

                userRoom2.CHAT_Room_ID = ChatNum;
                userRoom2.CHAT_User_YY = "ADMN";
                userRoom2.CHAT_User_Seq = 0;

                DB.CHAT_User_Room.Add(userRoom2);
            }
            else
            {
                var roomData = DB.CHAT_Room.Where(w => w.CHAT_Room_ID.ToString().Equals(ChatNum.ToString()));

                roomData.Single().CHAT_Title = titMessage;
                roomData.Single().CHAT_isNew = "Y";
            }

            try
            {
                var cntmessages = DB.CHAT_Message.Where(w => w.CHAT_Room_ID.ToString().Equals(ChatNum.ToString()))
                                                    .GroupBy(g => new { g.CHAT_Room_ID })
                                                    .Select(s => new { cnt = s.Max(max => max.CHAT_Message_ID) });

                idx = cntmessages.Single().cnt + 1;
            }
            catch
            {
                idx = 1;
            }

            CHAT_Message messageData = new CHAT_Message();

            messageData.CHAT_Room_ID = ChatNum;
            messageData.CHAT_Message_ID = idx;
            messageData.CHAT_Message1 = message;
            messageData.CHAT_Message_Date = date;
            messageData.CHAT_Message_Time = time;
            messageData.CHAT_User_YY = userKey[1];
            messageData.CHAT_User_Seq = Convert.ToInt32(userKey[2]);
            messageData.CHAT_isNew = "Y";

            DB.CHAT_Message.Add(messageData);

            DB.SaveChanges();

            message = message.Replace("\n", "<br />");

            //Clients.All.addNewmessageToPage(name, message, outputDate, user);

            Clients.Group(ChatNum.ToString()).addNewmessageToPage(ChatNum, userKey[0], message, outputDate, user);
            Clients.All.addNewmessage(ChatNum, userKey[0], titMessage);
        }

        public string[] getUserKey(string user)
        {
            user = user.Replace("ASPN15TRGT", "");

            string User_YY = "",
                   name = "";

            int User_Seq = 0;

            if (user == "GUEST")
            {
                User_YY = "GUST";
                User_Seq = 0;

                name = "손님";
            }
            else
            {
                try
                {
                    User_YY = user.Substring(0, 4).ToString();
                    User_Seq = Convert.ToInt32(user.Substring(4).ToString());

                    user = "ASPN15TRGT" + user;

                    name = getUserName(User_YY, User_Seq);
                }
                catch
                {
                    User_YY = "ADMN";
                    User_Seq = 0;

                    name = "엔타비";
                }
            }

            string[] userKey = { name, User_YY, User_Seq.ToString() };

            return userKey;
        }

        public string getUserName(string User_YY, int User_Seq)
        {
            string name = "";

            var userData = nDB.CU001.Where(w => w.CU_YY.Equals(User_YY))
                                    .Where(w => w.CU_SEQ == User_Seq);

            if (userData.Any())
            {
                name = userData.Single().CU_NM_KOR.ToString();
            }

            return name;
        }

        public int getChatNum(string User_YY, int User_Seq)
        {
            var chkChat = DB.CHAT_User_Room.Where(w => w.CHAT_User_YY.ToString().Equals(User_YY))
                                           .Where(w => w.CHAT_User_Seq == User_Seq);

            if (chkChat.Any())
            {
                return chkChat.FirstOrDefault().CHAT_Room_ID;
            }

            return 0;
        }

        public bool isChat(string User_YY, int User_Seq)
        {
            var chkChat = DB.CHAT_User_Room.Where(w => w.CHAT_User_YY.ToString().Equals(User_YY))
                                           .Where(w => w.CHAT_User_Seq == User_Seq);

            if (chkChat.Any())
            {
                return false;
            }

            return true;
        }

        //private readonly Messenger _messenger;

        //public chatHub() : this(Messenger.Instance) { }

        //public chatHub(Messenger messenger)
        //{
        //    _messenger = messenger;
        //}

        //public void AddToGroup(string group)
        //{
        //    this.Groups.Add(Context.ConnectionId, group);
        //}

        //public IEnumerable<Message> GetAllMessages()
        //{
        //    return _messenger.GetAllMaessages();
        //}

        //public void BroadCastMessage(Object message, string group)
        //{
        //    _messenger.BroadCastMessage(message, group);
        //}
    }
}