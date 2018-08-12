using System;
using SteamKit2;
using Newtonsoft.Json;
using System.Net.Http;
using System.Collections.Generic;
using System.Net;
using PlayerSum;
using BanJson;

namespace SteamUtil
{
    /// <summary>
    /// Steam ID Grabber
    /// </summary>
    public class SteamIDGrabber
    {
        
        static SteamClient steamClient;
        static CallbackManager manager;
        static SteamUser steamUser;
        static string username;
        static ulong ID;
        static bool isRunning;


        /// <summary>
        /// Get's the ID of any steam LOGIN account.
        /// </summary>
        /// <returns>The ID</returns>
        /// <param name="user">User.</param>
        public ulong GrabID(String user)
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
                    Password = "mhiut76ytrdtg66",
                });
            }
            catch (Exception)
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
            
            ulong steamid = callback.ClientSteamID.ConvertToUInt64();
            ulong staticurl = callback.ClientSteamID.GetStaticAccountKey();
            uint accountid = callback.ClientSteamID.AccountID;
        
            if(accountid!=0){
                ID = steamid;
            }else{
                ID = 0;
            }

        }
    }

    /// <summary>
    /// Steam Util Library.
    /// </summary>
    public class SteamUtilities
    {
        private SteamIDGrabber _INTERNAL_GRAB = new SteamIDGrabber();
        public const ulong STEAM_NOT_FOUND = 0;
        private int ProxyUse = 0;
        private int pIndex = 0;
        public const String STEAM_URL = "http://steamcommunity.com/profiles/";
        private String API_KEY;
  
        public String[] Proxies { get => Proxies; set => Proxies = value; }

        public SteamUtilities(String APIKEY){
            API_KEY = APIKEY;
        }

        private DateTime UnixTimeStampToDateTime(double unixTimeStamp)
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
        public bool IsInLastYear(DateTime dt)
        {
            return dt.Year == DateTime.Now.Year - 1;
        }

        /// <summary>
        /// Compares the with email.
        /// </summary>
        /// <returns><c>true</c>, if with email was was found as the login for the account, <c>false</c> otherwise.</returns>
        /// <param name="user">User.</param>
        /// <param name="EmailDomain">Email domain.</param>
        public Boolean CompareAccountWithEmail(User user, String EmailDomain)
        {
            
            var id = _INTERNAL_GRAB.GrabID($"{user.PossibleSteamUsername}@{EmailDomain}");
            if (id != 0 && id == user.Steam64ID)
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
        public User GetFullInformationOfSteamUser_ID(ulong ID)
        {
            User user = new User();
            Player sums;
            PlayerBan player;

            //account name is unknown at this time.
            user.SteamUsername = "-1";

            user.Steam64ID = ID;
            try
            {
                player = GetBanInfo(user.Steam64ID);
                sums = GetPlayerSum(user.Steam64ID);
                user.CommBan = player.CommunityBanned;
                user.EcoBan = !player.EconomyBan.Contains("none");
                user.VacBan = player.VacBanned;
                user.PossibleSteamUsername = sums.Personaname;
                user.LeagacyID = IDToRealSteamID(ID);
                user.Exists = true;
                user.ProfileSet = sums.Profilestate == 1;
                user.ProfileURL = STEAM_URL + user.Steam64ID;
                user.LastOnline = UnixTimeStampToDateTime(sums.Lastlogoff);
                user.TimeCreated = UnixTimeStampToDateTime(sums.Timecreated);
            }
            catch (Exception)
            {
                if (user.PossibleSteamUsername == default(User).PossibleSteamUsername) { user.Exists = false; return user; }
            }

            return user;

        }
        /// <summary>
        /// Gets the full information of steam user legacy.
        /// </summary>
        /// <returns>The full information of steam user legacy.</returns>
        /// <param name="legacyUser">User.</param>
        public User GetFullInformationOfSteamUser_Legacy(String legacyUser)
        {
            Player sums;
            PlayerBan player;
            User user = new User
            {
                LeagacyID = legacyUser,
                SteamUsername = "-1",
                Steam64ID = ConvertTo64ID(legacyUser)
            };

            if (user.Steam64ID == 0) { user.Exists = false; return user; }

            try
            {
                player = GetBanInfo(user.Steam64ID);
                sums = GetPlayerSum(user.Steam64ID);
                user.PossibleSteamUsername = sums.Personaname;

                user.ProfileURL = STEAM_URL + user.Steam64ID;
                user.TimeCreated = UnixTimeStampToDateTime(sums.Timecreated);
                user.LastOnline = UnixTimeStampToDateTime(sums.Lastlogoff);
                user.VacBan = player.VacBanned;
                user.ProfileSet = sums.Profilestate == 1;
                user.CommBan = player.CommunityBanned;
                user.EcoBan = !player.EconomyBan.Contains("none");

                user.Exists = true;
            }
            catch (Exception)
            {
                if (user.PossibleSteamUsername == default(User).PossibleSteamUsername) { user.Exists = false; return user; }
            }

            return user;
        }
        /// <summary>
        /// Gets the full information of steam user of type user.
        /// </summary>
        /// <returns>The full information of steam user user.</returns>
        /// <param name="steamUser">User.</param>
        public User GetFullInformationOfSteamUser_User(String steamUser)
        {
            Player sums;
            PlayerBan player;
            User user = new User();

            user.SteamUsername = steamUser;
            user.Steam64ID = _INTERNAL_GRAB.GrabID(steamUser);
            user.LeagacyID = IDToRealSteamID(user.Steam64ID);

            if (user.Steam64ID == 0) { user.Exists = false; return user; }

            player = GetBanInfo(user.Steam64ID);
            if (player == null) { user.Exists = true; return user; }

            sums = GetPlayerSum(user.Steam64ID);
            user.Exists = true;
            user.TimeCreated = UnixTimeStampToDateTime(sums.Timecreated);
            user.CommBan = player.CommunityBanned;
            user.VacBan = player.VacBanned;
            user.EcoBan = !player.EconomyBan.Contains("none");
            user.ProfileURL = STEAM_URL + user.Steam64ID;
            user.ProfileSet = sums.Profilestate == 1;
            user.PossibleSteamUsername = sums.Personaname;

            return user;
        }
        /// <summary>
        /// Gets the ban info.
        /// </summary>
        /// <returns>The ban info.</returns>
        /// <param name="ID">Identifier.</param>
        public PlayerBan GetBanInfo(ulong ID)
        {   
            try
            {
                var httpClient = new HttpClient();
                var response = httpClient.GetAsync($"https://api.steampowered.com/ISteamUser/GetPlayerBans/v1/?key={API_KEY}&steamids=" + ID).Result.Content.ReadAsStringAsync().Result;

                GetBanInfo jobject = JsonConvert.DeserializeObject<GetBanInfo>(response);

                return jobject.Players[0];
            }
            catch (Exception)
            {
                return null;
            }

        }

        /// <summary>
        /// Gets the player sum.
        /// </summary>
        /// <returns>The player sum.</returns>
        /// <param name="ID">Identifier.</param>
        public Player GetPlayerSum(ulong ID)
        {

            try
            {
                var httpClient = new HttpClient();
                var response = httpClient.GetStringAsync($"http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key={API_KEY}&steamids=" + ID).Result;

                var jobject = JsonConvert.DeserializeObject<PlayerSums>(response);

                return jobject.Response.Players[0];
            }
            catch (Exception)
            {
                return null;
            }

        }

        //https://stackoverflow.com/a/46375289
        private String IDToRealSteamID(ulong ID)
        {
            //Valve's random numbers
            var lowbit = (ID - 76561197960265728) & 1;
            var highbit = (ID - 76561197960265728 - lowbit) / 2;

            return $"STEAM_0:{lowbit}:{highbit}";
        }
        private ulong RealSteamToID(string real_Steam)
        {
            var ID64 = 76561197960265728;
            var id_split = real_Steam.Split(':');
            ID64 += int.Parse(id_split[2]) * 2;
            if (id_split[1] == "1")
                ID64 += 1;
            
            return (ulong)ID64;
        }

        /// <summary>
        /// Converts to legacy.
        /// </summary>
        /// <returns>The to legacy.</returns>
        /// <param name="IDList">IDL ist.</param>
        public IEnumerator<String> ConvertToLegacy(ulong[] IDList)
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
        public IEnumerator<ulong> ConvertTo64ID(String[] uList)
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
        public String ConvertToLegacy(ulong ID) => IDToRealSteamID(ID);

        /// <summary>
        /// Converts to 64 identifier.
        /// </summary>
        /// <returns>The to 64 identifier.</returns>
        /// <param name="user">User.</param>
        public ulong ConvertTo64ID(String user) => RealSteamToID(user);

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
        public Boolean CheckUserRegistered(String u)
        {
            return DoRequest(u).Contains("true");
        }
        /// <summary>
        /// Checks if the user registered.
        /// </summary>
        /// <returns><c>true</c>, if user registered was checked, <c>false</c> otherwise.</returns>
        /// <param name="u">U.</param>
        /// <param name="Proxy">Proxy.</param>
        public Boolean CheckUserRegistered(String u, String[] Proxy)
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
}
