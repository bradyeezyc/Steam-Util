# Steam Util

> This is a library with a collection of methods which exploit an information leak in SteamKit2 to get the 64ID of any steam account from their username. 
> It links multiple sources of public information to get a somewhat full profile of any steam user. 
> It is split into two parts, SteamCheck class and the SteamUtil class.

### Code Examples
###### Example 1 
```c#
SteamCheck sc = new SteamCheck();
var id = sc.GrabIDFromUser("username");
if(id != "-1"){
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
		Console.Writeline($"The login is {user.user.PossibleSteamUsername}@hotmail.com");
	}
}
```
###### Eample 3
```c#

Boolean check = su.Check_User_Registered("test");
if(check){
	Console.Writeline("Registered!");
}

su.Proxies = File.ReadAllLines("proxies.txt");
su.Users = File.ReadAllLines("users.txt");

for(int i = 0;i<su.Users.Length;i++){
	if(su.Check_User_Registered(su.Users[i],su.Proxies)){
		Console.Writeline("This account is registered");
	}
}

```

```c#
Compare_With_Email(User user, String EmailDomain)
```




