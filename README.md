# CSharp-UnrealBuildLauncher
Launcher app for Unreal Builds

# Feature Backlog:
1. Implement "add", "edit" & "remove" actions to modify the loaded configs
2. Implement "Save" action to save modified configs back into the json file
3. Have the app refresh its content automatically when a change is detected in the json file
4. Have the app refresh its content automatically when a change was made to the config entry from the app
5. Implement a tooltip displaying the additional parameters when hovering a config entry (or by expanding the config entry)
6. Improve json deserialization error handling and user feedback
7. Improve app icon
8. Improve status indicator icons
9. Implement tags for filtering build configs
10. Implement system to load a shared config and a personal config at the same time. The shared config could be common to the team, while the personal config is per user.
11. Implement system to display interactive checkboxes to enable/disable additional console arguments (these paremeters could be loaded from a json file)
12. Be able to bunch launch configs together into a "Recipe", so that you can launch a sequence like:
	- Build Platform X script
	- Launch Cook-on-the-fly
	- Wait X seconds
	- Launch game instance 1
	- Launch game instance 2