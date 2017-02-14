namespace BelkaCloudDownloader.BoxDrive
{
    using System;
    using System.Threading.Tasks;
    using System.Threading;
    using System.Collections.Generic;
    using Utils;
    using System.IO;
    using Box.V2;
    using Box.V2.Config;
    using Box.V2.Auth;
    using Box.V2.Models;


    public class BoxDownloader : AbstractDownloader
    {
        private CancellationToken cancelToken;

        private List<BoxItemWithPath> result = new List<BoxItemWithPath>();

        public BoxDownloader(CancellationToken cancellationToken)
            : base(cancellationToken, 10)
        {
            cancelToken = cancellationToken;
        }

        public static async Task<string> Authenticate(CancellationToken cancellationTolen, IIoLogger log)
        {
            AuthenticationForm authentification = new AuthenticationForm();
            authentification.ShowDialog();
            log.LogMessage("Authorizing manually for Box as Box Sync");
            try
            {
                var redirectUri = new Uri(AppConstants.redirectUrl);
                var config = new BoxConfig(AppConstants.clientId, AppConstants.clientSecret, redirectUri);
                var session = new OAuthSession(authentification.ReturnToken(), "NOT_NEEDED", 3600, "bearer");
                BoxClient boxClient = new BoxClient(config, session);
                log.LogMessage("Successfully authenticated");
                return authentification.ReturnToken();
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

        private async Task GetFilesList(BoxClient boxClient, string folder, string extraPath)
        {
            result.Clear();
            var items = await boxClient.FoldersManager.GetFolderItemsAsync(folder, 500);
            foreach (var item in items.Entries)
            {
                cancelToken.ThrowIfCancellationRequested();
                if (item.Type == "folder")
                {
                    await GetFilesList(boxClient, item.Id, extraPath + '\\' + item.Name);
                }
                else
                {
                    var tempItem = await boxClient.FilesManager.GetInformationAsync(item.Id);
                    var tempPathItem = new BoxItemWithPath(tempItem, extraPath);
                    result.Add(tempPathItem);
                }
            }
        }

        public async Task<MetaInformation> Download(BoxClient boxClient, string newPath, string folder, IList<string> selectedCapabilities)
        {
            List<BoxItem> temp = new List<BoxItem>();
            await GetFilesList(boxClient, "0", "");
            foreach (var item in result)
            {
                cancelToken.ThrowIfCancellationRequested();
                temp.Add(item.BoxItem);
                if (selectedCapabilities.Contains("Files with meta-information"))
                {
                    string path = newPath;
                    if (item.ExtraPath != "")
                    {
                        path += item.ExtraPath;
                        Directory.CreateDirectory(path);
                    }
                    path += '\\' + item.BoxItem.Name;
                    using (var fileStream = System.IO.File.Create(path))
                    {
                        (await boxClient.FilesManager.DownloadStreamAsync(item.BoxItem.Id)).CopyTo(fileStream);
                    }
                }
            }
            var meta = new MetaInformation(temp);
            return meta;
        }
    }
}
