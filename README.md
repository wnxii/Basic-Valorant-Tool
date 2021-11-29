# Basic-Valorant-Tool
A basic valorant store checker that uses C# .NET Framework 4.8 &amp; WinForm

The .cs file on it's own will not be able to compile as this tool was built on top of my other programm, so there are many other dependencies.

The code itself is garbage and pretty useless on it's own, most of it could not be done without [brianbaldner's ValAPI.NET class library](https://github.com/brianbaldner/ValAPI.Net).

I built on top of his class library with a bunch of bug fixes and additional features, also to retarget .NET Core 2.0 as his current .NET Core 2.1 does not support .NET 4.8

I made this solely for my own learning purposes and lost interest in this project as time went on as I'm lacking time and would like to work on other things. So if you would like to continue this project, you can contact me personally on discord @wenxi#9300

I may or may not continue the project, but if I do I will do an entire recode and hopefully learn and utilize a MVP architecture structure with WPF. 

Features
- Display basic account info
- Display basic competitive rank info
- Shows current store offers
- Polls every second to pull data (prevents from being rate-limited)
- Scraps the player's presences in both party and in-game
- Displays player's information partyId, raw display name, rank and more
- Auto-lock agent
- Dodge agent select
- Disassociate/disconnect player (doesn't work)

Planned Features
- View profile button to pop up more information about their user (match history, current party members)
- Port a UI framework
- Fiddle with XMPP to spoof certain data

![Authentication Tab](https://github.com/mawenxi2112/Basic-Valorant-Tool/blob/main/AuthenticationTab.png)

![Account Tab](https://github.com/mawenxi2112/Basic-Valorant-Tool/blob/main/AccountTab.png)
![Store Tab](https://github.com/mawenxi2112/Basic-Valorant-Tool/blob/main/StoreTab.png)
![Live Match Tab](https://github.com/mawenxi2112/Basic-Valorant-Tool/blob/main/LiveMatchTab.png)
![Misc Tab](https://github.com/mawenxi2112/Basic-Valorant-Tool/blob/main/MiscTab.png)
