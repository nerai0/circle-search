<div align="center">

<img src="assets/icon.png" width="112" alt="CircleSearch" />

# 🔍 CircleSearch for Windows
### Windows 10 & 11 向け高速ネイティブ「かこって検索 (Circle to Search)」

[![Release](https://img.shields.io/github/v/release/nerai0/circle-search?style=flat-square&color=4169E1)](https://github.com/nerai0/circle-search/releases)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg?style=flat-square)](LICENSE)
[![Platform](https://img.shields.io/badge/Platform-Windows%2010%20%7C%2011%20(x64)-0078D6?style=flat-square&logo=windows)](https://github.com/nerai0/circle-search)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)

[**English**](README.md) | [**Қазақша**](README.kk.md) | [**Русский**](README.ru.md) | [**Español**](README.es.md) | [**Deutsch**](README.de.md) | [**Français**](README.fr.md) | [**Português**](README.pt.md) | [**Türkçe**](README.tr.md) | [**Українська**](README.uk.md) | [**中文**](README.zh.md) | [**日本語**](README.ja.md) | [**한국어**](README.ko.md)

</div>

---

**CircleSearch** は、スマートフォンの大人気機能 **かこって検索 (Circle to Search)** を PC で手軽に利用できるようにするツールです。画面上のどこでも **`Win`** キーを長押し（または **`Alt + Space`** を入力）するだけで画面を固定し、囲んだ領域の Google レンズ画像検索・文字認識 (OCR)・翻訳をサイドパネルで即座に表示します —— **完全無料、API キーや有料登録は一切不要**。

---

## ✨ 主な機能

- ⚡ **`Win` キー長押しトリガー (400 ms):** 低レベル Win32 キーボードフック (`WH_KEYBOARD_LL`) により長押しを検知し、Windows スタートメニューのポップアップを防止。通常の単押しやシステムショートカット (`Win+R`, `Win+D`, `Win+V`) はそのまま利用可能。
- 🎯 **リサイズ可能なインタラクティブ選択枠:** 画面の好きな部分を囲んで選択。8つのハンドルでサイズ調整したり、枠全体をドラッグして移動可能。検索結果はリアルタイムに更新されます。
- 🌐 **Google レンズ内蔵ブラウザ:** `Microsoft.Web.WebView2` 採用。サイドパネル内で画像の一致結果、コピー可能なテキスト、リアルタイム翻訳を表示。
- 🖥️ **マルチモニター & 混合 DPI 対応:** `PerMonitorV2` 完全対応。異なる拡大率 (`100%`, `125%`, `150%`, `200%`, `2K`, `4K`) の環境でも綺麗に動作。
- 🖱️ **スマートモニター選択:** マウスカーソルがあるモニター（デフォルト）、または現在アクティブなウィンドウのモニターに自動表示。
- 🚀 **アクティブウィンドウ / 全画面の即時キャプチャ:** 画面下部の専用ボタンから、フォアグラウンドのアプリや全画面を一発で検索可能。
- 🌍 **言語自動判定 & 12言語内蔵:** Windows の表示言語に自動連動。設定から手動でいつでも切り替え可能（英語、カザフ語、ロシア語、スペイン語、ドイツ語、フランス語、ポルトガル語、トルコ語、ウクライナ語、中国語、日本語、韓国語）。
- 🎨 **ミニマルデザイン & テーマ:** 洗練された Material デザイン、ダーク/ライトテーマ、ロイヤルブルーのアクセントカラー、Windows 起動時自動起動に対応。

---

## 📥 ダウンロードとインストール

> **備考:** CircleSearch は超軽量（約 650 KB）で動作には [.NET 10 Desktop Runtime (x64)](https://dotnet.microsoft.com/download/dotnet/10.0) が必要です。未導入の場合、初回起動時に Windows が 1 クリックでのインストールを案内します。

### 方法 1: Windows インストーラー (`.msi`) — *推奨*
[**Releases**](https://github.com/nerai0/circle-search/releases) から最新の **`CircleSearch-v<version>-win-x64.msi`** をダウンロードします。
- ユーザー個別インストール（管理者権限不要）。
- スタートメニューとデスクトップに高解像度アプリアイコン付きショートカットを自動作成。

### 方法 2: ポータブル版 (`.zip`)
**`CircleSearch-v<version>-win-x64-portable.zip`** をダウンロードし、任意のフォルダに解凍して `CircleSearch.exe` を実行します。

---

## ⌨️ ショートカット & 操作方法

| 操作 | ショートカット |
| :--- | :--- |
| **オーバーレイ起動** | `Win` キーを 400 ms 長押し *または* `Alt + Space` |
| **領域選択** | 画面上で左ドラッグ |
| **サイズ調整** | 枠の角や辺にある 8 つのハンドルをドラッグ |
| **枠の移動** | 選択枠の内側をクリックしてドラッグ |
| **サイドパネルの伸縮** | ヘッダーの *幅広 / 標準* ボタンをクリック |
| **閉じる** | `Escape` キーまたは右上の `X` ボタン |

---

## 🛠️ ソースコードからのビルド

### 必要要件
- Windows 10/11 x64
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Microsoft Edge WebView2 Runtime (Windows 標準搭載)

```powershell
# リポジトリのクローン
git clone https://github.com/nerai0/circle-search.git
cd circle-search

# 開発モードで実行
dotnet run --project src/CircleSearch.App/CircleSearch.App.csproj

# .msi インストーラーおよび .zip の作成
powershell -ExecutionPolicy Bypass -File .\installer\build-release.ps1
```

---

## 📄 ライセンス

本ソフトウェアは [MIT License](LICENSE) のもとで公開されており、個人利用・商用利用を問わず完全無料です。
