# Next version — install and uninstall

To add, not implemented yet.

## Install

- Create a subdirectory in Program Files, e.g. `C:\Program Files\Clock\` (or `C:\Program Files\Fred Zarma\Clock\`).
- Copy `Clock.exe` there (and the icon if needed).
- Create a desktop shortcut named Clock.
- Ideally also: a Start menu shortcut, and an entry in Apps & features for uninstall.

The installer will probably need administrator rights (write access to Program Files).

## Uninstall (clean)

- Delete the install directory.
- Delete the desktop shortcut.
- Delete the Start menu shortcut.
- Delete the uninstall registry key.
- Leave no leftover files. User settings (`%AppData%\DesktopClock`): offer to keep them or wipe everything.
