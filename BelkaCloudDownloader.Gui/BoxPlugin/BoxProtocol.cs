namespace BelkaCloudDownloader.Gui
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using Box.V2;
    using Box.V2.Auth;
    using Box.V2.Config;
    using Box.V2.Models;
    using BoxDrive;
    using Utils;

    /// <summary>
    /// Protocol for downloading Box information and documents.
    /// </summary>
    internal sealed class BoxProtocol : Protocol
    {
        public override IList<string> AuthenticationMethods { get; } = new List<string>
        {
            "Box consent screen"
        };

        public override IList<string> Capabilities { get; } = new List<string>
        {
            "Meta-information about files",
            "Files with meta-information"
        };

        public override Control AuthenticationControl(string authenticationMethod)
        {
            switch (authenticationMethod)
            {
                case "Box consent screen":
                    {
                        return new ConsentScreenControl(
                           token => BoxDownloader.Authenticate(token, this.Log));
                    }
            }

            Debug.Assert(false, "Unknown authentication method passed to protocol");
            return null;
        }

        public override async Task<object> Execute(string targetDirectory, Control authenticationControl, IList<string> selectedCapabilities, CancellationToken cancellationToken)
        {
            var downloader = new BoxDownloader(cancellationToken);
            this.AddDownloader(downloader);

            var consentScreenControl = authenticationControl as ConsentScreenControl;
            if (consentScreenControl == null)
            {
                throw new InternalErrorException("Incorrect authentication control for Box protocol.");
            }

            Uri redirectUri = new Uri(AppConstants.redirectUrl);
            var config = new BoxConfig(AppConstants.clientId, AppConstants.clientSecret, redirectUri);
            var session = new OAuthSession(consentScreenControl.RefreshToken, "NOT_NEEDED", 3600, "bearer");
            BoxClient boxClient = new BoxClient(config, session);

            return await downloader.Download(boxClient, targetDirectory, "0", selectedCapabilities);
        }
    }
}
