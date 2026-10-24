<div align="center">

# 🔍 CircleSearch for Windows
### The fast, native Circle to Search experience on Windows 10 & 11

[![Release](https://img.shields.io/github/v/release/nerai0/circle-search?style=flat-square&color=4169E1)](https://github.com/nerai0/circle-search/releases)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg?style=flat-square)](LICENSE)
[![Platform](https://img.shields.io/badge/Platform-Windows%2010%20%7C%2011%20(x64)-0078D6?style=flat-square&logo=windows)](https://github.com/nerai0/circle-search)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)

[**English**](README.md) | [**Қазақша**](README.kk.md) | [**Русский**](README.ru.md)

</div>

---

**CircleSearch** brings Android's beloved **Circle to Search** gesture to your PC. Hold down the **`Win`** key (or press **`Alt + Space`**) anywhere on your screen to freeze the display, drag or resize any selection area, and get instant visual search, OCR, and translations in a sleek Google Lens side panel — **100% free, no API keys, and no subscriptions required**.

---

## ✨ Features

- ⚡ **Hold `Win` Key Trigger (400 ms):** Low-level Win32 keyboard hook (`WH_KEYBOARD_LL`) detects long-presses and suppresses the Windows Start menu. Normal taps and Windows shortcuts (`Win+R`, `Win+D`, `Win+V`) continue to work seamlessly.
- 🎯 **Interactive & Resizable Selection:** Select any portion of your screen. Drag the 8 resize handles on the corners and edges, or move the entire bounding box — results update in real-time.
- 🌐 **Embedded Google Lens Browser:** Built with `Microsoft.Web.WebView2`. Displays full search results, visual matches, copyable text, and instant translations right inside an adjustable overlay side panel.
- 🖥️ **Multi-Monitor & Mixed DPI Ready:** True per-monitor awareness (`PerMonitorV2`). Works flawlessly across multi-monitor setups with different scaling factors (`100%`, `125%`, `150%`, `200%`, `2K`, `4K`).
- 🖱️ **Smart Target Monitor:** Automatically opens on the monitor under your mouse cursor (default) or on the monitor hosting your active foreground window.
- 🚀 **One-Click Active Window / Full Screen:** Dedicated bottom buttons to quickly scan the active window or capture the entire screen.
- 🌍 **System Language Auto-Detection & 12 Built-in Languages:** Automatically matches your Windows display language out of the box, with full manual override in Settings (English, Kazakh, Russian, Spanish, German, French, Portuguese, Turkish, Ukrainian, Chinese, Japanese, and Korean).
- 🎨 **Minimalist Design & Themes:** Clean Google-inspired dark and light surfaces, Royal Blue accents, zero distracting animations, and optional Windows startup integration.

---

## 📥 Downloads & Installation

### Option 1: Windows Installer (`.msi`) — *Recommended*
Download the latest **`CircleSearch-v1.0.1-win-x64.msi`** from [**Releases**](https://github.com/nerai0/circle-search/releases).
- Runs per-user (no administrator privileges required).
- Automatically adds Start Menu and Desktop shortcuts with high-res icon.

### Option 2: Portable Archive (`.zip`)
Download **`CircleSearch-v1.0.1-win-x64-portable.zip`**, extract to any directory, and run `CircleSearch.exe`.

---

## ⌨️ Shortcuts & Controls

| Action | Shortcut |
| :--- | :--- |
| **Summon Overlay** | Hold `Win` key for 400 ms *or* press `Alt + Space` |
| **Select Region** | Left-click and drag across any area of the screen |
| **Resize Selection** | Drag any of the 8 white handles on borders/corners |
| **Move Selection** | Click and drag inside the selection box |
| **Expand Side Panel** | Click the *Wider / Narrower* button in the panel header |
| **Dismiss / Close** | Press `Escape` or click the `X` button |

---

## 🛠️ Building from Source

### Prerequisites
- Windows 10/11 x64
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Microsoft Edge WebView2 Runtime (pre-installed on modern Windows)

```powershell
# Clone the repository
git clone https://github.com/nerai0/circle-search.git
cd circle-search

# Run in Development mode
dotnet run --project src/CircleSearch.App/CircleSearch.App.csproj

# Build .msi installer and portable .zip
powershell -ExecutionPolicy Bypass -File .\installer\build-release.ps1 -Version 1.0.1
```

---

## 📄 License

Distributed under the [MIT License](LICENSE). Open-source and free for personal and commercial use.
