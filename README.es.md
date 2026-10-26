<div align="center">

<img src="assets/icon.png" width="112" alt="CircleSearch" />

# 🔍 CircleSearch para Windows
### La experiencia nativa y rápida de Circle to Search en Windows 10 y 11

[![Release](https://img.shields.io/github/v/release/nerai0/circle-search?style=flat-square&color=4169E1)](https://github.com/nerai0/circle-search/releases)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg?style=flat-square)](LICENSE)
[![Platform](https://img.shields.io/badge/Platform-Windows%2010%20%7C%2011%20(x64)-0078D6?style=flat-square&logo=windows)](https://github.com/nerai0/circle-search)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)

[**English**](README.md) | [**Қазақша**](README.kk.md) | [**Русский**](README.ru.md) | [**Español**](README.es.md) | [**Deutsch**](README.de.md) | [**Français**](README.fr.md) | [**Português**](README.pt.md) | [**Türkçe**](README.tr.md) | [**Українська**](README.uk.md) | [**中文**](README.zh.md) | [**日本語**](README.ja.md) | [**한국어**](README.ko.md)

</div>

---

**CircleSearch** lleva el popular gesto **Circle to Search** (Rodea para buscar) de los smartphones directamente a tu PC. Mantén presionada la tecla **`Win`** (o presiona **`Alt + Espacio`**) en cualquier parte de la pantalla para congelar la imagen, seleccionar un área y obtener búsquedas visuales instantáneas con Google Lens, reconocimiento óptico de caracteres (OCR) y traducciones en un elegante panel lateral — **100% gratis, sin claves de API ni suscripciones**.

---

## ✨ Características

- ⚡ **Disparador manteniendo la tecla `Win` (400 ms):** Un hook de teclado Win32 de bajo nivel (`WH_KEYBOARD_LL`) detecta pulsaciones largas y suprime el menú Inicio de Windows. Las pulsaciones rápidas y los atajos del sistema (`Win+R`, `Win+D`, `Win+V`) continúan funcionando con total normalidad.
- 🎯 **Selección interactiva y redimensionable:** Selecciona cualquier porción de la pantalla. Ajusta los 8 controladores de tamaño o mueve el recuadro completo: los resultados se actualizan en tiempo real.
- 🌐 **Navegador Google Lens integrado:** Basado en `Microsoft.Web.WebView2`. Muestra resultados de búsqueda, productos visuales, texto copiable y traducciones instantáneas dentro de un panel lateral ajustable.
- 🖥️ **Listo para múltiples monitores y DPI mixto:** Compatible con `PerMonitorV2`. Funciona perfectamente en configuraciones multimonitor con diferentes factores de escala (`100%`, `125%`, `150%`, `200%`, `2K`, `4K`).
- 🖱️ **Monitor objetivo inteligente:** Se abre automáticamente en el monitor bajo el cursor del ratón (por defecto) o en el monitor de la ventana activa en primer plano.
- 🚀 **Captura de ventana activa o pantalla completa:** Botones de acceso rápido inferiores para escanear la ventana activa o capturar toda la pantalla con un solo clic.
- 🌍 **Detección automática de idioma y 12 idiomas integrados:** Se adapta automáticamente al idioma de Windows, con posibilidad de selección manual en Ajustes (Inglés, Kazajo, Ruso, Español, Alemán, Francés, Portugués, Turco, Ucraniano, Chino, Japonés y Coreano).
- 🎨 **Diseño minimalista y temas:** Estilo moderno inspirado en Material, temas claro y oscuro, acentos en Azul Real y opción de inicio automático con Windows.

---

## 📥 Descargas e instalación

> **Nota:** CircleSearch es ultraligero (~650 KB) y requiere [.NET 10 Desktop Runtime (x64)](https://dotnet.microsoft.com/download/dotnet/10.0). Si aún no está instalado, Windows te ofrecerá instalarlo en un clic en el primer inicio.

### Opción 1: Instalador de Windows (`.msi`) — *Recomendado*
Descarga el último **`CircleSearch-v<versión>-win-x64.msi`** desde [**Releases**](https://github.com/nerai0/circle-search/releases).
- Instalación por usuario (no requiere permisos de administrador).
- Añade automáticamente accesos directos al Menú Inicio y al Escritorio con el icono de alta resolución.

### Opción 2: Archivo portátil (`.zip`)
Descarga **`CircleSearch-v<versión>-win-x64-portable.zip`**, extrae en cualquier carpeta y ejecuta `CircleSearch.exe`.

---

## ⌨️ Atajos y controles

| Acción | Atajo |
| :--- | :--- |
| **Abrir la superposición** | Mantener tecla `Win` durante 400 ms *o* `Alt + Espacio` |
| **Seleccionar región** | Clic izquierdo y arrastrar sobre cualquier área |
| **Redimensionar selección** | Arrastrar cualquiera de los 8 marcadores de esquinas/bordes |
| **Mover selección** | Clic y arrastrar dentro del recuadro de selección |
| **Expandir panel lateral** | Botón *Más ancho / Más estrecho* en la cabecera |
| **Cerrar superposición** | Presionar `Escape` o hacer clic en la `X` superior |

---

## 🛠️ Compilación desde el código fuente

### Requisitos previos
- Windows 10/11 x64
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Microsoft Edge WebView2 Runtime (preinstalado en Windows moderno)

```powershell
# Clonar el repositorio
git clone https://github.com/nerai0/circle-search.git
cd circle-search

# Ejecutar en modo desarrollo
dotnet run --project src/CircleSearch.App/CircleSearch.App.csproj

# Compilar instalador .msi y archivo portátil .zip
powershell -ExecutionPolicy Bypass -File .\installer\build-release.ps1
```

---

## ⚙️ Configuración

- La configuración se almacena en `%LocalAppData%\CircleSearch\settings.json` y se edita en **Ajustes** (clic derecho en el icono de la bandeja del sistema).
- Un archivo `.env` opcional junto a `CircleSearch.exe` permite definir valores predeterminados (ver [`.env.example`](.env.example)).
- `CIRCLESEARCH_CONFIG_DIR` permite cambiar la carpeta de configuración.
- Al abrir una búsqueda en el navegador externo, se utiliza el navegador predeterminado de Windows.

---

## 📄 Licencia

Distribuido bajo la [Licencia MIT](LICENSE). Código abierto y gratuito para uso personal y comercial.
