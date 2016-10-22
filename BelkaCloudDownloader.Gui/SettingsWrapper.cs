namespace BelkaCloudDownloader.Gui
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Xml;

    /// <summary>
    /// Provides automatic serialization and deserialization of downloader settings into/from XML file.
    /// </summary>
    internal sealed class SettingsWrapper
    {
        /// <summary>
        /// File name of a file where to store downloader settings.
        /// </summary>
        private const string SettingsFileName = "DownloaderSettings.xml";

        /// <summary>
        /// Underlying settings holder object.
        /// </summary>
        private DownloaderSettings settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsWrapper"/> class.
        /// </summary>
        public SettingsWrapper()
        {
            Action initDefault = () =>
                this.settings = new DownloaderSettings
                {
                    AuthenticationMethod = new Dictionary<string, string>(),
                    Capabilities = new Dictionary<string, IDictionary<string, ISet<string>>>(),
                    ProtocolSettings = new Dictionary<string, object>()
                };

            if (!File.Exists(SettingsWrapper.SettingsFileName))
            {
                initDefault();
                return;
            }

            var serializer = new DataContractSerializer(typeof(DownloaderSettings));

            try
            {
                var fs = new FileStream(SettingsWrapper.SettingsFileName, FileMode.Open);
                using (var reader = new StreamReader(fs))
                {
                    using (var xmlReader = new XmlTextReader(reader))
                    {
                        this.settings = (DownloaderSettings)serializer.ReadObject(xmlReader);
                    }
                }
            }
            catch (Exception)
            {
                initDefault();
            }
        }

        /// <summary>
        /// Gets or sets selected cloud name. Serializes settings when set.
        /// </summary>
        public string CloudName
        {
            get
            {
                return this.settings.CloudName;
            }

            set
            {
                this.settings.CloudName = value;
                this.Serialize();
            }
        }

        /// <summary>
        /// Gets selected authentication method for currently selected cloud.
        /// </summary>
        /// <returns>Authentication method for given cloud.</returns>
        public string GetAuthenticationMethod() => this.settings.AuthenticationMethod.ContainsKey(this.CloudName)
                ? this.settings.AuthenticationMethod[this.CloudName]
                : null;

        /// <summary>
        /// Sets selected authentication method for currently selected cloud and serializes current settings.
        /// </summary>
        /// <param name="authenticationMethod">Selected authentication method.</param>
        public void SetAuthenticationMethod(string authenticationMethod)
        {
            this.settings.AuthenticationMethod[this.CloudName] = authenticationMethod;
            this.Serialize();
        }

        /// <summary>
        /// Returns a set of selected capabilities for current cloud and given authentication method.
        /// </summary>
        /// <param name="authenticationMethod">Selected authentication method.</param>
        /// <returns>A set of capabilities from settings.</returns>
        public ISet<string> GetCapabilities(string authenticationMethod)
        {
            if (!this.settings.Capabilities.ContainsKey(this.CloudName))
            {
                this.settings.Capabilities.Add(this.CloudName, new Dictionary<string, ISet<string>>());
            }

            if (!this.settings.Capabilities[this.CloudName].ContainsKey(authenticationMethod))
            {
                this.settings.Capabilities[this.CloudName].Add(authenticationMethod, new HashSet<string>());
            }

            return this.settings.Capabilities[this.CloudName][authenticationMethod];
        }

        /// <summary>
        /// Sets or unsets the capability as selected in a settings file.
        /// </summary>
        /// <param name="authenticationMethod">Selected authentication method.</param>
        /// <param name="capability">Selected capability.</param>
        /// <param name="isSet">true, if capability is selected.</param>
        public void SetCapability(string authenticationMethod, string capability, bool isSet)
        {
            if (!this.settings.Capabilities.ContainsKey(this.CloudName))
            {
                this.settings.Capabilities.Add(this.CloudName, new Dictionary<string, ISet<string>>());
            }

            if (!this.settings.Capabilities[this.CloudName].ContainsKey(authenticationMethod))
            {
                this.settings.Capabilities[this.CloudName].Add(authenticationMethod, new HashSet<string>());
            }

            if (isSet)
            {
                this.settings.Capabilities[this.CloudName][authenticationMethod].Add(capability);
            }
            else
            {
                this.settings.Capabilities[this.CloudName][authenticationMethod].Remove(capability);
            }

            this.Serialize();
        }

        public object GetProtocolSettings(string protocol)
        {
            if (this.settings.ProtocolSettings.ContainsKey(protocol))
            {
                return this.settings.ProtocolSettings[protocol];
            }

            return null;
        }

        public void SetProtocolSettings(string protocol, object settings)
        {
            if (this.settings.ProtocolSettings.ContainsKey(protocol))
            {
                this.settings.ProtocolSettings[protocol] = settings;
            }
            else
            {
                this.settings.ProtocolSettings.Add(protocol, settings);
            }

            this.Serialize();
        }

        /// <summary>
        /// Serializes current settings into XML file.
        /// </summary>
        private void Serialize()
        {
            var serializer = new DataContractSerializer(typeof(DownloaderSettings));
            using (var writer = new StreamWriter(SettingsWrapper.SettingsFileName))
            {
                using (var xmlWriter = new XmlTextWriter(writer))
                {
                    xmlWriter.Formatting = Formatting.Indented;
                    serializer.WriteObject(xmlWriter, this.settings);
                    xmlWriter.Flush();
                }
            }
        }
    }
}
