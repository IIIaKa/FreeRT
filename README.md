# Free RT

A simple plugin that allows **players with permissions to open card-locked doors** in Rad Towns **without a card**, **by knocking on the door** instead (**except** for **Arctic Research Base** and **Nuclear Missile Silo**) :
- **Arctic Research Base**: Opens **by toggling a switch button**;
- **Nuclear Missile Silo**: Opens **by pressing a button**.  
**P.S.** You can also grant temporary permissions using the [**TemporaryPermissions**](https://github.com/IIIaKa/TemporaryPermissions) plugin.

## Permissions

- **`freert.all`** - Allows players to open **all** card-locked doors without a card;
- **`freert.green`** - Allows players to open only **green** card-locked doors without a card;
- **`freert.blue`** - Allows players to open only **blue** card-locked doors without a card;
- **`freert.red`** - Allows players to open only **red** card-locked doors without a card.

## Default Configuration

```json
{
  "Is it worth showing messages to players who don't have permissions?": true,
  "Is it worth enabling GameTips for messages?": true,
  "Is it worth using Notify plugins for messages instead of the vanilla UI?": true,
  "Specify the message type for notify": 1,
  "Time in seconds(1-10) after which the door will close(hinged doors only)": 5.0,
  "Version": {
    "Major": 0,
    "Minor": 1,
    "Patch": 10
  }
}
```

## Localization

### EN
```json
{
  "MsgNotAllowed": "You do not have permission to open this door without the card!"
}
```

### RU
```json
{
  "MsgNotAllowed": "У вас недостаточно прав для открытия этой двери без карточки!"
}
```