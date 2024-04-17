# NativeConsole
A very simple mod for [Resonite](https://resonite.com) that adds a Console window which will show the game's log output.  
Since the mod uses a native DLL it will only work on Windows (Tested on 10, maybe works on 11 too).

![Demo PNG](/NativeConsole.PNG)

## Known Bugs
- If the console text is highlighted, the game will freeze until it is unhighlighted again. This could be fixed by moving the console code into it's own thread, but that's a lot of work to sync properly and would probably make this whole system have a noticeable performance impact, so... I guess just don't highlight the text. On Windows 10, the text can be unhighlighted by right-clicking into the window.

## How to install
The mod uses [ResoniteModLoader](https://github.com/resonite-modding-group/ResoniteModLoader) to function.

## Dependency
The mod requires [Pastel](https://github.com/silkfire/Pastel) (i.e. Pastel.dll) to be available in your `rml_libs` folder.
