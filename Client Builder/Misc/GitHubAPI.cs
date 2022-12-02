using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_Builder.Misc
{
    internal class GitHubAPI
    {
        public string url { get; set; }
        public string assets_url { get; set; }
        public string upload_url { get; set; }
        public string html_url { get; set; }
        public string tag_name { get; set; }
        public string body { get; set; }
    }
}
