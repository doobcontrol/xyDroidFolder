using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace xyDroidFolder
{
    public class R
    {
        static ResourceManager resourceManager;
        static ResourceManager ResourceManager
        {
            get {
                if(resourceManager == null)
                {
                    resourceManager
                        = new ResourceManager("xyDroidFolder.Properties.Resources",
                            typeof(Program).Assembly);
                }
                return resourceManager;
            }
        }
        static public void setCulture(string Culture)
        {
            // Set the culture to English
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo(Culture);

            resourceManager
            = new ResourceManager("xyDroidFolder.Properties.Resources",
                typeof(Program).Assembly);
        }

        static public string? AppName 
            => ResourceManager.GetString("AppName");
        static public string? App_Exit 
            => ResourceManager.GetString("App_Exit");
        static public string? ScanTitle 
            => ResourceManager.GetString("ScanTitle");
        static public string? InitFolderNode
            => ResourceManager.GetString("InitFolderNode");
        static public string? FileBtn_Download
            => ResourceManager.GetString("FileBtn_Download");
        static public string? FileBtn_Upload
            => ResourceManager.GetString("FileBtn_Upload");
        static public string? tsbRefreshCurrentNode_tooltip
            => ResourceManager.GetString("tsbRefreshCurrentNode_tooltip");
        static public string? tsbClipboardWatch_tooltip_notWatch
            => ResourceManager.GetString("tsbClipboardWatch_tooltip_notWatch");
        static public string? tsbClipboardWatch_tooltip_inWatch
            => ResourceManager.GetString("tsbClipboardWatch_tooltip_inWatch");
    }
}
