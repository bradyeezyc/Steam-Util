using System;
using SteamKit2;
using Newtonsoft.Json;
using System.Net.Http;
using System.Collections.Generic;
using System.Net;

namespace SteamUlti
{
    /// <summary>
    /// Steam Ulti Library.
    /// </summary>
    public class SteamUlti
    {

        public String STEAM_URL = "http://steamcommunity.com/profiles/";

        private static SteamCheck SteamCheck = new SteamCheck();

        private String[] CommonEmails { get => CommonEmails; set => CommonEmails = value; }
        private String[] FullEmails { get => FullEmails; set => FullEmails = value; }
        private String[] TempEmails { get => TempEmails; set => TempEmails = value; }
        private String[] Users { get => Users; set => Users = value; }
        private String[] Proxies { get => Proxies; set => Proxies = value; }
        private String[] Legacy_IDS { get => Legacy_IDS; set => Legacy_IDS = value; }

        private int ProxyUse = 0;
        private int pIndex = 0;


        private static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        /// <summary>
        /// If the date provided is within the last year.
        /// </summary>
        /// <returns><c>true</c>, if date within last year, <c>false</c> otherwise.</returns>
        /// <param name="dt">Datetime.</param>
        public static bool IsInLastYear(DateTime dt)
        {
            return dt.Year == DateTime.Now.Year - 1;
        }

        /// <summary>
        /// Compares the with email.
        /// </summary>
        /// <returns><c>true</c>, if with email was was found as the login for the account, <c>false</c> otherwise.</returns>
        /// <param name="user">User.</param>
        /// <param name="EmailDomain">Email domain.</param>
        public Boolean Compare_With_Email(User user, String EmailDomain)
        {
            var test = SteamCheck.GrabIDFromUsername(user.PossibleSteamUsername + EmailDomain);
            if (test != "-1" && long.Parse(test) == user.Steam64ID)
            {
                return true;

            }
            return false;
        }

        /// <summary>
        /// Gets the full information of steam user ID.
        /// </summary>
        /// <returns>The full information of steam user identifier.</returns>
        /// <param name="ID">Identifier.</param>
        public User GetFullInformationOfSteamUser_ID(long ID)
        {
            User u = new User();
            PlayerSum.Player sums;
            BanJson.Player player;

            u.SteamUsername = "-1";
            u.Steam64ID = ID;
            try
            {
                player = GetBanInfo(u.Steam64ID);
                sums = GetPlayerSum(u.Steam64ID);
                u.CommBan = player.CommunityBanned;
                u.EcoBan = !player.EconomyBan.Contains("none");
                u.VacBan = player.VacBanned;
                u.PossibleSteamUsername = GetPlayerSum(ID).Personaname;
                u.LeagacyID = IDToRealSteamID(ID);
                u.Exists = true;
                u.ProfileSet = sums.Profilestate == 1;
                u.ProfileURL = STEAM_URL + u.Steam64ID;
                u.LastOnline = UnixTimeStampToDateTime(sums.Lastlogoff);
                u.TimeCreated = UnixTimeStampToDateTime(sums.Timecreated);
            }
            catch (Exception)
            {
                if (u.PossibleSteamUsername == default(User).PossibleSteamUsername) { u.Exists = false; return u; }
            }

            return u;

        }
        /// <summary>
        /// Gets the full information of steam user legacy.
        /// </summary>
        /// <returns>The full information of steam user legacy.</returns>
        /// <param name="user">User.</param>
        public User GetFullInformationOfSteamUser_Legacy(String user)
        {
            PlayerSum.Player sums;
            BanJson.Player player;
            User u = new User();

            u.LeagacyID = user;
            u.SteamUsername = "-1";
            u.Steam64ID = Convert_To_64ID(u.LeagacyID);

            if (u.Steam64ID == -1) { u.Exists = false; return u; }

            try
            {
                player = GetBanInfo(u.Steam64ID);
                sums = GetPlayerSum(u.Steam64ID);
                u.PossibleSteamUsername = sums.Personaname;

                u.ProfileURL = STEAM_URL + u.Steam64ID;
                u.TimeCreated = UnixTimeStampToDateTime(sums.Timecreated);
                u.LastOnline = UnixTimeStampToDateTime(sums.Lastlogoff);
                u.VacBan = player.VacBanned;
                u.ProfileSet = sums.Profilestate == 1;
                u.CommBan = player.CommunityBanned;
                u.EcoBan = !player.EconomyBan.Contains("none");


                u.Exists = true;
            }
            catch (Exception)
            {
                if (u.PossibleSteamUsername == default(User).PossibleSteamUsername) { u.Exists = false; return u; }
            }

            return u;
        }
        /// <summary>
        /// Gets the full information of steam user of type user.
        /// </summary>
        /// <returns>The full information of steam user user.</returns>
        /// <param name="user">User.</param>
        public User GetFullInformationOfSteamUser_User(String user)
        {

            PlayerSum.Player sums;
            BanJson.Player player;
            User u = new User();

            u.SteamUsername = user;
            u.Steam64ID = long.Parse(SteamCheck.GrabIDFromUsername(user));
            u.LeagacyID = IDToRealSteamID(u.Steam64ID);

            if (u.Steam64ID == -1) { u.Exists = false; return u; }

            player = GetBanInfo(u.Steam64ID);
            sums = GetPlayerSum(u.Steam64ID);
            u.Exists = true;
            u.TimeCreated = UnixTimeStampToDateTime(sums.Timecreated);
            u.CommBan = player.CommunityBanned;
            u.VacBan = player.VacBanned;
            u.EcoBan = !player.EconomyBan.Contains("none");
            u.ProfileURL = STEAM_URL + u.Steam64ID;
            u.ProfileSet = sums.Profilestate == 1;
            u.PossibleSteamUsername = sums.Personaname;

            return u;
        }
        /// <summary>
        /// Gets the ban info.
        /// </summary>
        /// <returns>The ban info.</returns>
        /// <param name="ID">Identifier.</param>
        public BanJson.Player GetBanInfo(long ID)
        {
            var httpClient = new HttpClient();
            var response = httpClient.GetAsync("https://api.steampowered.com/ISteamUser/GetPlayerBans/v1/?key=12A1D1DE83F9932934EDD6DF2BA00463&steamids=" + ID).Result.Content.ReadAsStringAsync().Result;

            var jobject = JsonConvert.DeserializeObject<BanJson.Welcome>(response);
            return jobject.Players[0];
        }

        /// <summary>
        /// Gets the player sum.
        /// </summary>
        /// <returns>The player sum.</returns>
        /// <param name="ID">Identifier.</param>
        public PlayerSum.Player GetPlayerSum(long ID)
        {
            var httpClient = new HttpClient();
            var response = httpClient.GetStringAsync("http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key=12A1D1DE83F9932934EDD6DF2BA00463&steamids=" + ID).Result;

            var jobject = JsonConvert.DeserializeObject<PlayerSum.PlayerSums>(response);

            return jobject.Response.Players[0];
        }

        //https://stackoverflow.com/a/46375289
        private String IDToRealSteamID(Int64 ID)
        {
            var lowbit = (ID - 76561197960265728) & 1;
            var highbit = (ID - 76561197960265728 - lowbit) / 2;

            return $"STEAM_0:{lowbit}:{highbit}";
        }
        private Int64 RealSteamToID(string real_Steam)
        {
            var ID64 = 76561197960265728;
            var id_split = real_Steam.Split(':');
            ID64 += int.Parse(id_split[2]) * 2;
            if (id_split[1] == "1")
                ID64 += 1;
            return ID64;
        }

        /// <summary>
        /// Converts to legacy.
        /// </summary>
        /// <returns>The to legacy.</returns>
        /// <param name="IDList">IDL ist.</param>
        public IEnumerator<String> Convert_To_Legacy(Int64[] IDList)
        {
            for (int i = 0; i < IDList.Length; i++)
            {
                yield return IDToRealSteamID(IDList[i]);
            }
        }
        /// <summary>
        /// Converts to 64 identifier.
        /// </summary>
        /// <returns>The to 64 identifier.</returns>
        /// <param name="uList">User list.</param>
        public IEnumerator<Int64> Convert_To_64ID(String[] uList)
        {
            for (int i = 0; i < uList.Length; i++)
            {
                yield return RealSteamToID(uList[i]);
            }
        }

        /// <summary>
        /// Converts to legacy ID.
        /// </summary>
        /// <returns>The to legacy.</returns>
        /// <param name="ID">Identifier.</param>
        public String Convert_To_Legacy(Int64 ID) => IDToRealSteamID(ID);

        /// <summary>
        /// Converts to 64 identifier.
        /// </summary>
        /// <returns>The to 64 identifier.</returns>
        /// <param name="user">User.</param>
        public Int64 Convert_To_64ID(String user) => RealSteamToID(user);

        private String DoRequest(String u)
        {
            HttpClient client = new HttpClient();
            var values = new Dictionary<string, string>
            {
                { "accountname", u },
               { "count", "1"}
            };

            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36");
            var postData = new FormUrlEncodedContent(values);

            var response = client.PostAsync("https://store.steampowered.com/join/checkavail/", postData).Result;

            var responseString = response.Content.ReadAsStringAsync().Result;

            return responseString;

        }
        private String DoRequest(String u, String Proxy)
        {
            try
            {
                HttpClientHandler handler = new HttpClientHandler()
                {
                    Proxy = new WebProxy(Proxy, false),
                    UseProxy = true
                };

                HttpClient client = new HttpClient(handler);

                var values = new Dictionary<string, string>
                {
                   { "accountname", u },
                   { "count", "1"}
                };
                var postData = new FormUrlEncodedContent(values);

                var response = client.PostAsync("https://store.steampowered.com/join/checkavail/", postData).Result;

                return response.Content.ReadAsStringAsync().Result;

            }
            catch (WebException)
            {
                throw;
            }
        }
        /// <summary>
        /// Checks if the user registered.
        /// </summary>
        /// <returns><c>true</c>, if user registered was checked, <c>false</c> otherwise.</returns>
        /// <param name="u">U.</param>
        public Boolean Check_User_Registered(String u)
        {
            return DoRequest(u).Contains("true");
        }
        /// <summary>
        /// Checks if the user registered.
        /// </summary>
        /// <returns><c>true</c>, if user registered was checked, <c>false</c> otherwise.</returns>
        /// <param name="u">U.</param>
        /// <param name="Proxy">Proxy.</param>
        public Boolean Check_User_Registered(String u, String[] Proxy)
        {
            bool retry = true;
            while (retry)
            {
                try
                {
                    if (ProxyUse >= 30) { pIndex++; ProxyUse = 0; }
                    Boolean l = DoRequest(u, Proxy[pIndex]).Contains("true");
                    return l;
                }
                catch (Exception)
                {
                    pIndex++;
                    retry = true;
                    Console.WriteLine($"Invalid Proxy {Proxy[pIndex]} , Skipping to next");
                }
            }

            return false;

        }
    }
    /// <summary>
    /// Steam ID Grabber
    /// </summary>
    public class SteamCheck
    {
        static SteamClient steamClient;
        static CallbackManager manager;
        static SteamUser steamUser;
        static string username;
        static string ID = "";
        static bool isRunning;

        /// <summary>
        /// Get's the ID of any steam LOGIN account.
        /// </summary>
        /// <returns>The ID</returns>
        /// <param name="user">User.</param>
        public string GrabIDFromUsername(String user)
        {
            steamClient = new SteamClient();
            steamUser = steamClient.GetHandler<SteamUser>();
            manager = new CallbackManager(steamClient);
            manager.Subscribe<SteamClient.ConnectedCallback>(OnConnected);
            manager.Subscribe<SteamUser.LoggedOnCallback>(OnLoggedOn);
            manager.Subscribe<SteamClient.DisconnectedCallback>(OnDisconnected);
            username = user;
            steamClient.Connect();
            isRunning = true;
            while (isRunning)
            {
                try { manager.RunWaitCallbacks(TimeSpan.FromSeconds(1)); }
                catch (Exception) { manager.RunWaitCallbacks(TimeSpan.FromSeconds(1)); }
            }

            return ID;
        }

        private static void OnConnected(SteamClient.ConnectedCallback callback)
        {
            try
            {
                steamUser.LogOn(new SteamUser.LogOnDetails
                {
                    Username = username,
                    Password = "s?<password?>?1",
                });
            }
            catch (System.ArgumentException)
            {
                isRunning = false;
            }
        }

        private static void OnDisconnected(SteamClient.DisconnectedCallback callback)
        {
            isRunning = false;
        }

        private static void OnLoggedOn(SteamUser.LoggedOnCallback callback)
        {

            string steamid = Convert.ToString(callback.ClientSteamID.ConvertToUInt64());
            string staticurl = Convert.ToString(callback.ClientSteamID.GetStaticAccountKey());
            string accountid = Convert.ToString(callback.ClientSteamID.AccountID);

            if (accountid != "0")
            {
                ID = steamid;
            }
            else
            {
                ID = "-1";
            }
        }
    }

}