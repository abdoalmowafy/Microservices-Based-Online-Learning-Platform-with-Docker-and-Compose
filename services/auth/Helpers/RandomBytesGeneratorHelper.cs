using System.Security.Cryptography;

namespace auth.Helpers
{
    public static class RandomBytesGeneratorHelper
    {
        public static string GenerateRandomBytes(int length)
        {
            if (length <= 0)
                throw new ArgumentOutOfRangeException(nameof(length), "Length must be a positive integer.");
            
            var randomBytes = new Byte[length];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            
            return Convert.ToBase64String(randomBytes);
        }
    }
}
