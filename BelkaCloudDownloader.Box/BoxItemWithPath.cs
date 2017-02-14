namespace BelkaCloudDownloader.BoxDrive
{
    using Box.V2.Models;

    /// <summary>
    /// class for box item what saves item's extra path
    /// </summary>
    public class BoxItemWithPath
    {
        public BoxItem BoxItem { get; set; }
        public string ExtraPath { get; set; }

        public BoxItemWithPath(BoxItem boxItem, string extraPath)
        {
            BoxItem = boxItem;
            ExtraPath = extraPath;
        }
    }
}
