# Free RT

A simple plugin that allows **players with permissions to open card-locked doors** in Rad Towns **without a card**, **by knocking on the door** instead (**except** for **Arctic Research Base** and **Nuclear Missile Silo**) :
- **Arctic Research Base**: Opens **by toggling a switch button**;
- **Nuclear Missile Silo**: Opens **by pressing a button**.  
**P.S.** You can also grant temporary permissions using the [**TemporaryPermissions**](https://github.com/IIIaKa/TemporaryPermissions) plugin.

## Contacts
**Telegram:** https://t.me/iiiaka  
**Discord:** @iiiaka  
**GitHub:** https://github.com/IIIaKa  
**uMod:** https://umod.org/user/IIIaKa  
**Codefling:** https://codefling.com/iiiaka  
**GitHub repository page:** https://github.com/IIIaKa/FreeRT

## Donations

**USDT TRC20:** `TLN9Tsrdmt96yFCXZTfh4NLtyzWYGkqTA3`  
**USDT TON:** `UQDma5Ovkk7M9Qve-4P2njmrgSXZQdACU0gLCGNEgkXDlngn`  
**TON:** `UQDma5Ovkk7M9Qve-4P2njmrgSXZQdACU0gLCGNEgkXDlngn`

## Permissions

- **`freert.all`** - Allows players to open **all** card-locked doors without a card;
- **`freert.green`** - Allows players to open only **green** card-locked doors without a card;
- **`freert.blue`** - Allows players to open only **blue** card-locked doors without a card;
- **`freert.red`** - Allows players to open only **red** card-locked doors without a card.

## Default Configuration

```json
{
  "Is it worth showing messages to players who don't have permissions?": true,
  "Version": {
    "Major": 0,
    "Minor": 1,
    "Patch": 7
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