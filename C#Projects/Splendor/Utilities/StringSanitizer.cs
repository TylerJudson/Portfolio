using System.Text.RegularExpressions;

namespace Splendor.Utilities
{
    /// <summary>
    /// Utility class for sanitizing string inputs to prevent security vulnerabilities
    /// </summary>
    public static class StringSanitizer
    {
        private const int MaxPlayerNameLength = 50;
        private const int MaxImageNameLength = 200;

        /// <summary>
        /// Sanitizes an image name to prevent path traversal attacks
        /// </summary>
        /// <param name="imageName">The image name to sanitize</param>
        /// <returns>Sanitized image name, or null if invalid</returns>
        public static string? SanitizeImageName(string? imageName)
        {
            if (string.IsNullOrWhiteSpace(imageName))
            {
                return null;
            }

            // Trim whitespace
            imageName = imageName.Trim();

            // Check length
            if (imageName.Length > MaxImageNameLength)
            {
                return null;
            }

            // Prevent path traversal - reject if contains .. or / or \
            if (imageName.Contains("..") || imageName.Contains("/") || imageName.Contains("\\"))
            {
                return null;
            }

            // Only allow alphanumeric, hyphens, underscores, and dots
            // Valid examples: "Level1-Diamond-3P-Cost.png", "Noble-Ruby-Emerald.png"
            if (!Regex.IsMatch(imageName, @"^[a-zA-Z0-9\-_.]+$"))
            {
                return null;
            }

            // Must end with .png or .jpg
            if (!imageName.EndsWith(".png", StringComparison.OrdinalIgnoreCase) &&
                !imageName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) &&
                !imageName.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            return imageName;
        }

        /// <summary>
        /// Sanitizes a player name to prevent XSS and enforce length limits
        /// </summary>
        /// <param name="playerName">The player name to sanitize</param>
        /// <returns>Sanitized player name, or null if invalid</returns>
        public static string? SanitizePlayerName(string? playerName)
        {
            if (string.IsNullOrWhiteSpace(playerName))
            {
                return null;
            }

            // Trim whitespace
            playerName = playerName.Trim();

            // Check length
            if (playerName.Length == 0 || playerName.Length > MaxPlayerNameLength)
            {
                return null;
            }

            // Remove HTML/script tags to prevent XSS
            playerName = Regex.Replace(playerName, @"<[^>]*>", string.Empty);

            // Remove any remaining angle brackets
            playerName = playerName.Replace("<", "").Replace(">", "");

            // Remove control characters
            playerName = Regex.Replace(playerName, @"[\x00-\x1F\x7F]", string.Empty);

            // Ensure we still have content after sanitization
            if (string.IsNullOrWhiteSpace(playerName))
            {
                return null;
            }

            return playerName;
        }
    }
}
