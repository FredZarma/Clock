# ZTime

ZTime is a free digital desktop clock for Windows.

A small always-on-top gadget you can drag like a desktop icon, with a 7-segment LED display, date, calendar, alarm and themes.

**Copyright Fred Zarma 2026** — zarma@sylm.info

## Download

The latest version is in [Releases](../../releases): the `ZTime.exe` file.

Double-click `ZTime.exe` to run it without installing, or run `Setup.exe` (administrator) to install. You choose the folder, the desktop shortcut, and the Start menu shortcut.

To uninstall: right-click the clock and choose **Uninstall...**, or use Apps & features.

Windows 10 or 11, 64-bit. Requires .NET Framework 4 (already present on Windows). Microsoft has certified the app.

## Usage

| Action | Effect |
| --- | --- |
| Drag | Move (snaps to the icon grid) |
| Double-click | Calendar |
| Right-click | Menu |

In the menu: language flags (English, Mandarin, Hindi, Spanish, French, Russian, Japanese, German), calendar, copy the time, size, color, seconds, date, 12/24-hour format, alarm, icon name (change or hide), always on top, run at startup, uninstall, copyright (zarma@sylm.info).

## Build

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\Build.ps1
```

## Install / uninstall

See `NEXT.md`.

## License

Free, MIT license. See `LICENSE`.
