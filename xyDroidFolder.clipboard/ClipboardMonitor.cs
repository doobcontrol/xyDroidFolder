using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace xyDroidFolder.clipboard
{
    public class ClipboardMonitor
    {
        IntPtr nextClipboardViewer;
        public event EventHandler ClipboardMsgHandler;

        private string _clipboardString;
        public string ClipboardString { get { return _clipboardString; } }

        /// <summary>
        /// handle WindowsSystem.Windows.Forms.Message
        /// </summary>
        /// <param name="m">WindowsSystem.Windows.Forms.Message</param>
        public bool WndProc(ref Message m)
        {
            // defined in winuser.h
            const int WM_DRAWCLIPBOARD = 0x308;
            const int WM_CHANGECBCHAIN = 0x030D;

            bool msgDone = false;

            switch (m.Msg)
            {
                case WM_DRAWCLIPBOARD:
                    DisplayClipboardData();
                    SendMessage(nextClipboardViewer, m.Msg, m.WParam, m.LParam);
                    msgDone = true;
                    break;
                case WM_CHANGECBCHAIN:
                    if (m.WParam == nextClipboardViewer)
                        nextClipboardViewer = m.LParam;
                    else
                        SendMessage(nextClipboardViewer, m.Msg, m.WParam, m.LParam);
                    msgDone = true;
                    break;
                default:
                    //base.WndProc(ref m);
                    break;
            }
            return msgDone;
        }
        /// <summary>
        /// show content of clipboard
        /// </summary>
        public void DisplayClipboardData()
        {
            try
            {
                IDataObject iData = new DataObject();
                iData = Clipboard.GetDataObject();

                //only text
                if (iData.GetDataPresent(DataFormats.Text))
                {
                    _clipboardString = (string)iData.GetData(DataFormats.Text);
                    ClipboardMsgHandler(this, null);
                }
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.ToString());
            }
        }

        public void changeMonitorStatus(bool inClipboardMonitor, IntPtr Handle)
        {
            if (inClipboardMonitor)
            {
                startMonitor(Handle);
            }
            else
            {
                stopMonitor(Handle);
            }
        }
        /// <summary>
        /// stop monitor and remove from watch chain
        /// </summary>
        public void stopMonitor(IntPtr Handle)
        {
            ChangeClipboardChain(Handle, nextClipboardViewer);
        }
        /// <summary>
        /// stop monitor and add to watch chain
        /// </summary>
        public void startMonitor(IntPtr Handle)
        {
            nextClipboardViewer = (IntPtr)SetClipboardViewer((int)Handle);
        }

        #region WindowsAPI
        /// <summary>
        /// Adds the specified window to the chain of clipboard viewers. 
        /// </summary>
        /// <param name="hWndNewViewer">
        ///     A handle to the window to be added to the clipboard chain.</param>
        /// <returns>If the function succeeds, the return value identifies the next 
        ///     window in the clipboard viewer chain. If an error occurs or there are no 
        ///     other windows in the clipboard viewer chain, the return value is NULL. 
        ///     To get extended error information, call GetLastError.</returns>
        [DllImport("User32.dll")]
        protected static extern int SetClipboardViewer(int hWndNewViewer);

        /// <summary>
        /// Removes a specified window from the chain of clipboard viewers.
        /// </summary>
        /// <param name="hWndRemove">A handle to the window to be removed from the chain. 
        ///     The handle must have been passed to the SetClipboardViewer function.</param>
        /// <param name="hWndNewNext">A handle to the window that follows the hWndRemove 
        ///     window in the clipboard viewer chain. (This is the handle returned by 
        ///     SetClipboardViewer, unless the sequence was changed in response to a 
        ///     WM_CHANGECBCHAIN message.)</param>
        /// <returns>The return value indicates the result of passing the 
        ///     WM_CHANGECBCHAIN message to the windows in the clipboard viewer chain. 
        ///     Because a window in the chain typically returns FALSE when it processes 
        ///     WM_CHANGECBCHAIN, the return value from ChangeClipboardChain is typically 
        ///     FALSE. If there is only one window in the chain, the return value is 
        ///     typically TRUE.</returns>
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool ChangeClipboardChain(IntPtr hWndRemove, IntPtr hWndNewNext);

        /// <summary>
        /// Sends the specified message to a window or windows. 
        /// </summary>
        /// <param name="hwnd">A handle to the window whose window procedure will receive 
        ///     the message. </param>
        /// <param name="wMsg">The message to be sent.</param>
        /// <param name="wParam">Additional message-specific information.</param>
        /// <param name="lParam">Additional message-specific information.</param>
        /// <returns>The return value specifies the result of the message processing; it 
        ///     depends on the message sent.</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);
        #endregion
    }
}
