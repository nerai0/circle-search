using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace CircleSearch.Services;

public static class GoogleLensService
{
    public const string UploadEndpoint = "https://lens.google.com/upload?ep=ccm&s=&hl=";
    public const string FallbackUrl = "https://www.google.com/";

    public static byte[] ConvertToOptimizedJpeg(byte[] pngBytes, long quality = 90L)
    {
        try
        {
            using var msIn = new MemoryStream(pngBytes);
            using var bmp = new Bitmap(msIn);
            using var msOut = new MemoryStream();

            var jpegEncoder = ImageCodecInfo.GetImageEncoders()
                .FirstOrDefault(c => c.FormatID == ImageFormat.Jpeg.Guid);

            if (jpegEncoder != null)
            {
                using var encoderParams = new EncoderParameters(1);
                encoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
                bmp.Save(msOut, jpegEncoder, encoderParams);
                return msOut.ToArray();
            }

            bmp.Save(msOut, ImageFormat.Jpeg);
            return msOut.ToArray();
        }
        catch
        {
            return pngBytes;
        }
    }

    /// <summary>Google ожидает региональный код для китайского: zh -> zh-CN (упрощённый), остальные коды совпадают.</summary>
    public static string ToGoogleLanguageCode(string appLanguage) =>
        appLanguage.Equals("zh", StringComparison.OrdinalIgnoreCase) ? "zh-CN" : appLanguage;

    public static string BuildAutoSubmitHtml(byte[] imageBytes, bool isJpeg = false)
    {
        var base64 = Convert.ToBase64String(imageBytes);
        var mimeType = isJpeg ? "image/jpeg" : "image/png";
        var fileName = isJpeg ? "selection.jpg" : "selection.png";

        var hl = Domain.ConfigManager.Current.UiLanguage;
        if (string.IsNullOrWhiteSpace(hl) || hl.Equals("auto", StringComparison.OrdinalIgnoreCase))
        {
            hl = LocalizationService.CurrentLanguage;
        }
        hl = ToGoogleLanguageCode(hl);

        var loadingText = LocalizationService.T("overlay.lens_loading");

        // Прямой POST на официальный эндпоинт загрузки Google Lens из контекста самого браузера!
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
      font-family: 'Segoe UI', system-ui, -apple-system, sans-serif;
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
  <form id="lensForm" method="POST" action="{{UploadEndpoint}}{{hl}}" enctype="multipart/form-data">
    <input type="file" id="lensFile" name="encoded_image" style="display:none;" />
  </form>
  <script>
    (function() {
      try {
        const b64 = "{{base64}}";
        const bin = atob(b64);
        const len = bin.length;
        const bytes = new Uint8Array(len);
        for (let i = 0; i < len; i++) {
          bytes[i] = bin.charCodeAt(i);
        }
        const blob = new Blob([bytes], { type: "{{mimeType}}" });
        const file = new File([blob], "{{fileName}}", { type: "{{mimeType}}" });

        const dt = new DataTransfer();
        dt.items.add(file);
        const input = document.getElementById("lensFile");
        input.files = dt.files;

        document.getElementById("lensForm").submit();
      } catch (e) {
        window.location.replace("{{FallbackUrl}}");
      }
    })();
  </script>
</body>
</html>
""";
    }

    public static string WriteTempAutoSubmitHtmlFile(byte[] imageBytes, bool isJpeg = false)
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
        File.WriteAllText(htmlPath, BuildAutoSubmitHtml(imageBytes, isJpeg), Encoding.UTF8);
        return htmlPath;
    }

    public static Task OpenInExternalBrowserAsync(byte[] imageBytes)
    {
        // ВАЖНО: Google Lens связывает сессию поиска исключительно с тем браузером,
        // который загрузил изображение (его cookies, сессия аккаунта, IP, TLS-отпечаток).
        // Поэтому загрузку ДОЛЖЕН выполнять сам браузер пользователя через форму автоотправки,
        // а не фоновый процесс HttpClient приложения!
        var jpegBytes = ConvertToOptimizedJpeg(imageBytes, 88L);
        var uploadHtmlPath = WriteTempAutoSubmitHtmlFile(jpegBytes, isJpeg: true);

        OpenBrowserFileOrUrl(uploadHtmlPath);
        return Task.CompletedTask;
    }

    public static void OpenBrowserFileOrUrl(string filePathOrUrl)
    {
        // 1. Запускаем браузер, выбранный пользователем в Windows по умолчанию (любой: Chrome, Edge, Firefox, Brave...)
        try
        {
            var browserExe = Infrastructure.DefaultBrowserResolver.ResolveExecutablePath();
            if (browserExe != null)
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = browserExe,
                    Arguments = $"\"{filePathOrUrl}\"",
                    UseShellExecute = false
                });
                return;
            }
        }
        catch (Exception ex)
        {
            Infrastructure.AppLogger.Warning("Failed to start default browser directly", new { ex.Message });
        }

        // 2. Если браузер определить не удалось — отдаём файл/URL системе (ShellExecute)
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = filePathOrUrl,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            Infrastructure.AppLogger.Error("Failed to open default browser", ex);
        }
    }
}
