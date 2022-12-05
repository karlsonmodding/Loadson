# Loadson
Loadson is an external mod loader made for Karlson (itch.io version)

# Building Solution (and running off-grid)
- Run the `DevKit.exe` found in the root of the solution.
- Open the `Loadson.sln` solution in Visual Studio 2022 (earlier version should also work)
- Build Solution and the output is found in the `build` folder in solution root
- To run your assemblies, you will need to change [this line](https://github.com/karlsonmodding/Loadson/blob/main/- Launcher/App.xaml.cs#L94) from `true` to `false` to bypass the bootstrapper.
- Copy the files you modified from the `build` folder to the `%appdata%/Loadson` folder
- Run the bypassed launcher from `%appdata%/Loadson/Launcher/Launcher.exe`

# Used dependencies
- MInject: https://github.com/devilExE3/MInject
- Harmony: https://github.com/pardeike/Harmony