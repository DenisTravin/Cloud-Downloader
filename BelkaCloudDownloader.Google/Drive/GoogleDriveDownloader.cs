namespace BelkaCloudDownloader.Google.Drive
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using global::Google.Apis.Auth.OAuth2;
    using global::Google.Apis.Auth.OAuth2.Flows;
    using global::Google.Apis.Auth.OAuth2.Responses;
    using global::Google.Apis.Drive.v3;
    using global::Google.Apis.Drive.v3.Data;
    using global::Google.Apis.Services;
    using Utils;

    /// <summary>
    /// Main class that downloads files and meta-information from Google Drive.
    /// It uses Google Drive Sync client id and secret to obtain access token by given refresh token.
    /// Downloading process can be cancelled by cancellationToken and reports overall progress
    /// (by ProgressChanged event) in a range of [0..100].
    /// It also reports progress for a currently downloaded file, current status and when meta-information is ready.
    /// </summary>
    public sealed class GoogleDriveDownloader : AbstractDownloader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GoogleDriveDownloader"/> class.
        /// </summary>
        /// <param name="cancellationToken">Token by which downloading progress can be cancelled.</param>
        public GoogleDriveDownloader(CancellationToken cancellationToken)
            : base(cancellationToken, 10)
        {
        }

        /// <summary>
        /// Authenticates a client using browser consent form, may require login and password for Google account.
        /// </summary>
        /// <param name="cancellationToken">Token that allows to cancel authentication task.</param>
        /// <param name="log">Logger object used to output debug info.</param>
        /// <returns>Refresh token.</returns>
        /// <exception cref="OperationCanceledException">Operation was cancelled by user.</exception>
        public static async Task<string> AuthenticateManually(CancellationToken cancellationToken, IIoLogger log)
        {
            log.LogMessage("Authorizing manually for Google Drive as Google Drive Sync");
            string[] scopes = { DriveService.Scope.DriveReadonly };
            var secrets = new ClientSecrets
                              {
                                  ClientId = Constants.GoogleDriveSyncClientId,
                                  ClientSecret = Constants.GoogleDriveSyncClientSecret
                              };

            return await ManualAuthenticator.AuthenticateManually(
                cancellationToken,
                secrets,
                scopes,
                log,
                "GoogleDrive_Datastore");
        }

        /// <summary>
        /// Downloads meta-information and files from Google Drive.
        /// </summary>
        /// <param name="settings">Object with parameters of a downloader.</param>
        /// <returns>Object with downloaded meta-information.</returns>
        /// <exception cref="OperationCanceledException">The token has had cancellation requested.</exception>
        /// <exception cref="ObjectDisposedException">The associated
        /// <see cref="T:System.Threading.CancellationTokenSource" /> has been disposed.</exception>
        public async Task<MetaInformation> Download(DriveDownloaderSettings settings)
        {
            this.Log.LogMessage($"Started Google Drive downloading, Refresh Token = {settings.RefreshToken},"
                    + $" Target Directory = {settings.TargetDirectory}.");
            this.Status = Status.Authenticating;

            this.Log.LogMessage("Authenticating...");

            try
            {
                using (var service = await this.CreateService(settings.RefreshToken))
                {
                    this.Log.LogMessage("Authentication successful");

                    var metaInformationDownloader = new MetaInformationDownloader(this.CancellationToken, service);
                    var filesDownloader = new FilesDownloader(
                        service,
                        settings.TargetDirectory,
                        this.CancellationToken);

                    this.AddSubDownloader(metaInformationDownloader);
                    this.AddSubDownloader(filesDownloader);

                    this.Status = Status.DownloadingMetaInformation;
                    this.CancellationToken.ThrowIfCancellationRequested();

                    this.Log.LogMessage("Downloading meta-information");

                    var metaInformation = await metaInformationDownloader.DownloadMetaInformation();

                    this.EmitPreliminaryResultsReady(metaInformation);

                    if (settings.DoDownloadFiles)
                    {
                        this.Status = Status.DownloadingFiles;
                        this.CancellationToken.ThrowIfCancellationRequested();

                        this.Log.LogMessage("Downloading files...");
                        metaInformation.DownloadedFiles = await filesDownloader.DownloadFiles(
                            metaInformation.Files,
                            metaInformation.About.ExportFormats);
                    }
                    else
                    {
                        metaInformation.DownloadedFiles = new List<DownloadedFile>();
                    }

                    this.Done();

                    return metaInformation;
                }
            }
            catch (TokenResponseException)
            {
                this.Error();
                return new MetaInformation(null, new List<File>());
            }
        }

        /// <summary>
        /// Creates Google Drive service object with correct credentials.
        /// </summary>
        /// <param name="refreshToken">Refresh Token to use.</param>
        /// <returns>Google Drive service object.</returns>
        private async Task<DriveService> CreateService(string refreshToken)
        {
            var initializer =
                new AuthorizationCodeFlow.Initializer(
                    "https://accounts.google.com/o/oauth2/auth",
                    "https://accounts.google.com/o/oauth2/token")
                    {
                        ClientSecrets =
                            new ClientSecrets
                                {
                                    ClientId = Constants.GoogleDriveSyncClientId,
                                    ClientSecret = Constants.GoogleDriveSyncClientSecret
                                }
                    };

            // Hardcoded Google Drive Sync secret.
            string[] scopes = { DriveService.Scope.DriveReadonly };
            initializer.Scopes = scopes;

            using (var codeFlow = new AuthorizationCodeFlow(initializer))
            {
                try
                {
                    var token = await GoogleRequestHelper.Execute(
                        ct => codeFlow.RefreshTokenAsync("user", refreshToken, ct),
                        this.CancellationToken);

                    var credential = new UserCredential(codeFlow, "user", token);

                    var service =
                        new DriveService(
                            new BaseClientService.Initializer
                                {
                                    HttpClientInitializer = credential,
                                    ApplicationName = Utils.Constants.ApplicationName
                                });

                    return service;
                }
                catch (Exception e)
                {
                    if (this.CancellationToken.IsCancellationRequested)
                    {
                        this.Log.LogMessage("Operation was cancelled.");
                        throw;
                    }

                    this.StatusAccumulator.FailureOccurred();
                    this.Log.LogError("Authorization error.", e);
                    throw;
                }
            }
        }
    }
}
