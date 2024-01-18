### How to use
Download the `Assembly-CSharp.dll` file and replace it in your hollow knight install's `Managed` folder.  
The keybinds are customizable in `<saves location>/minisavestates.json` and by default it is
```json
{
    "LoadStateButton": "f1",
    "SaveStateButton": "f2"
}
```
When you save a state it gets saved to the file `<saves location>/minisavestates-saved.json`

This version also includes a screen shake modifier which can be modified at `<saves location>/screenShakeModifier.json`. The default is
```json
{
    "multiplier": 1.0
}
```

### Building

1. Copy Vanilla managed folder into a new folder named `Vanilla` in this repository (next to the .sln file).
2. run `dotnet build`.
3. The output will be in `out`.