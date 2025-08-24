namespace AdvertisingPlatforms
{
    public class FileHandler
    {
        public const string path = "Files/Platforms.txt";

        private static class UploadSettings
        {
            public static int MaxFileSize = 50 * 1024 * 1024; // 50MB
            public static string[] PermittedExtensions = [".txt"];
        }

        public static async Task<IResult> Upload(HttpRequest request)
        {
            if (!request.HasFormContentType)
                return Results.Problem(detail: "Invalid content type", statusCode: 404);

            var form = await request.ReadFormAsync();
            var file = form.Files["uploads"];

            // Валидация
            if (file == null)
                return Results.BadRequest("File is required");

            if (file.Length == 0)
                return Results.BadRequest("File is empty");

            if (file.Length > UploadSettings.MaxFileSize)
                return Results.BadRequest($"File size exceeds {UploadSettings.MaxFileSize / 1024 / 1024}MB limit");

            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(fileExtension) || !UploadSettings.PermittedExtensions.Contains(fileExtension))
                return Results.BadRequest($"Invalid file type. Allowed: {string.Join(", ", UploadSettings.PermittedExtensions)}");
            
            try
            {
                using var sr = new StreamReader(file.OpenReadStream());
                var content = await sr.ReadToEndAsync();
                WriteFile(content);
                PlatformHandler.FillPlatform(path);
            }
            catch (Exception ex)
            {
                return Results.Problem(detail: $"Ошибка при загрузке файла: {ex.Message}", statusCode: 404);
            }
            return Results.RedirectToRoute("index");
        }

        private static async void WriteFile(string? content)
        {
            using StreamWriter writer = new(path, false);
            await writer.WriteLineAsync(content);
        }
    }
}
