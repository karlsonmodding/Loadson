# Loadson
![Lines of code](https://img.shields.io/tokei/lines/github/karlsonmodding/loadson)
![GitHub repo size](https://img.shields.io/github/repo-size/karlsonmodding/loadson)
![GitHub](https://img.shields.io/github/license/karlsonmodding/loadson)
![GitHub all releases](https://img.shields.io/github/downloads/karlsonmodding/loadson/total)
<br>
[<img src="https://img.shields.io/badge/-Discord-333333?logo=discord">](https://discord.gg/5ZjzQk8)
[<img src="https://img.shields.io/badge/-Speedrun.com-333333?logo=data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABMAAAARCAYAAAA/mJfHAAAB8klEQVQ4jZ2UzWsTURTFf3dINalYcKEYqKAgjQkISnETEnfdZLYaFbNw7cqACzeF0vqxEWNc+Bco4oi6Siq4EK2kgmAEP+JHsBAXUfyCgJ2plLkuJpO0cSZQDwzzznvnnjnv3ZkRgEo+6o7FhWzZNrpcAcWDdO+MxUV9ni3bMjODkf4UXY1tE8mWbYlU8lE3d3kagAXOuwCZUynfCEaT/XEsCaP71NMW3M5bZer6TQBdoOAaAPy4Bz/vNjPFIp12v/YfiMDyux7NzR2B9rWmzw0/thx4MSG7L3rcbshai96luo5L4oYh+x9PANJpq3jJ7AbaunQsPFI4tBY700tmWo5Ur7qw/OrWho0Ug827rlRPn8S0HC+ZaTlGe/E+Wp/sH5jdCHfxz+1DYfXblzim5QhAxF9/Oa+36/NvgrY62JEer07fYWvcsHxu+IOc5ZwwLccAZj8+08G6dWgtPgGRWdNaMQ6X7eP+vASq8V7k3LkUbEn1NbGkVs/Oib+tQYSa1Y6Ox37J998DGu2m3zgq+eiK1idV3xdUWxe0mo+uDNP/31NCEBm66iqs6YLqkE9tmFkRlrYn0iMPSjXgNQA7EumRIo+WSrAnqCawAUXQQ5lNOn7QYOfeqZ7ma/Ohfq678vzpH0oBtaFmBPzPunMCBJr9Bbg4tEXW9NKzAAAAAElFTkSuQmCC">](https://www.speedrun.com/karlson_itch_io)
[<img src="https://img.shields.io/badge/-Wiki home-333333?logo=data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAXElEQVQ4je2SwQoAIAhDt+j/f9lOgsgWHjvkqWybD5IRERBFkvXudEs1u8GZAYD5WCdOqFIjCTq+69mAbnDma8C0fsALATsPdfvcv6sNlQRKaNd7KnR0Y1RlBoADh7A0E8jZLOQAAAAASUVORK5CYII=">](https://github.com/karlsonmodding/Loadson/wiki)

Loadson is an external mod loader made for Karlson (itch.io version)

# Building Solution (and running off-grid)
- Run the `DevKit.exe` found in the root of the solution.
- Open the `Loadson.sln` solution in Visual Studio 2022 (earlier version should also work)
- Build Solution and the output is found in the `build` folder in solution root
- To run your assemblies, you will need to change [this line](https://github.com/karlsonmodding/Loadson/blob/main/Launcher/App.xaml.cs#L141) from `true` to `false` to bypass the bootstrapper.
- Copy the files you modified from the `build` folder to the `%appdata%/Loadson` folder
- Run the bypassed launcher from `%appdata%/Loadson/Launcher/Launcher.exe`

# Used dependencies
- MInject: https://github.com/devilExE3/MInject
- Harmony: https://github.com/pardeike/Harmony
