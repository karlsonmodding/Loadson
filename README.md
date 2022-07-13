# Milk
Milk is a mod loader made for Karlson (itch.io version)

# Building Solution
Compiling the solution requieres the assembly files of the game, and a custom branch of MInject (found [here](https://github.com/devilExE3/MInject). Create a folder named `lib` in the root of this project and copy over there the following files from your copy of the game:
- Karlson_Data/Managed/Assembly-CSharp.dll
- Karlson_Data/Managed/Unity.TextMeshPro.dll
- Karlson_Data/Managed/UnityEngine.dll
- Karlson_Data/Managed/UnityEngine.\*.dll

You can also do this by running the [devkit bootstrapper](github.com/devilExE3/Milk)
