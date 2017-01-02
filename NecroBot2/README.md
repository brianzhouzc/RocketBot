<!-- title -->
# NecroBot2
Modified Version of NecroBot2 and RocketBot
<!-- disclaimer -->
## The contents of this repo are a proof of concept and are for educational use only

# Images
## Home
![alt tag](https://github.com/Necrobot-Private/NecroBot/blob/8311b945c2d503b5263f21d37b6b751a22479388/NecroBot2/Img/home.png)  
## Settings Auth
![alt tag](https://github.com/Necrobot-Private/NecroBot/blob/8311b945c2d503b5263f21d37b6b751a22479388/NecroBot2/Img/settings.png)  
## Settings Device
![alt tag](https://github.com/Necrobot-Private/NecroBot/blob/8311b945c2d503b5263f21d37b6b751a22479388/NecroBot2/Img/settings1.png)  
## Settings Pokemon Catch
![alt tag](https://github.com/Necrobot-Private/NecroBot/blob/8311b945c2d503b5263f21d37b6b751a22479388/NecroBot2/Img/settings2.png)  
## Settings Pokemon Transfer
![alt tag](https://github.com/Necrobot-Private/NecroBot/blob/8311b945c2d503b5263f21d37b6b751a22479388/NecroBot2/Img/settings3.png)
## Settings Pokemon PowerUp
![alt tag](https://github.com/Necrobot-Private/NecroBot/blob/8311b945c2d503b5263f21d37b6b751a22479388/NecroBot2/Img/settings4.png)  
## Settings Pokemon Evolve
![alt tag](https://github.com/Necrobot-Private/NecroBot/blob/8311b945c2d503b5263f21d37b6b751a22479388/NecroBot2/Img/settings5.png)  
## Settings Items
![alt tag](https://github.com/Necrobot-Private/NecroBot/blob/8311b945c2d503b5263f21d37b6b751a22479388/NecroBot2/Img/settings6.png)  
## Settings Advanced Settings
![alt tag](https://github.com/Necrobot-Private/NecroBot/blob/8311b945c2d503b5263f21d37b6b751a22479388/NecroBot2/Img/settings7.png) 
## Run
![alt tag](https://github.com/Necrobot-Private/NecroBot/blob/8311b945c2d503b5263f21d37b6b751a22479388/NecroBot2/Img/run.png)  
#

## Features
* PTC / Google Login
* Get Map Objects and Inventory
* Live map showing Pokéstops and farming path
* Search for Pokéstop
* Farm Pokéstops
* Farm all Pokémon in the neighbourhood
* Evolve Pokémon
* Transfer Pokémon
* Powerup Pokémon
* Force unban
* Use LuckyEgg
* Auto-Recycle uneeded items
* View all Pokémon CP/IV %
* Transfer/Powerup/Evolve Pokémon
* Output level and needed XP for levelup
* Output Username, Level, Stardust, XP/hour, Pokémon/hour in Console Title
* Automatic use of Razzberries
* Automatic Update checker
* Logs everything into Logs folder

## Getting Started
### Login
There are problems with google oauth login, so we have to use account and password to login for now.  
To ensure your account's safety, we suggest you to creat an app password just for botting. This will also allows users with 2-fact-auth enable to use the bot.  
Tutorial on how to use app password: [Google support](https://support.google.com/mail/answer/185833?hl=en)
### Settings
Change your settings using the settings tab on the bot. If you want more advance settings, edit the settings file under the bot's folder.
### Wola
Click Start Bot and enjoy!

# How can I contribute?
## For users:
You can contribute in many ways, here are some that you can do to help the project out!
### Join discord channel and help answer questions
We have more and more users everyday, so we have a lot of questions from new users who haven't fully understand how the bot works yet. If you want to help them out, join our official discord channel :)
### Answer questions in [issues](https://github.com/Necrobot-Private/NecroBot/issues)
Same as above, you can help by answering questions in the [issues](https://github.com/Necrobot-Private/NecroBot/issues) tab!
### Report bugs
Report bugs you found in [issues](https://github.com/Necrobot-Private/NecroBot/issues).  
In order to help us fix the problem, please take a screenshot of the error you get and also attach your log file (under the Logs folder) as well. Add [Bug] to the title to help us quickly identify the category of the issue.  
### Suggestions/ideas
Tell us what you think we can do better in [issues] (https://github.com/Necrobot-Private/NecroBot/issues).  
Give detailed discription to help us understand what you are looking for. Add [Suggestion] to the title to help us quickly identify the category of the issue. Your suggestion might not be accept, but hey, maybe we will accept your suggestion next time! :)

# Settings
## AuthType
* *google* - Google login
* *ptc* - Pokémon Trainer Club

## PtcUsername
* *username* - for PTC account. No need for when using Google.

## PtcPassword
* *password* - for PTC account. No need for when using Google.

## Email
* *email@gmail.com* - for Google account. No need for when using PTC.

## Password
* *password* - for Google account. No need for when using PTC.

## GoogleRefreshToken
* *token* - for Google account. No need for wen using PTC. (Obsolete)

## DefaultLatitude
* *40.764891* - Latitude of your location you want to use the bot in. Number between -90 and +90. Doesn't matter how many numbers stand after the comma.

## DefaultLongitude
* *-73.972877* - Longitude of your location you want to use the bot in. Number between -180 and +180. Doesn't matter how many numbers stand after the comma.

## LevelOutput
* *time* - Every X amount of time it prints the current level and experience needed for the next level.
* *levelup* - Only outputs the level and needed experience for next level on levelup.

## LevelTimeInterval
* *seconds* - After X seconds it will print the current level and experience needed for levelup when using *time* mode.

## Recycler
* *false* - Recycler not active.
* *true* - Recycler active.

## RecycleItemsInterval
* *seconds* - After X seconds it recycles items from the filter in *Settings.cs*.

## RazzBerryMode
* *cp* - Use RazzBerry when Pokémon is over specific CP.
* *probability* - Use RazzBerry when Pokémon catch chance is under a specific percentage.

## RazzBerrySetting
* *cp value* - If RazzBerryMode is cp. Use RazzBerry when Pokémon is over this value
* *probability value* - If RazzBerryMode is probability. Use Razzberry when % of catching is under this value. Between 0 and 1.

## TransferCPThreshold
* *CP* - transfers all Pokémon with less CP than this value.

## TransferIVThreshold
* *IV* - transfers all Pokémon with less IV than this value. Between 0 and 1.

## TravelSpeed
* *Speed* - Travel speed in km/h

## ImageSize
* *px* - Pixel size for Pokémon Thumbnails

## CatchPokemon
* *true* - Catch Pokémon and get Items from PokéStops
* *false* - Don't catch Pokémon and get Items from PokéStops

## EvolveAllGivenPokemons
* *false* - Evolves no Pokémon.
* *true* - Evolves all Pokémon.

## Credits [Necrobot-Private](https://github.com/Necrobot-Private/NecroBot)

## Credits [RocketBot](https://github.com/TheUnnameOrganization/RocketBot)
