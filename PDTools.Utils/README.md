# PDTools.Utils

Various utils.

* `AlphaNumStringComparer.cs` - Mostly used for file/volume builders, when ordering entries by mandatory bsearch. They use bsearch a lot.
* `BitStream.cs` - a VERY useful class for reading bit streams. This is a `ref struct` so always pass it around functions with `ref`. Supports little/big endian (LSB/MSB).
* `OptimizedStringTable.cs` a useful class for building string tables, when building files that have file offsets/pointers to recurring strings.
* `PDIPFSPathScrambler.cs` - The scrambler for file paths, used in GT5's PDIPFS. See [here](https://nenkai.github.io/gt-modding-hub/formats/volume/ps3_volume/?h=pdipfs#pdipfs-path-scrambling).
* `RegionUtil.cs` Reversed driver region generator from GT6. Intended to be combined with the driver name table/xml.
