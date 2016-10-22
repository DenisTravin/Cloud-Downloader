namespace BelkaCloudDownloader.Gui
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Serializable storage of selected cloud, authorization scheme and downloader settings.
    /// </summary>
    [DataContract]
    [KnownType(typeof(HashSet<string>))]
    [KnownType(typeof(Google.Drive.DriveDownloaderSettings))]
    public sealed class DownloaderSettings
    {
        /// <summary>
        /// Gets or sets selected cloud name.
        /// </summary>
        [DataMember]
        public string CloudName { get; set; }

        /// <summary>
        /// Gets or sets selected authentication method for each cloud.
        /// </summary>
        [DataMember]
        public IDictionary<string, string> AuthenticationMethod { get; set; }

        /// <summary>
        /// Gets or sets selected capabilities for a cloud and authentication method.
        /// </summary>
        [DataMember]
        public IDictionary<string, IDictionary<string, ISet<string>>> Capabilities { get; set; }

        /// <summary>
        /// Gets or sets settings for selected protocols.
        /// </summary>
        [DataMember]
        public IDictionary<string, object> ProtocolSettings { get; set; }
    }
}
