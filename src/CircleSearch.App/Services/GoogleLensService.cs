using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
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

        var loadingText = LocalizationService.T("overlay.lens_loading");

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
  <form id="lensForm" method="POST" action="https://lens.google.com/upload?ep=ccm&s=&hl={{hl}}" enctype="multipart/form-data">
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
        window.location.replace("https://www.google.com/?olud");
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

    public static async Task OpenInExternalBrowserAsync(byte[] imageBytes)
    {
        try
        {
            var targetUrl = await UploadViaChromeLensEndpointAsync(imageBytes).ConfigureAwait(false);
            if (!string.IsNullOrWhiteSpace(targetUrl))
            {
                Infrastructure.AppLogger.Info("Opening Google Lens URL in external browser", new { Url = targetUrl });
                OpenBrowserUrl(targetUrl);
                return;
            }
        }
        catch (Exception ex)
        {
            Infrastructure.AppLogger.Warning($"Direct Lens ccm upload failed: {ex.Message}");
        }

        var jpegBytes = ConvertToOptimizedJpeg(imageBytes, 88L);
        var fallbackHtml = WriteTempAutoSubmitHtmlFile(jpegBytes, isJpeg: true);
        OpenBrowserUrl(fallbackHtml);
    }

    public static async Task<string?> UploadViaChromeLensEndpointAsync(byte[] imageBytes)
    {
        var hl = Domain.ConfigManager.Current.UiLanguage;
        if (string.IsNullOrWhiteSpace(hl) || hl.Equals("auto", StringComparison.OrdinalIgnoreCase))
        {
            hl = LocalizationService.CurrentLanguage;
        }

        var uploadUri = $"https://lens.google.com/upload?ep=ccm&s=&hl={hl}";

        using var request = new HttpRequestMessage(HttpMethod.Post, uploadUri);
        request.Headers.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36");

        var jpegBytes = ConvertToOptimizedJpeg(imageBytes, 88L);

        using var content = new MultipartFormDataContent("----WebKitFormBoundaryCircleSearch");
        var imageContent = new ByteArrayContent(jpegBytes);
        imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
        content.Add(imageContent, "encoded_image", "image.jpg");

        request.Content = content;

        using var response = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

        if (response.Headers.Location != null)
        {
            return response.Headers.Location.ToString();
        }

        return null;
    }

    public static void OpenBrowserUrl(string urlOrPath)
    {
        // 1. Попытка открыть в Google Chrome напрямую (если установлен), так как Google Lens родной для Chrome
        try
        {
            var chromeKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\chrome.exe");
            var chromePath = chromeKey?.GetValue("") as string;
            if (!string.IsNullOrWhiteSpace(chromePath) && File.Exists(chromePath))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = chromePath,
                    Arguments = $"\"{urlOrPath}\"",
                    UseShellExecute = false
                });
                return;
            }
        }
        catch (Exception ex)
        {
            Infrastructure.AppLogger.Warning($"Failed to start Chrome directly: {ex.Message}");
        }

        // 2. Системный браузер по умолчанию через ShellExecute
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
            Infrastructure.AppLogger.Error("Failed to open default browser", ex);
        }
    }
}
