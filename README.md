# Clock

A free digital desktop clock for Windows.

A small always-on-top gadget you can drag like a desktop icon, with a 7-segment LED display, date, calendar, alarm and themes.

**Copyright Fred Zarma 2026** — zarma@sylm.info

## Download

The latest version is in [Releases](../../releases): the `Clock.exe` file.

No installer. Double-click to run. Copy the exe wherever you like (desktop, etc.).

Windows 10 or 11, 64-bit. Requires .NET Framework 4 (already present on Windows).

On first launch, Windows SmartScreen may show a warning (unsigned application). Choose *More info*, then *Run anyway*.

## Usage

| Action | Effect |
| --- | --- |
| Drag | Move (snaps to the icon grid) |
| Double-click | Calendar |
| Right-click | Menu |

In the menu: language flags (English, Mandarin, Hindi, Spanish, French, Russian, Japanese, German), calendar, copy the time, size, color, seconds, date, 12/24-hour format, alarm, icon name (change or hide), always on top, run at startup, copyright (zarma@sylm.info).

## Build

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\Build.ps1
```

## Next version

See `NEXT.md`: installer (copy into Program Files + desktop shortcut) and clean uninstall.

## License

Free, MIT license. See `LICENSE`.
