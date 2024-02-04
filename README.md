# Whats it for

Gives users an ability to create inidividual news feed via subscription to diffferent sources and topics and reading it in one place (for example via telegram bot).

# What users can do
 - Get a list of avaliable sources and topics 
 - Subscribe to them
 - Unsubscribe
 - Get notifications based on user subscriptions

# Users can use it via
- [Telegram bot](https://t.me/news_mix_bot) ([Description](/NewsMix/UI/Telegram/Doc.md))

# Supported news sources
- icy-veins.com
- noob-club.ru
- ea.com (only apex currently)
- habr.com

# Configuration

```json
"ConnectionStrings": {
        "Sqlite": "Data Source=path to .db"
    },
"TelegramBotToken":"token here"
```
Also you can change [Serilog](https://github.com/serilog/serilog) configuration by adding `Serilog` section, for example: 
```json
"Serilog": {
    "Using": [
      "Serilog.Sinks.Console",
      "Serilog.Sinks.File"
    ],
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "File",
        "Args": {
          "path": "/mount_dir/Logs/log.txt",
          "rollingInterval": "Day",
          "outputTemplate": "{Timestamp:MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
        }
      },
      {
        "Name": "Console",
        "Args": {
          "outputTemplate": "{Timestamp:MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
        }
      }
    ]
  },
```

# Docker

add `NewsMix.ConsoleRunner/appsettings.Docker.json`
```json
{
  "TelegramBotToken": "TokenGoesHere",
  "ConnectionStrings": {
    "Sqlite": "Data Source=/mount_dir/newsmix.db"
  }
}
```

Create volume: 
```
docker volume create \
  --driver local \
  --opt type=none \
  --opt device=i_forgot_to_set_the_path \
  --opt o=bind \
  newsmix
```

run docker-compose
```
docker-compose up
```