namespace FloraMix.Services
{
    public static class Constants
    {
        public const string DatabaseFilename = "FloraMix.db3";

        public static string DatabasePath =>
            Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);
    }
}