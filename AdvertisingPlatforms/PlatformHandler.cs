namespace AdvertisingPlatforms
{
    public class PlatformHandler
    {
        private static Dictionary<string, List<string>> platformLocations = null!;

        /// <summary>
        /// Заполнение рекламных площадок и их локаций из файла.
        /// </summary>
        public static void FillPlatform(string filePath)
        {
            platformLocations = [];

            foreach (var line in File.ReadAllLines(filePath))
            {
                var parts = line.Split(':');
                if (parts.Length != 2) continue;

                var platform = parts[0].Trim();
                var locations = parts[1].Split(',')
                    .Select(l => l.Trim())
                    .Where(l => !string.IsNullOrEmpty(l))
                    .ToList();

                platformLocations[platform] = locations;
            }
        }

        /// <summary>
        /// Поиск рекламных площадок для заданной локации.
        /// </summary>
        public static List<string> FindPlatformsForLocation(string targetLocation)
        {
            if (string.IsNullOrEmpty(targetLocation))
                return [];

            if (platformLocations.Count == 0)
                return [];

            var result = new List<string>();

            foreach (var platform in platformLocations)
            {
                foreach (var platformLocation in platform.Value)
                {
                    // Проверяем, что целевая локация вложена в локацию площадки.
                    if (IsLocationCovered(targetLocation, platformLocation))
                    {
                        result.Add(platform.Key);
                        break;
                    }
                }
            }
            return [.. result.OrderBy(p => p)];
        }

        /// <summary>
        /// Проверка локации.
        /// </summary>
        private static bool IsLocationCovered(string targetLocation, string platformLocation)
        {
            // Локация площадки должна быть префиксом целевой локации.
            return targetLocation.StartsWith(platformLocation + "/") ||
                   targetLocation == platformLocation;
        }
    }
}
