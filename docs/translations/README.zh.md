<div align="center">

<img src="../../assets/icon.png" width="112" alt="CircleSearch" />

# 🔍 CircleSearch for Windows
### Windows 10 与 11 上的原生极速 Circle to Search 体验

[![Release](https://img.shields.io/github/v/release/nerai0/circle-search?style=flat-square&color=4169E1)](https://github.com/nerai0/circle-search/releases)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg?style=flat-square)](../../LICENSE)
[![Platform](https://img.shields.io/badge/Platform-Windows%2010%20%7C%2011%20(x64)-0078D6?style=flat-square&logo=windows)](https://github.com/nerai0/circle-search)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)

[**English**](../../README.md) | [**Қазақша**](README.kk.md) | [**Русский**](README.ru.md) | [**Español**](README.es.md) | [**Deutsch**](README.de.md) | [**Français**](README.fr.md) | [**Português**](README.pt.md) | [**Türkçe**](README.tr.md) | [**Українська**](README.uk.md) | [**中文**](README.zh.md) | [**日本語**](README.ja.md) | [**한국어**](README.ko.md)

</div>

---

**CircleSearch** 将广受欢迎的 **Circle to Search（即圈即搜）** 手势带到您的 PC。在屏幕任意位置长按 **`Win`** 键（或按 **`Alt + Space`**）即可冻结画面，拖拽圈选任意区域，在精美的侧边栏中即时获取 Google Lens 视觉搜索、文字识别 (OCR) 和翻译结果 —— **100% 免费，无需 API Key，无任何订阅**。

---

## ✨ 核心特性

- ⚡ **长按 `Win` 键触发 (400 ms):** 底层 Win32 键盘钩子 (`WH_KEYBOARD_LL`) 精准检测长按并屏蔽 Windows 开始菜单弹出。正常的单按及系统快捷键 (`Win+R`, `Win+D`, `Win+V`) 零延迟正常使用。
- 🎯 **交互式可调选区:** 自由框选屏幕任意区域。拖拽 8 个控制点调整大小，或整体平移选区 —— 搜索结果实时刷新。
- 🌐 **内嵌 Google Lens 浏览器:** 基于 `Microsoft.Web.WebView2`。在右侧滑动面板中直接显示视觉匹配项、可复制文本和即时翻译。
- 🖥️ **支持多显示器与混合 DPI:** 深度集成 `PerMonitorV2`。在不同缩放比例 (`100%`, `125%`, `150%`, `200%`, `2K`, `4K`) 的多屏环境下完美运行。
- 🖱️ **智能目标显示器:** 默认在鼠标指针所在屏幕呼出，或选择在当前活动窗口所在的屏幕打开。
- 🚀 **一键扫描活动窗口 / 全屏:** 底部提供专属快捷按钮，秒级检索当前前台应用或整个屏幕。
- 🌍 **系统语言自适应 & 12 种内置语言:** 启动时自动匹配 Windows 系统语言，亦可在设置中随时手动切换（英语、哈萨克语、俄语、西班牙语、德语、法语、葡萄牙语、土耳其语、乌克兰语、中文、日语、韩语）。
- 🎨 **极简设计与主题:** 现代化 Material 风格、暗黑/明亮双主题、皇家蓝主题色、无冗余视觉干扰，支持 Windows 开机自启。

---

## 📥 下载与安装

> **说明:** CircleSearch 极为轻量（约 650 KB），运行基于 [.NET 10 Desktop Runtime (x64)](https://dotnet.microsoft.com/download/dotnet/10.0)。若系统尚未安装，首次运行时 Windows 将提示一键自动安装。

### 选项 1: Windows 安装程序 (`.msi`) — *推荐*
从 [**Releases**](https://github.com/nerai0/circle-search/releases) 下载最新的 **`CircleSearch-v<version>-win-x64.msi`**。
- 单用户安装（无需管理员权限）。
- 自动生成带高清图标的开始菜单和桌面快捷方式。

### 选项 2: 便携压缩包 (`.zip`)
下载 **`CircleSearch-v<version>-win-x64-portable.zip`**，解压至任意目录即可运行 `CircleSearch.exe`。

---

## ⌨️ 快捷键与操作

| 操作 | 快捷键 |
| :--- | :--- |
| **呼出覆盖层** | 长按 `Win` 键 400 毫秒 *或* 按 `Alt + Space` |
| **框选区域** | 鼠标左键点击并拖拽框选任意区域 |
| **调整大小** | 拖动边缘或边角的 8 个控制把手 |
| **移动选区** | 在选区框内按住左键拖动 |
| **展开侧边栏** | 点击侧边栏顶部的 *更宽 / 更窄* 按钮 |
| **退出 / 关闭** | 按 `Escape` 键或点击右上角 `X` 按钮 |

---

## 🛠️ 从源码构建

### 前置要求
- Windows 10/11 x64
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Microsoft Edge WebView2 Runtime（现代 Windows 系统内置）

```powershell
# 克隆仓库
git clone https://github.com/nerai0/circle-search.git
cd circle-search

# 开发模式运行
dotnet run --project src/CircleSearch.App/CircleSearch.App.csproj

# 构建 .msi 安装包及绿色便携版 .zip
powershell -ExecutionPolicy Bypass -File .\installer\build-release.ps1
```

---

## ⚙️ 配置说明

- 配置文件保存在 `%LocalAppData%\CircleSearch\settings.json`，可在托盘图标右键菜单的 **Settings** 中设置。
- 根目录可选放置 `.env` 提供初始默认项，详见 [`.env.example`](../../.env.example)。
- 支持通过 `CIRCLESEARCH_CONFIG_DIR` 环境变量自定义配置存储路径。
- 在外部打开搜索时，自动调用 Windows 系统设置中的**默认浏览器**。

---

## 📄 开源许可证

本项目基于 [MIT License](../../LICENSE) 开源发布，完全免费，支持个人及商业使用。
