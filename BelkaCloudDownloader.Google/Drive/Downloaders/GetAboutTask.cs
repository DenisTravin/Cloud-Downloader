namespace BelkaCloudDownloader.Google.Drive
{
    using System;
    using System.Threading.Tasks;
    using global::Google.Apis.Drive.v3;
    using global::Google.Apis.Drive.v3.Data;
    using Utils;

    /// <summary>
    /// Downloads information about user and account.
    /// </summary>
    internal sealed class GetAboutTask : ITask<DriveService, About>
    {
        /// <summary>
        /// Downloads information about user and account.
        /// </summary>
        /// <param name="service">Google Drive service used to download about info.</param>
        /// <param name="taskContext">Provides context for task execution, like logger object and
        ///     cancellation token.</param>
        /// <returns>Information about user and account.</returns>
        public async Task<About> Run(DriveService service, ITaskContext taskContext)
        {
            taskContext.Log.LogMessage("Downloading user account info...");
            try
            {
                var aboutRequest = service.About.Get();
                aboutRequest.Fields = "*";
                return await GoogleRequestHelper.Execute(aboutRequest.ExecuteAsync, taskContext.CancellationToken);
            }
            catch (Exception e)
            {
                if (taskContext.CancellationToken.IsCancellationRequested)
                {
                    throw;
                }

                taskContext.Fail("User account info download failed.", e);
                return null;
            }
        }
    }
}
