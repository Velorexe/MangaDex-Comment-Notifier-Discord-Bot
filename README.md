# MangaDex Forum Comment Notifier Discord Bot

<p align="center">
    A .NET Core 7.0 Console Application to update you on new comments on your MangaDex Forum!<br>
</p>

<p align="center">
    <img src="https://i.imgur.com/ZeKiHvI.png" style="width:40%;">
</p>

[![Publish](https://github.com/Velorexe/mangadex-comment-notifier-discord-bot/actions/workflows/publish.yml/badge.svg)](https://github.com/Velorexe/mangadex-comment-notifier-discord-bot/actions/workflows/publish.yml)

## Summary

<b>mangadex-forum-comment-notifier-discord-bot</b> is an in .NET 7.0 written Console Application that uses <b>DiscatSharp</b> and <b>HtmlAgilityPack</b> to scrape added MangaDex Forums to check if any of them contain new comments, and when that's the case, will send you an update through a specified Discord channel!

Why did I make this? Though MangaDex offers a beautiful API with lots of features, these kinds of API features are not available for the Forums. I love to hear from the community of MangaDex so I wanted to make an application where it'd be easy to keep track of when new comments are placed on the Forums.

<p align="left">
    <img src="https://i.imgur.com/IdoHaGF.png" style="width:40%;">
</p>

## Quick Start

Build the application using `dotnet build MangaDexForumNotifier` inside the cloned repository or use Visual Studio (Windows & Mac only) to compile a Debug or Release version.

## Settings

To get started, you can run the application once and a `settings.json` file will be created. In here there are a few options to get the Discord bot up and running. The default `settings.json` will contain the fields and values below:

```json
{
  "ForumUris": [],
  "ChannelId": 0,
  "ScanTimeoutInMinutes": 30,
  "InbetweenTimeoutInSeconds": 5,
  "DiscordBotToken": ""
}

```

If you quickly want to get started, you fill in the `DiscordBotToken` and the Discord Bot will start! Below is an explanation about the individual fields:

|Field|Description|
| --- | --- |
| `ForumUris` | A collection of all the MangaDex Forum URI's being tracked. These will be written automatically once a new URI has been added. |
| `ChannelId` | The Discord Channel ID to send the notifications to. Can be set using the `[here` command |
| `ScanTimeoutInMinutes` | The amount of minutes in between scanning all the MangaDex Forum URI's<br> :warning: Keep this at a reasonable time! Settings this at a lower value will increase the load on MangaDex's servers :warning:|
| `InbetweenTimeoutInSeconds` | The timeout between scanning individual Forum URI's. If you're encountering 404 errors, setting this higher could solve it. |
| `DiscordBotToken` | The token of your Discord Bot. You need to create this yourself at the [Discord Developer Portal](https://discord.com/developers/applications). |

## Commands

The commands are quite simple:

#### Add

Adds new MangaDex Forum URI to the bot to track.

```js
[add https://forums.mangadex.org/threads/manga-title-and-id/12345
```

#### Remove

Removes a MangaDex Forum URI if it's known to the bot.

```js
[remove https://forums.mangadex.org/threads/manga-title-and-id/12345
```

#### Here

Sets the `ChannelId` for your bot to send messages in. All notifications about comments will be send in that specific channel.

```js
[here
```

#### Refresh

Forces a scan of all the MangaDex Forum URI's

```js
[refresh
```

## Future

In the future I'd like to add support for some cool features!

* Adding all MangaDex forums from a certain scanlation group
* Automatically navigating to the next page if the comments exceed a single page
