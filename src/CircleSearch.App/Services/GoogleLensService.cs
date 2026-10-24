using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace CircleSearch.Services;

public static class GoogleLensService
{
    private static readonly HttpClient HttpClient = new(new HttpClientHandler
    {
        AllowAutoRedirect = false
    })
    {
        Timeout = TimeSpan.FromSeconds(15)
    };

    public static string BuildAutoSubmitHtml(byte[] pngBytes)
    {
        var base64 = Convert.ToBase64String(pngBytes);
        var hl = Domain.ConfigManager.Current.UiLanguage;
        if (string.IsNullOrWhiteSpace(hl) || hl.Equals("auto", StringComparison.OrdinalIgnoreCase))
        {
            hl = LocalizationService.CurrentLanguage;
        }

        var loadingText = LocalizationService.T("overlay.lens_loading");
        var ts = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        return $$"""
<!DOCTYPE html>
<html lang="{{hl}}">
<head>
  <meta charset="utf-8" />
  <title>Google Lens...</title>
  <style>
    body {
      margin: 0;
      height: 100vh;
      display: flex;
      align-items: center;
      justify-content: center;
      background: #16181D;
      color: #F1F3F4;
      font-family: 'Segoe UI', system-ui, sans-serif;
      font-size: 14px;
    }
    .box {
      display: flex;
      align-items: center;
      gap: 12px;
      background: #20232A;
      padding: 14px 20px;
      border-radius: 14px;
    }
    .spinner {
      width: 18px;
      height: 18px;
      border: 2.5px solid #4169E1;
      border-top-color: transparent;
      border-radius: 50%;
      animation: spin 0.7s linear infinite;
    }
    @keyframes spin { to { transform: rotate(360deg); } }
  </style>
</head>
<body>
  <div class="box">
    <div class="spinner"></div>
    <span>{{loadingText}}</span>
  </div>
  <script>
    (async function() {
      try {
        const b64 = "{{base64}}";
        const bin = atob(b64);
        const bytes = new Uint8Array(bin.length);
        for (let i = 0; i < bin.length; i++) {
          bytes[i] = bin.charCodeAt(i);
        }
        const blob = new Blob([bytes], { type: "image/png" });
        const file = new File([blob], "selection.png", { type: "image/png" });

        const form = document.createElement("form");
        form.method = "POST";
        form.enctype = "multipart/form-data";
        form.action = "https://lens.google.com/v3/upload?hl={{hl}}&re=df&st={{ts}}";
        form.style.display = "none";

        const input = document.createElement("input");
        input.type = "file";
        input.name = "encoded_image";

        const dt = new DataTransfer();
        dt.items.add(file);
        input.files = dt.files;

        form.appendChild(input);
        document.body.appendChild(form);
        form.submit();
      } catch (e) {
        window.location.replace("https://www.google.com/?olud");
      }
    })();
  </script>
</body>
</html>
""";
    }

    public static string WriteTempAutoSubmitHtmlFile(byte[] pngBytes)
    {
        var tempDir = Path.Combine(Path.GetTempPath(), "CircleSearch");
        Directory.CreateDirectory(tempDir);

        try
        {
            foreach (var oldFile in Directory.GetFiles(tempDir, "lens_upload_*.html"))
            {
                if (File.GetCreationTimeUtc(oldFile) < DateTime.UtcNow.AddMinutes(-10))
                    File.Delete(oldFile);
            }
        }
        catch { }

        var htmlPath = Path.Combine(tempDir, $"lens_upload_{Guid.NewGuid():N}.html");
        File.WriteAllText(htmlPath, BuildAutoSubmitHtml(pngBytes), Encoding.UTF8);
        return htmlPath;
    }

    public static async Task OpenInExternalBrowserAsync(byte[] pngBytes)
    {
        try
        {
            // Отправляем POST multipart/form-data напрямую в Google Lens и получаем свежий 303 Redirect URL.
            // Это гарантирует, что сессия поиска создается в момент открытия внешнего браузера и не протухает!
            var targetUrl = await UploadImageAndGetSearchUrlAsync(pngBytes).ConfigureAwait(false);
            if (!string.IsNullOrWhiteSpace(targetUrl))
            {
                OpenUrl(targetUrl);
                return;
            }
        }
        catch (Exception ex)
        {
            Infrastructure.AppLogger.Warning($"Direct upload failed, falling back to html launcher: {ex.Message}");
        }

        // Запасной вариант через локальный auto-submit html в браузере пользователя
        var fallbackHtml = WriteTempAutoSubmitHtmlFile(pngBytes);
        OpenUrl(fallbackHtml);
    }

    public static async Task<string?> UploadImageAndGetSearchUrlAsync(byte[] pngBytes)
    {
        var hl = Domain.ConfigManager.Current.UiLanguage;
        if (string.IsNullOrWhiteSpace(hl) || hl.Equals("auto", StringComparison.OrdinalIgnoreCase))
        {
            hl = LocalizationService.CurrentLanguage;
        }

        var ts = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var uploadUri = $"https://lens.google.com/v3/upload?hl={hl}&re=df&st={ts}";

        using var request = new HttpRequestMessage(HttpMethod.Post, uploadUri);
        request.Headers.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36");

        using var content = new MultipartFormDataContent();
        var imageContent = new ByteArrayContent(pngBytes);
        imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/png");
        content.Add(imageContent, "encoded_image", "selection.png");

        request.Content = content;

        using var response = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

        // Google Lens возвращает HTTP 303 See Other со свежим search URL в заголовке Location
        if (response.Headers.Location != null)
        {
            return response.Headers.Location.ToString();
        }

        return null;
    }

    public static void OpenUrl(string urlOrPath)
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = urlOrPath,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            Infrastructure.AppLogger.Error("Failed to open browser URL", ex);
        }
    }
}
