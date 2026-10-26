<div align="center">

<img src="../../assets/icon.png" width="112" alt="CircleSearch" />

# 🔍 CircleSearch pour Windows
### L'expérience native et ultra-rapide de Circle to Search sous Windows 10 & 11

[![Release](https://img.shields.io/github/v/release/nerai0/circle-search?style=flat-square&color=4169E1)](https://github.com/nerai0/circle-search/releases)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg?style=flat-square)](../../LICENSE)
[![Platform](https://img.shields.io/badge/Platform-Windows%2010%20%7C%2011%20(x64)-0078D6?style=flat-square&logo=windows)](https://github.com/nerai0/circle-search)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)

[**English**](../../README.md) | [**Қазақша**](README.kk.md) | [**Русский**](README.ru.md) | [**Español**](README.es.md) | [**Deutsch**](README.de.md) | [**Français**](README.fr.md) | [**Português**](README.pt.md) | [**Türkçe**](README.tr.md) | [**Українська**](README.uk.md) | [**中文**](README.zh.md) | [**日本語**](README.ja.md) | [**한국어**](README.ko.md)

</div>

---

**CircleSearch** apporte le geste populaire **Circle to Search** (Entourer pour chercher) de votre smartphone directement sur votre PC. Maintenez la touche **`Win`** enfoncée (ou appuyez sur **`Alt + Espace`**) n'importe où sur l'écran pour figer l'affichage, sélectionner une zone et profiter instantanément de la recherche visuelle Google Lens, de l'OCR et de la traduction dans un volet latéral élégant — **100% gratuit, sans clé API ni abonnement**.

---

## ✨ Fonctionnalités

- ⚡ **Déclenchement par appui long sur `Win` (400 ms) :** Un hook clavier Win32 bas niveau (`WH_KEYBOARD_LL`) détecte l'appui long et bloque l'ouverture intempestive du menu Démarrer. Les frappes brèves et les raccourcis système (`Win+R`, `Win+D`, `Win+V`) continuent de fonctionner parfaitement.
- 🎯 **Sélection interactive et redimensionnable :** Sélectionnez n'importe quelle partie de l'écran. Ajustez les 8 poignées de dimensionnement ou déplacez le cadre complet — les résultats s'actualisent en temps réel.
- 🌐 **Navigateur Google Lens intégré :** Basé sur `Microsoft.Web.WebView2`. Affiche les correspondances visuelles, le texte copiable et les traductions directes dans un panneau latéral modulable.
- 🖥️ **Multi-écrans et gestion DPI mixte :** Prise en charge intégrale de `PerMonitorV2`. Fonctionne sans accroc sur les configurations multi-écrans avec des échelles différentes (`100%`, `125%`, `150%`, `200%`, `2K`, `4K`).
- 🖱️ **Écran cible intelligent :** S'ouvre par défaut sur le moniteur sous le curseur de la souris, ou sur celui de la fenêtre active au premier plan.
- 🚀 **Capture rapide de la fenêtre active ou plein écran :** Boutons d'accès rapide dédiés pour analyser immédiatement l'application en cours ou la totalité de l'écran.
- 🌍 **Détection automatique de la langue & 12 langues intégrées :** S'adapte automatiquement à la langue de Windows, avec réglage manuel possible (Anglais, Kazakh, Russe, Espagnol, Allemand, Français, Portugais, Turc, Ukrainien, Chinois, Japonais et Coréen).
- 🎨 **Design minimaliste et thèmes :** Style moderne inspiré de Material, thèmes clair et sombre, nuances Royal Blue et option de lancement au démarrage de Windows.

---

## 📥 Téléchargement et installation

> **Remarque :** CircleSearch est ultra-léger (~650 Ko) et nécessite [.NET 10 Desktop Runtime (x64)](https://dotnet.microsoft.com/download/dotnet/10.0). S'il n'est pas déjà présent, Windows proposera de l'installer en un clic lors du premier lancement.

### Option 1 : Installateur Windows (`.msi`) — *Recommandé*
Téléchargez le dernier **`CircleSearch-v<version>-win-x64.msi`** depuis les [**Releases**](https://github.com/nerai0/circle-search/releases).
- Installation par utilisateur (aucun privilège administrateur requis).
- Crée automatiquement les raccourcis dans le menu Démarrer et sur le Bureau avec l'icône haute résolution.

### Option 2 : Archive portable (`.zip`)
Téléchargez **`CircleSearch-v<version>-win-x64-portable.zip`**, décompressez-le et lancez `CircleSearch.exe`.

---

## ⌨️ Raccourcis et commandes

| Action | Raccourci |
| :--- | :--- |
| **Ouvrir l'outil** | Maintenir la touche `Win` 400 ms *ou* `Alt + Espace` |
| **Sélectionner une zone** | Clic gauche et glisser sur l'écran |
| **Redimensionner la sélection** | Glisser l'un des 8 repères sur les bords/coins |
| **Déplacer la zone** | Cliquer et glisser à l'intérieur du cadre |
| **Élargir le panneau latéral** | Bouton *Plus large / Moins large* dans l'en-tête |
| **Fermer** | Touche `Échap` ou clic sur le `X` en haut à droite |

---

## 🛠️ Compilation depuis les sources

### Prérequis
- Windows 10/11 x64
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Runtime Microsoft Edge WebView2 (préinstallé sous Windows)

```powershell
# Cloner le dépôt
git clone https://github.com/nerai0/circle-search.git
cd circle-search

# Lancer en mode développement
dotnet run --project src/CircleSearch.App/CircleSearch.App.csproj

# Compiler l'installateur .msi et l'archive portable .zip
powershell -ExecutionPolicy Bypass -File .\installer\build-release.ps1
```

---

## ⚙️ Configuration

- Les paramètres sont enregistrés dans `%LocalAppData%\CircleSearch\settings.json` et modifiables via le menu de l'icône dans la barre des tâches.
- Un fichier `.env` facultatif peut être placé à côté de `CircleSearch.exe` (voir [`.env.example`](../../.env.example)).
- `CIRCLESEARCH_CONFIG_DIR` permet de personnaliser l'emplacement de configuration.
- Lors de l'ouverture d'une recherche dans le navigateur externe, votre navigateur Windows par défaut est utilisé.

---

## 📄 Licence

Distribué sous la [Licence MIT](../../LICENSE). Open source et gratuit pour un usage personnel et commercial.
