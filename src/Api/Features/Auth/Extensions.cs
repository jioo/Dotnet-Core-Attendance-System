using System;
using WebApi.Entities;

namespace WebApi.Features.Auth
{
    public static class Extensions
    {
        /// <summary>
        /// Date converted to seconds since Unix epoch (Jan 1, 1970, midnight UTC).
        /// </summary>
        public static long ToUnixEpochDate(DateTime date) => 
            (long) Math.Round((date.ToUniversalTime() -
                new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
                .TotalSeconds);

        /// <summary>
        /// Model validation based on JwtIssuerOptions
        /// </summary>
        public static void ThrowIfInvalidOptions(JwtIssuerOptions options)
        {
            if (options == null) 
                throw new ArgumentNullException(nameof(options));

            if (options.ValidFor <= TimeSpan.Zero)
                throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(JwtIssuerOptions.ValidFor));

            if (options.SigningCredentials == null)
                throw new ArgumentNullException(nameof(JwtIssuerOptions.SigningCredentials));

            if (options.JtiGenerator == null)
                throw new ArgumentNullException(nameof(JwtIssuerOptions.JtiGenerator));
        }
    }
}