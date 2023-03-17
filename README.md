# PDTools
A collection of utilities for working around certain Gran Turismo games.

## PDTools.Crypto
Library for dealing with crypto. Contains:
* MCipher class - Generic encryption class, also used for saves (GT5/6)
* MTRandom class - Used for pseudo-randomness generation
* Salsa20 class - Used in movie, database encryption in GT5/6 & more
* Chacha20 class - Used in GT7 volume & more
* PDIPFSDownloaderCrypto class - Used for GT5/6 online patch update encryption
* SimulationInterface folder - Sim Interface encryption handling for GT6, GTS, GT7

## PDTools.SimulatorInterface & PDTools.SimulatorInterfaceTestTool
Simulation Interface API implementation (GT6/Sport/7) to allow seeing "telemetry" information.

# Credits
* xFileFIN & Others - Crypto, Tooling, General help
* ddm999 - Course files & other bits
* flatz - Volume Tools & Invaluable Research
* GTPlanet for some SimulatorInterface figuring
