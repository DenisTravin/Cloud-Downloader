namespace BelkaCloudDownloader.Gui
{
    using System.Collections.Generic;

    /// <summary>
    /// Google Cloud support.
    /// </summary>
    internal sealed class GoogleCloudPlugin : CloudPlugin
    {
        public IList<Protocol> Protocols { get; } = new List<Protocol> { new GoogleDriveProtocol() };

        public string CloudName { get; } = "Google Cloud";
    }
}
