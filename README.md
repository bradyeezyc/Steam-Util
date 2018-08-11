# Steam Util

> This is a library with a collection of methods which exploit an information leak in SteamKit2 to get the 64ID of any steam account from their username. 
> It links multiple sources of public information to get a somewhat full profile of any steam user. 
> It is split into two parts, SteamCheck class and the SteamUtil class.

### Code Examples
###### Example 1 
```c#
SteamCheck sc = new SteamCheck();
var id = sc.GrabIDFromUser("username");
if(id != 0){
	Console.Writeline(id);
}else{
	Console.Writeline("Username does not exist");
}
```
###### Example 2
```c#
SteamUtil sU = new SteamUtil();
User user = sU.GetFullInformationOfSteamUser_Legacy("STEAM_0:0:123");
if(user.Exists){
	Boolean check = su.Compare_With_Email(user, "@hotmail.com")
	if(check){
		Console.Writeline($"The login is {user.PossibleSteamUsername}@hotmail.com");
	}
}
```
###### Eample 3
```c#

Boolean check = su.CheckUserRegistered("test");
if(check){
	Console.Writeline("Registered!");
}

String[] Users = {"user1","user2","user3"};
String[] Proxies = ....;
for(int i = 0;i<Users.Length;i++){
	if(su.Check_User_Registered(Users[i],Proxies)){
		Console.Writeline("This account is registered");
	}
}

```
## Documentation

#### Steam Util

##### Fields

> Compares the display name with an email to see if we can guess the account name.

```c#
Boolean CompareWithEmail(User user, String EmailDomain)
```

> Helper method, Good used in conjunction with the above method.

```c#
Boolean IsInLastYear(DateTime dt)
```

> Returns a User struct of information retaining to the user. Supports 64ID, Legacy ID and account name.
```c#
User GetFullInformationOfSteamUserID(long ID)
User GetFullInformationOfSteamUserLegacy(String user)
User GetFullInformationOfSteamUserUser(String user)
```
> Gets the Vac ban, Economy ban and Community ban status of a user.

```c#
PlayerBan GetBanInfo(long ID)
```

> Gets a full list of information of a user using Steam's API

```c#
Player GetPlayerSum(long ID)
```

> Converts ID64 to Legacy IDS (STEAM_0:0:123)

```c#
IEnumerator<String> ConvertToLegacy(Int64[] IDList)
String ConvertToLegacy(Int64 ID)
```

> Converts Legacy IDS to ID64.

```c#
IEnumerator<Int64> ConvertTo64ID(String[] uList)
Int64 ConvertTo64ID(String user)
```

> Checks if the account is registered

```c#
Boolean CheckUserRegistered(String u)
Boolean CheckUserRegistered(String u, String[] Proxy)
```

### Steam Check

> Gets the ID64 of any steam account, will return -1 if that account doesn't exist.

```c#
String GrabIDFromUsername(String user)
```


## Dependencies

> SteamKit2
> Newtonsoft.json
