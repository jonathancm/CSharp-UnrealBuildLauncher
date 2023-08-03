# CSharp-UnrealBuildLauncher
Launcher app for Unreal Builds

# Feature Backlog:
1. Implement "add entry" or "remove entry" actions to modify the loaded configs.
2. Have the app refresh its content automatically when a change is detected in the json file.
3. Implement a tooltip displaying the additional parameters when hovering a config entry (or by expanding the config entry).
4. Implement tags for filtering build configs.
5. Implement system to load a shared config and a personal config at the same time. The shared config could be common to the team, while the personal config is per user.
6. Implement system to display interactive checkboxes to enable/disable additional console arguments (these paremeters could be loaded from a json file).
7. Be able to bunch launch configs together into a "Recipe", so that you can launch a sequence like:
	- Build Platform X script
	- Launch Cook-on-the-fly
	- Wait X seconds
	- Launch game instance 1
	- Launch game instance 2