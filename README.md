
<!-- define variables -->
[1.1]: http://i.imgur.com/M4fJ65n.png (ATTENTION)
[1.2]: https://discordapp.com/api/guilds/208485545439920128/widget.png?style=banner2 (DISCORD)


<!-- disclaimer -->
[![Build status](https://ci.appveyor.com/api/projects/status/jkcqmoxr5wvo3vrq/branch/master?svg=true)](https://ci.appveyor.com/project/RocketBot/rocketbot/branch/master) 
[![Github All Releases](https://img.shields.io/github/downloads/TheUnnamedOrganisation/RocketBot/total.svg?maxAge=250)](https://github.com/TheUnnamedOrganisation/RocketBot/releases) 
[![GitHub license](https://img.shields.io/badge/license-AGPL-blue.svg)](https://raw.githubusercontent.com/TheUnnamedOrganisation/RocketBot/master/LICENSE.md) 

[![Stories in Ready](https://badge.waffle.io/TheUnnamedOrganisation/RocketBot.png?label=backlog&title=Backlog)](https://waffle.io/TheUnnamedOrganisation/RocketBot) 
[![Stories in Ready](https://badge.waffle.io/TheUnnamedOrganisation/RocketBot.svg?label=in%20progress&title=In%20Progress)](http://waffle.io/TheUnnamedOrganisation/RocketBot) 
[![Stories in Ready](https://badge.waffle.io/TheUnnamedOrganisation/RocketBot.png?label=ready&title=Ready)](https://waffle.io/TheUnnamedOrganisation/RocketBot) 
[![Stories in Ready](https://badge.waffle.io/TheUnnamedOrganisation/RocketBot.png?label=done&title=Done)](https://waffle.io/TheUnnamedOrganisation/RocketBot)

![alt text][1.1] <strong><em>`The contents of this repo are a proof of concept and are for educational use only`</em></strong>![alt text][1.1]<br/>

<h1>RocketBot2 is now compatible with 0.59.1 API.</h1>

<p>
RocketBot itself is free but now you will need to purchase an API key to run the bot.
<br/>
See https://talk.pogodev.org/d/51-api-hashing-service-by-pokefarmer for pricing for API keys.
</p>

[![Stories in Ready](https://discordapp.com/api/guilds/208485545439920128/widget.png?style=banner3&time-)](https://discord.gg/y6EU2qY)

## Developers and Contributors

### Requirements

To contribute to development, you will need to download and install the required software first.

- [Git](https://git-scm.com/downloads)
- [Visual Studio 2017](https://www.visualstudio.com/vs/whatsnew/) - We are using C# 7.0 code so VS 2017 is required to compile.  VS 2015 or older will not be able to compile the code.
- [.NET 4.6.2 SDK](https://www.microsoft.com/en-us/download/details.aspx?id=53321)

### Cloning Source Code

Next, you need to get the source code.  This source code repository uses git submodules. So when you clone the source code, you will need to clone recursively:

```
git clone --recursive https://github.com/TheUnnamedOrganisation/RocketBot.git
```

Or if you already cloned without the recursive option, you can update the submodules by running:

```
git clone --recursive https://github.com/TheUnnamedOrganisation/RocketBot.git
cd RocketBot
git submodule update --init --recursive
```

## A Pokémon Go bot in C#


## `Features`
 - PTC / Google Login
 - Get Map Objects and Inventory
 - Live map showing Pokéstops and farming path
 - Search for Pokéstop
 - Farm Pokéstops
 - Farm all Pokémon in the neighbourhood
 - Evolve Pokémon
 - Transfer Pokémon
 - Powerup Pokémon
 - Force unban
 - Use LuckyEgg
 - Auto-Recycle uneeded items
 - View all Pokémon CP/IV %
 - Transfer/Powerup/Evolve Pokémon
 - Output level and needed XP for levelup
 - Output Username, Level, Stardust, XP/hour, Pokémon/hour in Console Title
 - Automatic use of Razzberries
 - Automatic Update checker
 - Logs everything into Logs folder

## `Getting Started`

### `Download`
Download the bot from the [release](https://github.com/TheUnnamedOrganisation/RocketBot/releases) tab.  
If you want the latest Beta-Build, you have to download the build from the Beta-Build branch and compile them by yourself with VisualStudio 2017.

**Waning: Beta are unstable and might cause damage to your account, use at your own risk**

### `Login`
There are problems with google oauth login, so we have to use account and password to login for now.  
To ensure your account's safety, we suggest you to creat an app password just for botting. This will also allows users with 2-fact-auth enable to use the bot.  
Tutorial on how to use app password: [Google support](https://support.google.com/mail/answer/185833?hl=en)

### `Settings`
Change your settings using the settings tab on the bot. If you want more advance settings, edit the settings file under the bot's folder.

### `Wola`
 Click `▶ Start RocketBot2` and enjoy!

# `How can I contribute?`

## `For users:`
 You can contribute in many ways, here are some that you can do to help the project out!

### `Join discord channel and help answer questions`
 We have more and more users everyday, so we have a lot of questions from new users who haven't fully understand how the bot works yet. If you want to help them out, join our official discord channel :)

### `Answer questions in` [issues](https://github.com/TheUnnamedOrganisation/RocketBot/issues)
 Same as above, you can help by answering questions in the [issues](https://github.com/TheUnnamedOrganisation/RocketBot/issues) tab!

### `Report bugs`
 Report bugs you found in [issues](https://github.com/TheUnnamedOrganisation/RocketBot/issues).  
In order to help us fix the problem, please take a screenshot of the error you get and also attach your log file (under the Logs folder) as well. Add [Bug] to the title to help us quickly identify the category of the issue.

### `Suggestions/ideas`
 Tell us what you think we can do better in [issues](https://github.com/TheUnnamedOrganisation/RocketBot/issues).  
Give detailed discription to help us understand what you are looking for. Add [Suggestion] to the title to help us quickly identify the category of the issue. Your suggestion might not be accept, but hey, maybe we will accept your suggestion next time! :)

## [Credits](https://github.com/Necrobot-Private)
