<div align="center">

<img src="assets/icon.png" width="112" alt="CircleSearch" />

# 🔍 Windows-қа арналған CircleSearch
### Windows 10 & 11 жүйелерінде жылдам әрі ыңғайлы Circle to Search мүмкіндігі

[![Release](https://img.shields.io/github/v/release/nerai0/circle-search?style=flat-square&color=4169E1)](https://github.com/nerai0/circle-search/releases)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg?style=flat-square)](LICENSE)
[![Platform](https://img.shields.io/badge/Platform-Windows%2010%20%7C%2011%20(x64)-0078D6?style=flat-square&logo=windows)](https://github.com/nerai0/circle-search)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)

[**English**](README.md) | [**Қазақша**](README.kk.md) | [**Русский**](README.ru.md) | [**Español**](README.es.md) | [**Deutsch**](README.de.md) | [**Français**](README.fr.md) | [**Português**](README.pt.md) | [**Türkçe**](README.tr.md) | [**Українська**](README.uk.md) | [**中文**](README.zh.md) | [**日本語**](README.ja.md) | [**한국어**](README.ko.md)

</div>

---

**CircleSearch** — Android жүйесіндегі танымал әрі ыңғайлы **Circle to Search** мүмкіндігін дербес компьютеріңізге әкеледі. Экранның кез келген жерінде **`Win`** пернесін басып тұрыңыз (немесе **`Alt + Space`** басыңыз) — экран лезде бекітіледі. Кез келген аймақты белгілеп немесе өлшемін өзгертіп, кіріктірілген Google Lens бүйірлік тақтасы арқылы визуалды іздеуді, мәтінді тануды (OCR) және аударманы қолданыңыз. **100% тегін, API кілттерсіз және ешқандай жазылымсыз**.

---

## ✨ Негізгі артықшылықтары

- ⚡ **Win пернесін басып тұру (400 мс):** Төменгі деңгейдегі Win32 пернетақта ілмегі (`WH_KEYBOARD_LL`) ұзақ басуды дәл анықтайды және «Пуск» мәзірінің кездейсоқ ашылуын бөгейді. Қарапайым шертулер мен стандартты Windows тіркесімдері (`Win+R`, `Win+D`, `Win+V`) әдеттегідей жұмыс істейді.
- 🎯 **Интерактивті әрі өлшемі өзгеретін таңдау:** Экрандағы кез келген бөлікті белгілеңіз. Жиектер мен бұрыштардағы 8 нүктені тартып өлшемін өзгертіңіз немесе бүкіл таңдалған аймақты жылжытыңыз — іздеу нәтижесі лезде жаңарады.
- 🌐 **Кіріктірілген Google Lens браузері:** `Microsoft.Web.WebView2` арқылы жасалған. Нәтижелерді, сәйкес кескіндерді, көшіруге болатын мәтіндерді және аударманы тікелей оверлейдің бүйір тақтасында көрсетеді.
- 🖥️ **Бірнеше монитор мен әртүрлі DPI қолдауы:** Толық дербес монитор масштабтауы (`PerMonitorV2`). Әртүрлі масштабтағы (`100%`, `125%`, `150%`, `200%`, `2K`, `4K`) және екі не одан көп мониторы бар жүйелерде мінсіз жұмыс істейді.
- 🖱️ **Мақсатты мониторды ақылды таңдау:** Оверлей әдепкі бойынша тінтуір меңзері тұрған мониторда немесе белсенді терезе орналасқан мониторда ашылады (баптаулардан өзгертуге болады).
- 🚀 **Бір басу арқылы белсенді терезе немесе бүкіл экран:** Астыңғы жақтағы түймелер арқылы белсенді бағдарлама терезесін немесе бүкіл жұмыс үстелін лезде сканерлеу.
- 🌍 **Жүйе тілін автоматты анықтау & 12 кіріктірілген тіл:** Алғашқы іске қосқанда Windows жүйеңіздің тілін өзі табады (Қазақша, Ағылшын, Орыс, Испан, Неміс, Француз, Португал, Түрік, Украин, Қытай, Жапон, Корей). Қаласаңыз, баптаулардан қолмен таңдауға болады.
- 🎨 **Заманауи әрі ықшам UI:** Google Material стиліндегі күңгірт және ашық тақырыптар, Royal Blue екпінді түсі, артық көз алаңдататын анимациялардың болмауы және Windows-пен бірге автоіске қосу мүмкіндігі.

---

## 📥 Жүктеп алу және орнату

> **Ескертпе:** CircleSearch өте жеңіл (~650 КБ) және жұмысы үшін [.NET 10 Desktop Runtime (x64)](https://dotnet.microsoft.com/download/dotnet/10.0) қажет етеді. Егер ол жүйеде орнатылмаған болса, Windows бірінші рет қосқанда оны бір басу арқылы орнатуды ұсынады.


### 1-нұсқа: Windows орнатқышы (`.msi`) — *Ұсынылады*
[**Releases**](https://github.com/nerai0/circle-search/releases) бетінен соңғы **`CircleSearch-v<version>-win-x64.msi`** файлын жүктеп алыңыз.
- Әкімші құқығын талап етпейді (пайдаланушы профиліне орнатылады).
- «Пуск» мәзірі мен жұмыс үстеліне әдемі иконкасы бар жарлықтарды автоматты түрде қосады.

### 2-нұсқа: Тасымалды нұсқа (`.zip`)
**`CircleSearch-v<version>-win-x64-portable.zip`** файлын жүктеп алып, кез келген бумаға шығарыңыз және `CircleSearch.exe` іске қосыңыз.

---

## ⌨️ Ыстық пернелер мен басқару

| Әрекет | Ыстық перне |
| :--- | :--- |
| **Оверлейді шақыру** | `Win` пернесін 400 мс басып тұру *немесе* `Alt + Space` басу |
| **Аймақты таңдау** | Тінтуірдің сол жақ батырмасымен экранда созып белгілеу |
| **Өлшемін өзгерту** | Жиектер мен бұрыштардағы 8 ақ нүктені тарту |
| **Таңдауды жылжыту** | Таңдалған рамканың ішінен ұстап жылжыту |
| **Панельді кеңейту** | Бүйір тақтаның тақырыбындағы *Кеңейту / Кішірейту* түймесі |
| **Жабу / Болдырмау** | `Escape` пернесі немесе оң жақ жоғарғы `X` түймесі |

---

## 🛠️ Бастапқы кодтан құрастыру

### Талаптар:
- Windows 10/11 x64
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Microsoft Edge WebView2 Runtime (қазіргі Windows жүйелерінде орнатылған)

```powershell
# Репозиторийді жүктеу
git clone https://github.com/nerai0/circle-search.git
cd circle-search

# Әзірлеу режимінде іске қосу
dotnet run --project src/CircleSearch.App/CircleSearch.App.csproj

# .msi орнатқышы мен тасымалды .zip құрастыру
powershell -ExecutionPolicy Bypass -File .\installer\build-release.ps1   # version is read from CircleSearch.App.csproj
```

---

## ⚙️ Баптау

- Баптаулар `%LocalAppData%\CircleSearch\settings.json` файлында сақталады және **Баптаулар** терезесінде өзгертіледі (трей белгішесі → тінтуірдің оң жағы).
- `CircleSearch.exe` жанындағы міндетті емес `.env` файлы бастапқы мәндерді береді, [`.env.example`](.env.example) қараңыз. Баптауларда сақталған мәндердің басымдығы жоғары.
- `CIRCLESEARCH_CONFIG_DIR` айнымалысы баптаулар қалтасын ауыстырады.
- Іздеу сыртқы терезеде ашылғанда Windows-та таңдалған **әдепкі браузер** қолданылады.

## 🧱 Жоба құрылымы

```text
src/CircleSearch.App/
├── Domain/          # settings (ConfigManager)
├── Infrastructure/  # Win32: capture, keyboard hook, autostart, default browser, logging
├── Services/        # Google Lens integration, localization
├── UI/              # theme tokens and styles, adaptive metrics, overlay/settings windows, components
├── Locales/         # <lang>/common.json (12 languages)
└── Resources/       # application icon
tests/CircleSearch.Tests/   # console test runner
installer/                  # WiX MSI + build-release.ps1
```

Тестілерді іске қосу: `dotnet run --project tests/CircleSearch.Tests -c Release`

---

## 📄 Лицензия

[MIT License](LICENSE) бойынша таратылады. Жеке және коммерциялық мақсатта пайдалануға толықтай ашық және еркін.
