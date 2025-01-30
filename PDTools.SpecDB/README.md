# PDTools.SpecDB

Reader/Builder for Spec Databases (SpecDB), used in GT4/GT5/GTPSP.

For column names, mainly could be interpolated using GT6/GTS databases, which uses SQLite instead of this custom format. Version checking is attempted to be done by checking the folder names.

A pretty complex format when compression is involved, which is proprietary huffman. I figured quite a bit of it.

I did use a cheat where a certain amount of full rows (64 at best?) are cached as the most frequent (?) rows, then the game patches them using huffman, then creating other rows. I only cache a few rows and provide all byte patches to be made to said row. This technically makes search slower at runtime, but we still save more bytes than original (?), which is most important. GT4 could crash if writing all tables uncompressed, or even just a few.

010 Editor templates [here](https://github.com/Nenkai/GT-File-Specifications-Documentation/tree/master/Formats/Shared/SpecDB).
