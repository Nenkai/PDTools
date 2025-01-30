# PDTools.Structures

Mainly PS3 structures.

## MCarParameter

Represents a car structure and all its parts/settings. Mainly used when serializing to save.

Should support GT5/GT6 though I wouldn't be surprised if it could break somehow. Tried to follow versioning from the game code as much as possible.

## MGameParameter

MGameParameter aka event files in PS3 GTs have a XML and binary version. The binary version actually gets really close to the one that stays in memory that the game uses. It's also version checked.

[GTEventGenerator](https://github.com/Nenkai/GTEventGenerator) attempted to create them after painfully reversing the bit stream (which I got quite close!), reversing default values aswell, but it *may* still cause crashes in some cases so some more work is needed.

010 Editor Template for the bitstream [here](https://github.com/Nenkai/GT-File-Specifications-Documentation/blob/master/Formats/PS3/Structures/GameParameter.bt).
