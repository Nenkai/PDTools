# PDTools
A collection of utilities for working around certain Gran Turismo games.

## Tools included
* `PDTools.Algorithms` - Just documented algorithms.
* `PDTools.Compression` - For handling all compression needs - PS2ZIP (Inflate), PDIZIP (Inflate Fragments for GTS), ZSTD (GT7)
* `PDTools.Crypto` - Shared encryption utilities and implementations (GT4 <-> GT7)
* `PDTools.Files` - File formats (mostly GT5/6)
* `PDTools.GT4ElfBuilderTool` - Builds elf executables based on compressed/encrypted CORE.GT4 files for reverse-engineering (GT4)
* `PDTools.GrimPFS` - Online Patching (GT5P/GT6)
* `PDTools.LiveTimingApi` - Library Interface for the LiveTimingAPI (GT7, requires live event permissions)
* `PDTools.LiveTimingApiTestTool` - Test tool for LiveTimingAPI (GT7)
* `PDTools.RText` - Localization File handling
* `PDTools.SaveFile` - Library for dealing with encrypted save files (GT4)
* `PDTools.SimulatorInterface` - Simulation Interface API implementation (GT6/Sport/7)
* `PDTools.SimulatorInterfaceTestTool` - Tool for testing the simulator interface
* `PDTools.SpecDB` - GT4/5 SpecDB utils
* `PDTools.Structures` - Internal structures (GT5/6)
* `PDTools.Utils` - Various utilities
* `STStruct` - Handling serialized adhoc objects (GT5/6/Sport?)

# Credits
* xFileFIN & Others - Crypto, Tooling, General help
* ddm999 - Course files & other bits
* flatz - Volume Tools & Invaluable Research
* GTPlanet for some SimulatorInterface figuring
