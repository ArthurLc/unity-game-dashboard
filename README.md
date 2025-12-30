# Game Dashboard

**Game Dashboard** is a Unity Editor package that provides a centralized dashboard window to help you visualize and manage key project data from a single place.

Its goal is to reduce context switching, improve project overview, and let you focus more on game development rather than searching through your project files.


### Requirements

- Unity **6000.0** or newer
- Editor-only package
- No runtime dependencies


### Importing

You can add Game Dashboard to your project using the Unity Package Manager:

- [Install from a local folder](https://docs.unity3d.com/Manual/upm-ui-local.html)
- [Install from a Git URL](https://docs.unity3d.com/Manual/upm-ui-giturl.html)


## Getting Started

- Add data to the dashboard via:

Window → Game Dashboard → Settings...

- Open the dashboard via:

Window → Game Dashboard →  Game Dashboard

- (Optional) [Customize the toolbar](https://docs.unity3d.com/Manual/Toolbar.html) to add button shortcut.

> 💡 Toolbar overlays are disabled by default by Unity and must be enabled manually by the user.


## Third Parties

Game Dashboard provides optional compatibility with the following packages:

### ✔ [EditorAttributes](https://github.com/v0lt13/EditorAttributes)
- Automatically detects and supports EditorAttributes if present
- Enhances data visualization and editor UI
- No hard dependency required

### ✔ [unity-toolbar-extender](https://github.com/marijnz/unity-toolbar-extender)
- Used for toolbar integration on older Unity versions
- Supported **up to Unity 6000.2 inclusive**
- Automatically disabled on newer Unity versions

> Integrations are enabled only if the corresponding package is installed in the project.
