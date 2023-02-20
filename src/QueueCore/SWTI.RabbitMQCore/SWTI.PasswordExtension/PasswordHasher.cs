using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace SWTI.RabbitMQCore.SWTI.PasswordExtension
{
    /// <summary>
    /// https://juldhais.net/secure-way-to-store-passwords-in-database-using-sha256-asp-net-core-898128d1c4ef
    /// </summary>
    public static class PasswordHasher
    {
        /// <summary>
        /// Computes the hash.
        /// var passwordSaltPepper = $”{password}{salt}{pepper}”;
        /// </summary>
        /// <param name="password">The password.</param>
        /// <param name="salt">The salt.</param>
        /// <param name="pepper">The pepper.</param>
        /// <param name="iteration">The iteration.</param>
        /// <returns></returns>
        public static async Task<string> ComputeHash(string password, string salt, string pepper, int iteration = 3, CancellationToken cancellationToken = default)
        {
            if (iteration <= 0) return password;

            using var sha256 = SHA256.Create();
            var passwordSaltPepper = $"{password}{salt}{pepper}";
            using var stringStream = await passwordSaltPepper.ToStream();
            var byteHash = await sha256.ComputeHashAsync(stringStream);
            var hash = Convert.ToBase64String(byteHash);
            return await ComputeHash(hash, salt, pepper, iteration - 1, cancellationToken);
        }

        public static async Task<bool> Verify(string passwordRequest, string passwordHash, string salt, string pepper, int iteration = 3, CancellationToken cancellationToken = default)
        {
            var passwordHashCompare = await ComputeHash(passwordRequest, salt, pepper, iteration);
            return passwordHashCompare == passwordHash;
        }

        private static async Task<Stream> ToStream(this string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            await writer.WriteAsync(s);
            await writer.FlushAsync();
            stream.Position = 0;
            return stream;
        }
    }
}