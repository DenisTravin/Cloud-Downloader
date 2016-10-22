namespace BelkaCloudDownloader.Google
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using global::Google.Apis.Auth.OAuth2;
    using Utils;

    /// <summary>
    /// Helper class that provides consent screen authentication for Google services.
    /// </summary>
    internal static class ManualAuthenticator
    {
        /// <summary>
        /// Authenticates a client using browser consent form, may require login and password for Google account.
        /// </summary>
        /// <param name="cancellationToken">Token that allows to cancel authentication task.</param>
        /// <param name="secrets">A set of client secrets to use when authenticating.</param>
        /// <param name="scopes">A set of capabilities to request from Google services.</param>
        /// <param name="log">Logger that accepts debug info.</param>
        /// <param name="dataStoreName">Name of a file data store to use to store credentials.</param>
        /// <returns>Refresh token.</returns>
        /// <exception cref="OperationCanceledException">Operation was cancelled by user.</exception>
        public static async Task<string> AuthenticateManually(
            CancellationToken cancellationToken,
            ClientSecrets secrets,
            IEnumerable<string> scopes,
            IIoLogger log,
            string dataStoreName)
        {
            try
            {
                var dataStore = new DummyDataStore();
                var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                        secrets,
                        scopes,
                        "user",
                        cancellationToken,
                        dataStore);

                log.LogMessage("Successfully authenticated");
                return credential.Token.RefreshToken;
            }
            catch (Exception e)
            {
                if (e is OperationCanceledException)
                {
                    throw (OperationCanceledException)e;
                }

                log.LogError("Authentication error", e);
                throw;
            }
        }
    }
}
