using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vestris.ResourceLib;

namespace Client_Builder.Builder
{
    internal static class IconInjector
    {
        internal static void SetIcon(string iconPath, string exePath)
        {
            try
            {
                IconFile iconFile = new IconFile(iconPath);
                IconDirectoryResource iconDirectoryResource = new IconDirectoryResource(iconFile);
                iconDirectoryResource.SaveTo(exePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
