namespace AdvertisingPlatforms
{
    public class Initialization
    {
        public static void Init()
        {
            if (File.Exists(FileHandler.path))
                PlatformHandler.FillPlatform(FileHandler.path);
        }
    }
}
