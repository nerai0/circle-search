<div align="center">

<img src="assets/icon.png" width="112" alt="CircleSearch" />

# 🔍 CircleSearch para Windows
### A experiência nativa e rápida de Circle to Search no Windows 10 e 11

[![Release](https://img.shields.io/github/v/release/nerai0/circle-search?style=flat-square&color=4169E1)](https://github.com/nerai0/circle-search/releases)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg?style=flat-square)](LICENSE)
[![Platform](https://img.shields.io/badge/Platform-Windows%2010%20%7C%2011%20(x64)-0078D6?style=flat-square&logo=windows)](https://github.com/nerai0/circle-search)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)

[**English**](README.md) | [**Қазақша**](README.kk.md) | [**Русский**](README.ru.md) | [**Español**](README.es.md) | [**Deutsch**](README.de.md) | [**Français**](README.fr.md) | [**Português**](README.pt.md) | [**Türkçe**](README.tr.md) | [**Українська**](README.uk.md) | [**中文**](README.zh.md) | [**日本語**](README.ja.md) | [**한국어**](README.ko.md)

</div>

---

**CircleSearch** traz o famoso gesto **Circle to Search** (Circule para Pesquisar) dos smartphones direto para o seu computador. Segure a tecla **`Win`** (ou pressione **`Alt + Espaço`**) em qualquer ponto da tela para congelar a imagem, selecione uma área e obtenha pesquisas visuais instantâneas com Google Lens, reconhecimento óptico de caracteres (OCR) e traduções em um painel lateral elegante — **100% gratuito, sem chaves de API nem assinaturas**.

---

## ✨ Recursos

- ⚡ **Ativação ao segurar a tecla `Win` (400 ms):** Hook de teclado Win32 de baixo nível (`WH_KEYBOARD_LL`) detecta toques longos e impede a abertura acidental do menu Iniciar. Toques rápidos e atalhos normais (`Win+R`, `Win+D`, `Win+V`) continuam funcionando com resposta instantânea.
- 🎯 **Seleção interativa e redimensionável:** Selecione qualquer parte da tela. Use os 8 controles de ajuste ou arraste toda a caixa de seleção — os resultados são atualizados em tempo real.
- 🌐 **Navegador Google Lens integrado:** Desenvolvido com `Microsoft.Web.WebView2`. Exibe resultados visuais, texto selecionável e traduções instantâneas diretamente no painel lateral.
- 🖥️ **Pronto para múltiplos monitores e DPI misto:** Compatível com `PerMonitorV2`. Funciona com perfeição em telas com escalas variadas (`100%`, `125%`, `150%`, `200%`, `2K`, `4K`).
- 🖱️ **Monitor de destino inteligente:** Abre automaticamente no monitor sob o ponteiro do mouse (padrão) ou na tela onde está a janela ativa em primeiro plano.
- 🚀 **Captura de janela ativa ou tela inteira com um clique:** Botões rápidos na barra inferior para pesquisar diretamente a aplicação atual ou todo o desktop.
- 🌍 **Detecção automática de idioma e 12 idiomas inclusos:** Adapta-se ao idioma do Windows e oferece seleção manual nas Configurações (Inglês, Cazaque, Russo, Espanhol, Alemão, Francês, Português, Turco, Ucraniano, Chinês, Japonês e Coreano).
- 🎨 **Design minimalista e temas:** Visual moderno no estilo Material, temas claro e escuro, destaques em Azul Real e início automático com o Windows.

---

## 📥 Download e Instalação

> **Observação:** O CircleSearch é extremamente leve (~650 KB) e requer o [.NET 10 Desktop Runtime (x64)](https://dotnet.microsoft.com/download/dotnet/10.0). Caso ainda não esteja instalado, o Windows oferecerá a instalação em um clique no primeiro uso.

### Opção 1: Instalador do Windows (`.msi`) — *Recomendado*
Baixe a versão mais recente **`CircleSearch-v<versão>-win-x64.msi`** na aba [**Releases**](https://github.com/nerai0/circle-search/releases).
- Instalação por usuário (não requer direitos de administrador).
- Cria atalhos no Menu Iniciar e na Área de Trabalho com ícone de alta resolução.

### Opção 2: Arquivo portátil (`.zip`)
Baixe **`CircleSearch-v<versão>-win-x64-portable.zip`**, descompacte e execute `CircleSearch.exe`.

---

## ⌨️ Atalhos e Comandos

| Ação | Atalho |
| :--- | :--- |
| **Chamar sobreposição** | Segurar a tecla `Win` por 400 ms *ou* `Alt + Espaço` |
| **Selecionar área** | Clicar com o botão esquerdo e arrastar |
| **Redimensionar** | Arrastar qualquer um dos 8 marcadores |
| **Mover seleção** | Clicar e arrastar dentro da moldura |
| **Expandir painel lateral** | Botão *Mais largo / Mais estreito* no topo |
| **Fechar** | Tecla `Esc` ou botão `X` no canto superior |

---

## 📄 Licença

Distribuído sob a [Licença MIT](LICENSE). Código aberto e livre para uso pessoal e comercial.
