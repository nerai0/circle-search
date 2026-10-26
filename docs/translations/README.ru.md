<div align="center">

<img src="../../assets/icon.png" width="112" alt="CircleSearch" />

# 🔍 CircleSearch для Windows
### Быстрый и удобный Circle to Search для Windows 10 & 11

[![Release](https://img.shields.io/github/v/release/nerai0/circle-search?style=flat-square&color=4169E1)](https://github.com/nerai0/circle-search/releases)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg?style=flat-square)](../../LICENSE)
[![Platform](https://img.shields.io/badge/Platform-Windows%2010%20%7C%2011%20(x64)-0078D6?style=flat-square&logo=windows)](https://github.com/nerai0/circle-search)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)

[**English**](../../README.md) | [**Қазақша**](README.kk.md) | [**Русский**](README.ru.md) | [**Español**](README.es.md) | [**Deutsch**](README.de.md) | [**Français**](README.fr.md) | [**Português**](README.pt.md) | [**Türkçe**](README.tr.md) | [**Українська**](README.uk.md) | [**中文**](README.zh.md) | [**日本語**](README.ja.md) | [**한국어**](README.ko.md)

</div>

---

**CircleSearch** переносит фирменный жест **Circle to Search** (Поиск кругом) со смартфонов прямо на ваш ПК. Зажмите клавишу **`Win`** (или нажмите **`Alt + Space`**) в любой точке экрана, чтобы мгновенно заморозить отображение, выделить нужную область и получить результаты поиска Google Lens, распознанный текст (OCR) и перевод во встроенной боковой панели — **100% бесплатно, без API-ключей и без подписок**.

---

## ✨ Возможности

- ⚡ **Вызов удержанием клавиши `Win` (400 мс):** Низкоуровневый хук клавиатуры Win32 (`WH_KEYBOARD_LL`) перехватывает долгое нажатие и блокирует открытие меню «Пуск». Обычные короткие клики по `Win` и все системные сочетания (`Win+R`, `Win+D`, `Win+V`) продолжают работать без задержек.
- 🎯 **Интерактивная рамка выделения:** Выделите любую область экрана. Тяните за 8 контрольных точек по углам и граням или перемещайте всю рамку целиком — результаты обновляются в реальном времени.
- 🌐 **Встроенный браузер Google Lens:** Реализован на базе `Microsoft.Web.WebView2`. Отображает карточки товаров, похожие картинки, выделяемый текст и перевод прямо в выдвижной панели.
- 🖥️ **Поддержка нескольких мониторов и любого DPI:** Полная поддержка `PerMonitorV2`. Приложение корректно работает на конфигурациях с экранами разного масштаба (`100%`, `125%`, `150%`, `200%`, `2K`, `4K`).
- 🖱️ **Умный выбор монитора:** По умолчанию оверлей открывается на мониторе, где расположен курсор мыши, либо на мониторе с активным окном (настраивается).
- 🚀 **Сканирование активного окна или всего экрана в один клик:** Удобные кнопки внизу экрана для быстрого поиска по текущему приложению или всему рабочему столу.
- 🌍 **Автоопределение языка системы & 12 встроенных локализаций:** Язык интерфейса автоматически подстраивается под язык Windows, а в настройках доступен ручной выбор любого языка (English, Қазақша, Русский, Español, Deutsch, Français, Português, Türkçe, Українська, 中文, 日本語, 한국어).
- 🎨 **Современный интерфейс и темы:** Лаконичный стиль Material, темная и светлая темы, фирменный цвет Royal Blue, плавная прокрутка настроек и возможность автозапуска вместе с Windows.

---

## 📥 Скачивание и установка

> **Примечание:** CircleSearch ультралегковесен (~650 КБ) и использует [.NET 10 Desktop Runtime (x64)](https://dotnet.microsoft.com/download/dotnet/10.0). Если рантайм ещё не установлен в системе, Windows предложит установить его в один клик при первом запуске.


### Вариант 1: Установщик Windows (`.msi`) — *Рекомендуется*
Скачайте актуальный **`CircleSearch-v<version>-win-x64.msi`** из раздела [**Releases**](https://github.com/nerai0/circle-search/releases).
- Установка выполняется в профиль пользователя (права администратора не требуются).
- Автоматически создает ярлыки в меню «Пуск» и на рабочем столе с иконкой приложения.

### Вариант 2: Портативный архив (`.zip`)
Скачайте **`CircleSearch-v<version>-win-x64-portable.zip`**, распакуйте в любую папку и запустите `CircleSearch.exe`.

---

## ⌨️ Горячие клавиши и управление

| Действие | Горячая клавиша |
| :--- | :--- |
| **Вызов оверлея** | Удержание клавиши `Win` 400 мс *или* `Alt + Space` |
| **Выделение области** | Зажать левую кнопку мыши и растянуть рамку |
| **Изменение размера** | Потянуть за любой из 8 маркеров на гранях рамки |
| **Перемещение области** | Кликнуть и потянуть внутри рамки выделения |
| **Развернуть панель** | Кнопка *Шире / Уже* в шапке боковой панели |
| **Закрыть оверлей** | Клавиша `Escape` или кнопка `X` в правом верхнем углу |

---

## 🛠️ Сборка из исходников

### Требования:
- Windows 10/11 x64
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Среда Microsoft Edge WebView2 (встроена в современные версии Windows)

```powershell
# Клонирование репозитория
git clone https://github.com/nerai0/circle-search.git
cd circle-search

# Запуск в режиме разработки
dotnet run --project src/CircleSearch.App/CircleSearch.App.csproj

# Сборка .msi инсталлятора и портативного .zip
powershell -ExecutionPolicy Bypass -File .\installer\build-release.ps1   # version is read from CircleSearch.App.csproj
```

---

## ⚙️ Настройка

- Настройки хранятся в `%LocalAppData%\CircleSearch\settings.json` и меняются в окне **Настройки** (значок в трее → правая кнопка мыши).
- Необязательный `.env` рядом с `CircleSearch.exe` задаёт начальные значения, см. [`.env.example`](../../.env.example). Значения, сохранённые в настройках, приоритетнее.
- Переменная `CIRCLESEARCH_CONFIG_DIR` переопределяет папку настроек.
- Для открытия поиска во внешнем окне используется **браузер по умолчанию**, выбранный в Windows.

## 🧱 Структура проекта

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

Запуск тестов: `dotnet run --project tests/CircleSearch.Tests -c Release`

---

## 📄 Лицензия

Распространяется под лицензией [MIT](../../LICENSE). Полностью бесплатный открытый исходный код (Open Source).
