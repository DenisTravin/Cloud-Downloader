namespace BelkaCloudDownloader.Google
{
    using System.Threading.Tasks;
    using global::Google.Apis.Util.Store;

    /// <summary>
    /// Implementation of Google IDataStore interace that does not actually store anything, so user shall
    /// re-authenticate every time. It is actually desired behavior in forensics when there are multiple accounts
    /// to download and common authentication scenario includes refresh token that does not need authentication.
    /// </summary>
    internal sealed class DummyDataStore : IDataStore
    {
        /// <summary>Does not store anything, so returns task that does nothing.</summary>
        /// <typeparam name="T">The type to store in the data store.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value to store.</param>
        /// <returns>An empty <see cref="Task"/> object.</returns>
        public Task StoreAsync<T>(string key, T value) => TaskEx.Delay(0);

        /// <summary>
        /// Does nothing since nothing was stored there.
        /// </summary>
        /// <typeparam name="T">The type to delete from the data store.</typeparam>
        /// <param name="key">The key to delete.</param>
        /// <returns>An empty <see cref="Task"/> object.</returns>
        public Task DeleteAsync<T>(string key) => TaskEx.Delay(0);

        /// <summary>Returns <c>null</c> for every key.</summary>
        /// <typeparam name="T">The type to retrieve from the data store.</typeparam>
        /// <param name="key">The key to retrieve its value.</param>
        /// <returns>Task which returns <c>null</c>.</returns>
        public Task<T> GetAsync<T>(string key) => TaskEx.FromResult(default(T));

        /// <summary>Does nothing since nothing was stored here.</summary>
        /// <returns>An empty <see cref="Task"/> object.</returns>
        public Task ClearAsync() => TaskEx.Delay(0);
    }
}
