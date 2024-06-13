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
        static public string? Text_received_msg
            => ResourceManager.GetString("Text_received_msg");
        static public string? Error_msg
            => ResourceManager.GetString("Error_msg");
        static public string? Download_succeed_msg
            => ResourceManager.GetString("Download_succeed_msg");
        static public string? Upload_succeed_msg
            => ResourceManager.GetString("Upload_succeed_msg");
        static public string? Get_folder_succeed_msg
            => ResourceManager.GetString("Get_folder_succeed_msg");
        static public string? Getting_folder_content_msg
            => ResourceManager.GetString("Getting_folder_content_msg");
        static public string? Sent_clipboard_text_msg
            => ResourceManager.GetString("Sent_clipboard_text_msg");
        static public string? Send_file_progress
            => ResourceManager.GetString("Send_file_progress");
        static public string? Receive_file_progress
            => ResourceManager.GetString("Receive_file_progress");
        static public string? Request_upload
            => ResourceManager.GetString("Request_upload");
        static public string? Request_download
            => ResourceManager.GetString("Request_download");
        static public string? Received_File
            => ResourceManager.GetString("Received_File");
        static public string? tsbOpenReceiveFolder_tooltip
            => ResourceManager.GetString("tsbOpenReceiveFolder_tooltip");

    }
}
