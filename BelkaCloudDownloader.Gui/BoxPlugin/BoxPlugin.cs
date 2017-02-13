namespace BelkaCloudDownloader.Gui
{
    using System.Collections.Generic;

    /// <summary>
    /// Box support.
    /// </summary>
    internal sealed class BoxPlugin : CloudPlugin
    {
        public IList<Protocol> Protocols { get; } = new List<Protocol> { new BoxProtocol() };

        public string CloudName { get; } = "Box";
    }
}