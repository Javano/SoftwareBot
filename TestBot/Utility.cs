using System;
using System.Collections.Generic;
using SlackAPI;

namespace SoftwareBot
{
    public static class Utility
    {
        private static SortedDictionary<string,User> UserList = new SortedDictionary<string, User>();
        public static void UpdateUserList(SlackClient client)
        {
            TestClientAuth(client);
            client.GetUserList(
                (UserListResponse ulr) =>
                {
                    ulr.AssertOk();
                    foreach(User u in ulr.members)
                    {
                        if (u.IsSlackBot)
                        {
                            continue;
                        }
                        UserList.Add(u.id, u);
                        Console.Error.WriteLine("UID {0}, {2}, {1} is added.", u.id, u.profile.real_name, u.name);
                    }
                }
                
                );

        }

        private static void TestClientAuth(SlackClient client)
        {
            client.TestAuth(
            (AuthTestResponse atr) =>
                {
                    atr.AssertOk();
                    client.Connect();
                }
            );
        }

        public static User SearchUserByID(string SlackID) => UserList[SlackID];
        public static DateTime ConvertToLocal(DateTime utcTime) => utcTime.ToLocalTime();
    }
}
