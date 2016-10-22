#pragma warning disable SA1302 // Interface names must begin with I
// Justification: iCloud is one of the clouds, ICloudPlugin can be perceived as related to iCloud.

namespace BelkaCloudDownloader.Gui
{
    using System.Collections.Generic;

    /// <summary>
    /// GUI "plugin" that provides interface between generic GUI and a downloader library.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    internal interface CloudPlugin
    {
        /// <summary>
        /// Gets name of a cloud.
        /// </summary>
        string CloudName { get; }

        /// <summary>
        /// Gets a list of protocols supported by this plugin. Protocol is one method of authentication and pieces
        /// of data that can be downloaded by that method of authentication. For example, Google requires different
        /// permissions for Drive and Plus, so we actually use two sets of credentials for those applications,
        /// so it is two protocols.
        /// </summary>
        IList<Protocol> Protocols { get; }
    }
}

#pragma warning restore SA1302 // Interface names must begin with I
