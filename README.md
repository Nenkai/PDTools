# PDTools
A collection of utilities for working around certain Gran Turismo games.

## PDTools.Compression 
Handles all compression needs.
* PS2ZIP - GT3 to GT7 - Inflate, starts with 0xC5EEF7FF
* PDIZIP - GTS - Inflate fragments
* ZSTD - GT7

## PDTools.Crypto
Library for dealing with crypto. Contains:
* MCipher class - Generic encryption class, also used for saves (GT5/6)
* MTRandom class - Used for pseudo-randomness generation
* Salsa20 class - Used in movie, database encryption in GT5/6 & more
* Chacha20 class - Used in GT7 volume & more
* PDIPFSDownloaderCrypto class - Used for GT5/6 online patch update encryption
* SimulationInterface folder - Sim Interface encryption handling for GT6, GTS, GT7

## PDTools.Files
File format handling. Mostly PS3 formats.

## PDTools.Files.Fonts.NVecBuilder
TTF to NVEC Font file converter for GT5/GT6

## PDTools.GT4ElfBuilderTool
Builds elf executables based on compressed/encrypted CORE.GT4 files for reverse-engineering (GT4)

## PDTools.GTPatcher
Patcher for PS4 GTs to inject command line arguments & more (GTS) using ps4debug

## PDTools.GrimPFS
Online Patching library (GT5P/GT6) to be used in conjunction with GTToolsSharp

## PDTools.LiveTimingApi & PDTools.LiveTimingApiTestTool
Library Interface & test tool for the LiveTimingAPI.
Interfacing requires live event permission on the current GT7 account (or a way to enable it on a modded console)

## PDTools.RText
Localization File handling (GT4-5-6-S-7)

## PDTools.SaveFile
Library for reading, decrypting, writing encrypted save files (GT4)

## PDTools.SimulatorInterface & PDTools.SimulatorInterfaceTestTool
Simulation Interface API implementation (GT6/Sport/7) to allow seeing "telemetry" information.

## PDTools.SpecDB
GT4/GT5 Car Specification database file handler

## PDTools.Structures
Documented internal structures (GT5/6)

## PDTools.Utils
Various non-specific utilities

## PDTools.Utils
Handling serialized adhoc object files (GT5/6/Sport)

# Credits
* xFileFIN & Others - Crypto, Tooling, General help
* ddm999 - Course files & other bits
* flatz - Volume Tools & Invaluable Research
* GTPlanet for some SimulatorInterface figuring
