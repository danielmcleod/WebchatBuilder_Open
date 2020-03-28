using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Security.Principal;
using Microsoft.Web.Administration;
using Microsoft.Win32;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Forms;


namespace WebchatBuilderUpdater
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            if (!IsUserAdministrator())
            {
                tblResultMsg.Text = "You must run this updater as an Administrator.";
                Overlay.Visibility = Visibility.Visible;
            }
            else
            {
                tbPathSvc.Text = GetServicePath();
                tbPathWeb.Text = GetSitePath();
                RunFolderPathCheckWeb();
                RunFolderPathCheckSvc();
            }
        }

        bool IsUserAdministrator()
        {
            bool isAdmin;
            try
            {
                var user = WindowsIdentity.GetCurrent();
                var principal = new WindowsPrincipal(user);
                isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch (UnauthorizedAccessException ex)
            {
                isAdmin = false;
            }
            catch (Exception ex)
            {
                isAdmin = false;
            }
            return isAdmin;
        }

        String GetServicePath()
        {
            try
            {
                var keyPath = "HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\Webchat Builder Service";
                var val = (string)Registry.GetValue(keyPath, "ImagePath", "");
                return Path.GetDirectoryName(val.Trim('"'));
            }
            catch (Exception e)
            {
                LogMessage(e.Message);
            }

            return @"C:\Program Files (x86)\Qsect\Webchat Builder\WCB Service";
        }

        void LogMessage(string msg)
        {
            System.Windows.MessageBox.Show(msg);
        }

        string GetSitePath()
        {
            try
            {
                var mgr = new ServerManager();
                var site = mgr.Sites.FirstOrDefault(i => i.Name == "WebchatBuilder");
                if (site != null)
                {
                    var path = site.Applications[0].VirtualDirectories["/"].PhysicalPath;
                    return path.TrimEnd('\\');
                }
            }
            catch (Exception e)
            {
                LogMessage(e.Message);
            }

            return @"C:\Program Files (x86)\Qsect\Webchat Builder\WCB Web";
        }

        bool ProcessXcopy(string solutionDirectory, string targetDirectory)
        {
            try
            {
                var args = "\"" + solutionDirectory + "\" \"" + targetDirectory + "\"" + @" /e /s /y /i";
                var startInfo = new ProcessStartInfo
                {
                    CreateNoWindow = false,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    //RedirectStandardInput = true,
                    FileName = "XCOPY",
                    WindowStyle = ProcessWindowStyle.Normal,
                    Arguments = args
                };

                //var process = Process.Start(startInfo);
                //if (process != null) process.WaitForExit();
                using (var proc = Process.Start(startInfo))
                {
                    proc.OutputDataReceived += ((sender, e) => Console.WriteLine("Data: " + e.Data ?? String.Empty));
                    proc.ErrorDataReceived += (sender, e) => Console.WriteLine(string.Format("Error: {0}", e.Data ?? string.Empty));
                    proc.BeginErrorReadLine();
                    proc.BeginOutputReadLine();
                    proc.WaitForExit();
                }
            }
            catch (Exception e)
            {
                LogMessage(e.Message);
                tblResultMsg.Text = "Error copying new files over. Please manually copy over the new files for this update or contact support@qsect.com for help.";
                return false;
            }
            return true;
        }

        void RunFolderPathCheckWeb()
        {
            try
            {
                if (tbPathWeb != null)
                {
                    var path = tbPathWeb.Text;
                    if (Directory.Exists(path))
                    {
                        tblValidWeb.Text = "\uE10B;";
                        tblValidWeb.Foreground = new SolidColorBrush(Colors.LawnGreen);
                        ToggleUpdateBtn(true);
                    }
                    else
                    {
                        tblValidWeb.Text = "\uE10A;";
                        tblValidWeb.Foreground = new SolidColorBrush(Colors.Red);
                        ToggleUpdateBtn(false);
                    }
                }
            }
            catch (Exception e)
            {
                LogMessage(e.Message);
            }
        }

        void RunFolderPathCheckSvc()
        {
            try
            {
                if (tbPathSvc != null)
                {
                    var path = tbPathSvc.Text;
                    if (Directory.Exists(path))
                    {
                        tblValidSvc.Text = "\uE10B;";
                        tblValidSvc.Foreground = new SolidColorBrush(Colors.LawnGreen);
                        ToggleUpdateBtn(true);
                    }
                    else
                    {
                        tblValidSvc.Text = "\uE10A;";
                        tblValidSvc.Foreground = new SolidColorBrush(Colors.Red);
                        ToggleUpdateBtn(false);
                    }
                }
            }
            catch (Exception e)
            {
                LogMessage(e.Message);
            }
        }

        void ToggleUpdateBtn(bool isEnabled)
        {
            btnUpdate.IsEnabled = isEnabled;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void tbPathWeb_TextChanged(object sender, TextChangedEventArgs e)
        {
            RunFolderPathCheckWeb();
        }

        private void tbPathSvc_TextChanged(object sender, TextChangedEventArgs e)
        {
            RunFolderPathCheckSvc();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                btnBrowseWeb.IsEnabled = false;
                btnBrowseSvc.IsEnabled = false;
                btnUpdate.IsEnabled = false;
                Progress.Visibility = Visibility.Visible;
                var path = Assembly.GetExecutingAssembly().Location;
                path = Path.GetDirectoryName(path);
                var webPath = Path.Combine(path, "Files\\Wcb Web");
                var svcPath = Path.Combine(path, "Files\\Wcb Service");
                if (ProcessXcopy(webPath, tbPathWeb.Text))
                {
                    ProcessXcopy(svcPath, tbPathSvc.Text);
                }
            }
            catch (Exception ex)
            {
                LogMessage(ex.Message);
            }
            finally
            {
                Progress.Visibility = Visibility.Collapsed;
                Overlay.Visibility = Visibility.Visible;
            }
        }

        private void btnBrowseSvc_Click(object sender, RoutedEventArgs e)
        {
            var fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var folder = fbd.SelectedPath;
                tbPathSvc.Text = folder;
            }
        }

        private void btnBrowseWeb_Click(object sender, RoutedEventArgs e)
        {
            var fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var folder = fbd.SelectedPath;
                tbPathWeb.Text = folder;
            }
        }
    }
}
