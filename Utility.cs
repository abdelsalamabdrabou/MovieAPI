namespace MovieAPI
{
    public static class Utility
    {
        private static readonly List<string> allowedExtensions = new() { ".jpg", "png" };
        private static readonly long maxAllowedPosterSize = 1048576;

        public static bool IsFileExtensionAllowed(IFormFile file)
        {
            bool isAllowed = allowedExtensions.Contains(Path.GetExtension(file.FileName).ToLower());
            return isAllowed;
        }

        public static bool IsFileSizeAllowed(IFormFile file)
        {
            bool isAllowed = file.Length <= maxAllowedPosterSize;
            return isAllowed;
        }
        public static async Task<byte[]> ConvertFileToByteArray(IFormFile file)
        {
            using var dataStream = new MemoryStream();
            await file.CopyToAsync(dataStream);

            return dataStream.ToArray();
        }
    }
}
