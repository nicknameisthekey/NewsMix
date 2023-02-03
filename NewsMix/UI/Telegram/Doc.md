# First time chat with bot

When user contacts bot for the first time there is only one button (Start bot) provided by telegram client which sends `/start` message from user. Bot answers with greeting message with description how to use it.

# Commands (from text) 
- `/start` - sends greeting message with description how to use bot. TODO: maybe change message if command used more than once
- `/{source_name}` - sends [buttons](#topic-buttons) with avaliable topics on selected source. 
- `/manage-subs` - sends [buttons](#user-subscriptions-buttons) with user subscriptions

#### Topic buttons
If user is not subscribed on topic adds subscription.

If user subscribed on topic button text contains "unsubscribe", removes subscription.

Pressing any of theese buttons sends "success" or "error" message.   
TODO: error messages?

#### User subscriptions buttons
Conitains current subscriptions, button removes sobscription and sends "success" or "error" message. 

#### Botfather commands to add 
start - Погнали
icyveins - Новости с icy-veins.com
noobclub - Новости с noob-club.ru
habr - новости с habr.com
ea  - новости с ea.com
stats - статистика по боту

