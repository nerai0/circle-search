<div align="center">

<img src="assets/icon.png" width="112" alt="CircleSearch" />

# 🔍 Windows İçin CircleSearch
### Windows 10 ve 11 İçin Hızlı ve Yerel Circle to Search Deneyimi

[![Release](https://img.shields.io/github/v/release/nerai0/circle-search?style=flat-square&color=4169E1)](https://github.com/nerai0/circle-search/releases)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg?style=flat-square)](LICENSE)
[![Platform](https://img.shields.io/badge/Platform-Windows%2010%20%7C%2011%20(x64)-0078D6?style=flat-square&logo=windows)](https://github.com/nerai0/circle-search)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)

[**English**](README.md) | [**Қазақша**](README.kk.md) | [**Русский**](README.ru.md) | [**Español**](README.es.md) | [**Deutsch**](README.de.md) | [**Français**](README.fr.md) | [**Português**](README.pt.md) | [**Türkçe**](README.tr.md) | [**Українська**](README.uk.md) | [**中文**](README.zh.md) | [**日本語**](README.ja.md) | [**한국어**](README.ko.md)

</div>

---

**CircleSearch**, akıllı telefonlardaki popüler **Circle to Search** (Aramak İçin Daireye Al) hareketini doğrudan bilgisayarınıza getirir. Ekranın herhangi bir yerinde **`Win`** tuşunu basılı tutun (veya **`Alt + Boşluk`** tuşlarına basın); ekran anında donar, dilediğiniz alanı seçerek yan panelde anında Google Lens görsel araması, metin tanıma (OCR) ve çeviri elde edin — **tamamen ücretsiz, API anahtarı veya abonelik gerekmez**.

---

## ✨ Özellikler

- ⚡ **`Win` Tuşuna Basılı Tutarak Tetikleme (400 ms):** Düşük seviyeli Win32 klavye kancası (`WH_KEYBOARD_LL`) uzun basmayı algılar ve Başlat menüsünün açılmasını engeller. Normal tıklamalar ve sistem kısayolları (`Win+R`, `Win+D`, `Win+V`) gecikmesiz çalışır.
- 🎯 **Etkileşimli ve Boyutlandırılabilir Seçim:** Ekranın istediğiniz kısmını seçin. 8 tutamacı sürükleyerek boyutu değiştirin veya seçimi taşıyın — sonuçlar canlı olarak yenilenir.
- 🌐 **Dahili Google Lens Tarayıcısı:** `Microsoft.Web.WebView2` ile geliştirilmiştir. Görsel eşleşmeleri, kopyalanabilir metinleri ve çevirileri doğrudan yan panelde sunar.
- 🖥️ **Çoklu Monitör ve Karma DPI Desteği:** `PerMonitorV2` uyumludur. Farklı ölçekleme oranlarına (`%100`, `%125`, `%150`, `%200`, `2K`, `4K`) sahip ekranlarda kusursuz çalışır.
- 🖱️ **Akıllı Hedef Monitör:** Varsayılan olarak fare imlecinin bulunduğu ekranda veya etkin pencerenin bulunduğu monitörde açılır.
- 🚀 **Tek Tıkla Etkin Pencere / Tam Ekran:** Alt kısımdaki hızlı butonlarla o anki uygulamayı veya tüm ekranı anında tarayın.
- 🌍 **Sistem Dilini Otomatik Algılama & 12 Yerleşik Dil:** Windows sistem diliyle otomatik eşleşir; Ayarlar üzerinden istenen dil seçilebilir (İngilizce, Kazakça, Rusça, İspanyolca, Almanca, Fransızca, Portekizce, Türkçe, Ukraynaca, Çince, Japonca ve Korece).
- 🎨 **Minimalist Tasarım ve Temalar:** Sade Material tarzı, koyu ve açık tema, Royal Blue vurguları ve Windows ile otomatik başlama seçeneği.

---

## 📥 İndirme ve Kurulum

> **Not:** CircleSearch son derece hafiftir (~650 KB) ve [.NET 10 Desktop Runtime (x64)](https://dotnet.microsoft.com/download/dotnet/10.0) gerektirir. Sisteminizde yüklü değilse, ilk çalıştırmada Windows tek tıkla yükleme önerecektir.

### Seçenek 1: Windows Yükleyicisi (`.msi`) — *Önerilen*
[**Releases**](https://github.com/nerai0/circle-search/releases) sayfasından en son **`CircleSearch-v<sürüm>-win-x64.msi`** dosyasını indirin.
- Kullanıcı bazlı yükleme (yönetici yetkisi gerektirmez).
- Başlat Menüsü ve Masaüstüne yüksek çözünürlüklü simgeye sahip kısayollar ekler.

### Seçenek 2: Taşınabilir Arşiv (`.zip`)
**`CircleSearch-v<sürüm>-win-x64-portable.zip`** dosyasını indirin, klasöre çıkartın ve `CircleSearch.exe` dosyasını çalıştırın.

---

## 📄 Lisans

[MIT Lisansı](LICENSE) kapsamında dağıtılmaktadır. Kişisel ve ticari kullanım için tamamen açık kaynak ve ücretsizdir.
