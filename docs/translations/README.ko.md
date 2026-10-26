<div align="center">

<img src="../../assets/icon.png" width="112" alt="CircleSearch" />

# 🔍 CircleSearch for Windows
### Windows 10 & 11을 위한 빠르고 직관적인 Circle to Search

[![Release](https://img.shields.io/github/v/release/nerai0/circle-search?style=flat-square&color=4169E1)](https://github.com/nerai0/circle-search/releases)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg?style=flat-square)](../../LICENSE)
[![Platform](https://img.shields.io/badge/Platform-Windows%2010%20%7C%2011%20(x64)-0078D6?style=flat-square&logo=windows)](https://github.com/nerai0/circle-search)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)

[**English**](../../README.md) | [**Қазақша**](README.kk.md) | [**Русский**](README.ru.md) | [**Español**](README.es.md) | [**Deutsch**](README.de.md) | [**Français**](README.fr.md) | [**Português**](README.pt.md) | [**Türkçe**](README.tr.md) | [**Українська**](README.uk.md) | [**中文**](README.zh.md) | [**日本語**](README.ja.md) | [**한국어**](README.ko.md)

</div>

---

**CircleSearch**는 스마트폰의 인기 기능인 **Circle to Search(동그라미로 검색)** 제스처를 PC로 가져옵니다. 화면 어디서나 **`Win`** 키를 길게 누르거나(또는 **`Alt + Space`** 입력), 화면을 즉시 멈추고 영역을 지정하여 Google 렌즈 시각 검색, 텍스트 추출(OCR) 및 번역을 사이드 패널에서 바로 확인하세요 — **100% 무료, API 키 및 구독 불필요**.

---

## ✨ 주요 기능

- ⚡ **`Win` 키 길게 누르기 트리거 (400 ms):** 저수준 Win32 키보드 후크(`WH_KEYBOARD_LL`)를 통해 시작 메뉴 팝업을 방지하며 자연스럽게 동작합니다. 단일 탭 및 Windows 단축키(`Win+R`, `Win+D`, `Win+V`)는 평소대로 즉각 반응합니다.
- 🎯 **대화형 크기 조절 선택 상자:** 화면의 원하는 부분을 자유롭게 선택하세요. 8개의 조절 핸들로 크기를 바꾸거나 상자 전체를 이동할 수 있으며 결과가 실시간으로 갱신됩니다.
- 🌐 **내장 Google 렌즈 브라우저:** `Microsoft.Web.WebView2` 기반. 사이드 패널에서 검색 결과, 복사 가능한 텍스트, 실시간 번역을 제공합니다.
- 🖥️ **멀티 모니터 및 혼합 DPI 지원:** `PerMonitorV2` 완벽 지원. 다양한 배율(`100%`, `125%`, `150%`, `200%`, `2K`, `4K`) 환경에서도 선명하게 작동합니다.
- 🖱️ **스마트 대상 모니터:** 마우스 커서가 있는 모니터(기본값) 또는 활성 창이 있는 모니터에서 실행됩니다.
- 🚀 **원클릭 활성 창 / 전체 화면 캡처:** 하단 전용 버튼으로 현재 작업 중인 창이나 전체 화면을 즉시 검색합니다.
- 🌍 **시스템 언어 자동 감지 & 12개 내장 언어:** Windows 표시 언어에 자동 맞춤되며, 설정에서 원하는 언어로 수동 변경할 수 있습니다(영어, 카자흐어, 러시아어, 스페인어, 독일어, 프랑스어, 포르투갈어, 튀르키예어, 우크라이나어, 중국어, 일본어, 한국어).
- 🎨 **미니멀한 디자인과 테마:** Material 스타일, 다크/라이트 테마, 로열 블루 포인트 컬러, Windows 시작 시 자동 실행 지원.

---

## 📥 다운로드 및 설치

> **안내:** CircleSearch는 초경량(~650 KB)이며 실행을 위해 [.NET 10 Desktop Runtime (x64)](https://dotnet.microsoft.com/download/dotnet/10.0)이 필요합니다. 미설치 상태인 경우 첫 실행 시 Windows에서 1클릭 설치 창을 안내합니다.

### 옵션 1: Windows 설치 프로그램 (`.msi`) — *권장*
[**Releases**](https://github.com/nerai0/circle-search/releases)에서 최신 **`CircleSearch-v<버전>-win-x64.msi`**를 다운로드하세요.
- 사용자별 설치(관리자 권한 불필요).
- 시작 메뉴 및 바탕 화면에 고해상도 아이콘 바로 가기를 자동 등록합니다.

### 옵션 2: 포터블 압축 파일 (`.zip`)
**`CircleSearch-v<버전>-win-x64-portable.zip`**을 다운로드하고 원하는 폴더에 압축을 풀어 `CircleSearch.exe`를 실행하세요.

---

## 📄 라이선스

[MIT License](../../LICENSE)에 따라 배포됩니다. 개인 및 상업적 목적으로 완전히 무료로 사용할 수 있습니다.
