## PDTools.GTPatcher
Patcher for PS4 GTs to inject command line arguments & more (GTS/GT7) using ps4debug

Mainly loading unpacked, logging, and misc stuff. Sadly all of it is very version specific.

* `AdhocExceptionFixer.cs` fixes a crash in GT7 when the game tries to report adhoc script crashes to [vegas](https://nenkai.github.io/gt-modding-hub/concepts/online/vegas/) when offline. (GT7)
* `AdhocExceptionLogger.cs` logs adhoc exceptions. (GT7)
* `CommandLineInjector.cs` injects command line arguments (GTS/7), which are then added to adhoc's AppOpt.
* `EvalExpressionCompilationTokenTypeLogger.cs` - Prints each token lexing token numbers when using EvalExpressionString. I don't remember why I needed this other than maybe reverse the adhoc language itself using the built-in compiler. (GT7)
* `EvalExpressionStringLogger.cs` logs all adhoc expressions evaluated by GT7, as a compiler is included for some text related operations. It's not the full compiler though as it doesn't have preprocessing. Mainly intended for runtime only.
* `FileDeviceKernelAccessLogger.cs` logs direct file accesses (GTS)
* `FileDeviceMPHAccessLogger.cs` logs file access from the MPH volume system (GT7)
* `TinyWebAdhocModuleCacheDisabler.cs` disables script caching when running adhoc scripts through TinyWeb. Otherwise requires a restart. (GT7)
* `VersionBranchPatcher` & friends - changes branch/build/environment. for instance `build`'s value can be changed to `debug`.
