<div align="center">

<img src="../../assets/icon.png" width="112" alt="CircleSearch" />

# 🔍 CircleSearch für Windows
### Das native, schnelle Circle to Search Erlebnis für Windows 10 & 11

[![Release](https://img.shields.io/github/v/release/nerai0/circle-search?style=flat-square&color=4169E1)](https://github.com/nerai0/circle-search/releases)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg?style=flat-square)](../../LICENSE)
[![Platform](https://img.shields.io/badge/Platform-Windows%2010%20%7C%2011%20(x64)-0078D6?style=flat-square&logo=windows)](https://github.com/nerai0/circle-search)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)

[**English**](../../README.md) | [**Қазақша**](README.kk.md) | [**Русский**](README.ru.md) | [**Español**](README.es.md) | [**Deutsch**](README.de.md) | [**Français**](README.fr.md) | [**Português**](README.pt.md) | [**Türkçe**](README.tr.md) | [**Українська**](README.uk.md) | [**中文**](README.zh.md) | [**日本語**](README.ja.md) | [**한국어**](README.ko.md)

</div>

---

**CircleSearch** bringt die beliebte **Circle to Search** Geste von Smartphones direkt auf Ihren PC. Halten Sie die **`Win`**-Taste gedrückt (oder drücken Sie **`Alt + Leertaste`**) an einer beliebigen Stelle Ihres Bildschirms, um die Anzeige einzufrieren, einen Bereich auszuwählen und blitzschnell visuelle Google Lens Suchen, OCR und Übersetzungen in einer eleganten Seitenleiste zu erhalten — **100% kostenlos, ohne API-Schlüssel oder Abonnements**.

---

## ✨ Funktionen

- ⚡ **Aktivierung durch Halten der `Win`-Taste (400 ms):** Ein nativer Win32-Tastatur-Hook (`WH_KEYBOARD_LL`) fängt langes Drücken ab und verhindert das Öffnen des Windows-Startmenüs. Kurze Klicks und Systemkombinationen (`Win+R`, `Win+D`, `Win+V`) funktionieren wie gewohnt.
- 🎯 **Interaktiver & anpassbarer Auswahlrahmen:** Wählen Sie einen beliebigen Bereich des Bildschirms aus. Ziehen Sie an den 8 Griffen oder verschieben Sie das gesamte Auswahlfeld — die Ergebnisse werden sofort aktualisiert.
- 🌐 **Integrierter Google Lens Browser:** Basiert auf `Microsoft.Web.WebView2`. Zeigt Suchergebnisse, Bildtreffer, kopierbaren Text und Echtzeit-Übersetzungen direkt in einer Seitenleiste an.
- 🖥️ **Multi-Monitor & gemischte DPI-Werte:** Vollständige Unterstützung für `PerMonitorV2`. Funktioniert reibungslos auf Monitoren mit unterschiedlichen Skalierungsfaktoren (`100%`, `125%`, `150%`, `200%`, `2K`, `4K`).
- 🖱️ **Intelligenter Zielmonitor:** Öffnet sich standardmäßig auf dem Monitor unter dem Mauszeiger oder auf dem Monitor mit dem aktiven Vordergrundfenster.
- 🚀 **Aktives Fenster oder Vollbild mit einem Klick:** Schnelltasten am unteren Rand zum blitzschnellen Scannen des aktuellen Fensters oder des gesamten Desktops.
- 🌍 **Automatische Spracherkennung & 12 Sprachen:** Passt sich automatisch der Systemsprache von Windows an und bietet manuelle Auswahl in den Einstellungen (Englisch, Kasachisch, Russisch, Spanisch, Deutsch, Französisch, Portugiesisch, Türkisch, Ukrainisch, Chinesisch, Japanisch und Koreanisch).
- 🎨 **Minimalistisches Design & Themes:** Modernes Material-Design, dunkles und helles Design, Royal-Blue-Akzente und optionaler Windows-Autostart.

---

## 📥 Herunterladen und Installieren

> **Hinweis:** CircleSearch ist ultraleicht (~650 KB) und benötigt [.NET 10 Desktop Runtime (x64)](https://dotnet.microsoft.com/download/dotnet/10.0). Falls noch nicht installiert, bietet Windows beim ersten Start die 1-Klick-Installation an.

### Option 1: Windows Installer (`.msi`) — *Empfohlen*
Laden Sie den neuesten **`CircleSearch-v<version>-win-x64.msi`** unter [**Releases**](https://github.com/nerai0/circle-search/releases) herunter.
- Benutzerbezogene Installation (keine Administratorrechte erforderlich).
- Erstellt automatisch Verknüpfungen im Startmenü und auf dem Desktop mit hochauflösendem Icon.

### Option 2: Portables Archiv (`.zip`)
Laden Sie **`CircleSearch-v<version>-win-x64-portable.zip`** herunter, entpacken Sie es in einen beliebigen Ordner und starten Sie `CircleSearch.exe`.

---

## ⌨️ Tastenkombinationen & Steuerung

| Aktion | Tastenkombination |
| :--- | :--- |
| **Overlay aufrufen** | `Win`-Taste 400 ms gedrückt halten *oder* `Alt + Leertaste` |
| **Bereich auswählen** | Linke Maustaste gedrückt halten und ziehen |
| **Größe anpassen** | An einem der 8 Anfasser an Kanten/Ecken ziehen |
| **Bereich verschieben** | Innerhalb des Rahmens klicken und ziehen |
| **Seitenleiste verbreitern** | Schaltfläche *Breiter / Schmaler* in der Kopfzeile |
| **Schließen** | `Escape`-Taste oder das `X` oben rechts anklicken |

---

## 🛠️ Aus dem Quellcode bauen

### Voraussetzungen
- Windows 10/11 x64
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Microsoft Edge WebView2 Runtime (vorinstalliert auf modernen Windows-Systemen)

```powershell
# Repository klonen
git clone https://github.com/nerai0/circle-search.git
cd circle-search

# Im Entwicklungsmodus ausführen
dotnet run --project src/CircleSearch.App/CircleSearch.App.csproj

# .msi-Installer und portable .zip erstellen
powershell -ExecutionPolicy Bypass -File .\installer\build-release.ps1
```

---

## ⚙️ Konfiguration

- Einstellungen werden in `%LocalAppData%\CircleSearch\settings.json` gespeichert und über das Kontextmenü des Tray-Icons angepasst.
- Eine optionale `.env`-Datei neben `CircleSearch.exe` ermöglicht Standardvorgaben (siehe [`.env.example`](../../.env.example)).
- `CIRCLESEARCH_CONFIG_DIR` überschreibt das Konfigurationsverzeichnis.
- Bei externem Öffnen wird Ihr gewohnter Windows-Standardbrowser verwendet.

---

## 📄 Lizenz

Veröffentlicht unter der [MIT-Lizenz](../../LICENSE). Open-Source und kostenlos für private und gewerbliche Nutzung.
