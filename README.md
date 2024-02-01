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