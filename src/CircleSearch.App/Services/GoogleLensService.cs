using System.Diagnostics;
using System.IO;
using System.Text;

namespace CircleSearch.Services;

public static class GoogleLensService
{
    public static string BuildAutoSubmitHtml(byte[] pngBytes)
    {
        var base64 = Convert.ToBase64String(pngBytes);
        var hl = Domain.ConfigManager.Current.UiLanguage;
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

    public static Task OpenInExternalBrowserAsync(byte[] pngBytes, string? currentUrl = null)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(currentUrl) &&
                currentUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                OpenUrl(currentUrl);
            }
            else
            {
                var htmlPath = WriteTempAutoSubmitHtmlFile(pngBytes);
                OpenUrl(htmlPath);
            }
        }
        catch (Exception ex)
        {
            Infrastructure.AppLogger.Error("Failed to launch external browser for Google Lens", ex);
        }

        return Task.CompletedTask;
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
