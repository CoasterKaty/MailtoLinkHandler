using System;
using System.Linq;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using System.IO;
using System.Diagnostics;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace MailtoHandler
{

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                if (args.Length > 0)
                {

                    string currentUserDomain = Regex.Split(System.DirectoryServices.AccountManagement.UserPrincipal.Current.UserPrincipalName, "@")[1];
                    string arg = String.Join(" ", args);

                    string mailTo = Regex.Split(((string)arg), "mailto:")[1];
                    string mailSubject = "";
                    string mailBody = "";

                    if (mailTo.Contains('?'))
                    {
                        mailTo = mailTo.Replace('?', '&');
                    }
                    if (mailTo.Contains('&'))
                    {
                        string[] mailArgs = mailTo.Split('&');
                        mailTo = mailArgs[0];
                        foreach (string mailArg in mailArgs)
                        {
                            if (mailArg.Contains('='))
                            {
                                string[] item = mailArg.Split(new Char[] { '=' }, 2);
                                switch (item[0].ToLower())
                                {
                                    case "subject":
                                        mailSubject = item[1];
                                        break;
                                    case "body":
                                        mailBody = item[1];
                                        break;
                                }
                            }
                        }
                    }
                    

                    
                    Settings kSettings = new Settings(true, "mailtohandler.xml", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                    if (kSettings.Get("OpenInOutlook", "notconfig") == "notconfig" || kSettings.Get("DontAsk", "no") == "no")
                    {
                        frmPick fPick = new frmPick();

                        Settings kAppSettings = new Settings(true);
                        fPick.rad365.Text = kAppSettings.Get("365_Radio", "Outlook 365");
                        fPick.rad2016.Text = kAppSettings.Get("Client_Radio", "Outlook Client");
                        fPick.label365.Text = kAppSettings.Get("365_Text", "For most people, or if you are not sure, select this option");
                        fPick.labelClient.Text = kAppSettings.Get("Client_Text", "For some people in fixed offices or with multiple mailboxes");
                        fPick.labelTitle.Text = kAppSettings.Get("Main_Text", "Please select how you wish to open mail links");
                        fPick.chkDontAsk.Text = kAppSettings.Get("Remember", "Don't ask again");
                        fPick.Text = kAppSettings.Get("Title", "Open Mail Link");

                        switch (kSettings.Get("OpenInOutlook", "notconfig"))
                        {
                            case "yes":
                                fPick.rad365.Checked = false;
                                fPick.rad2016.Checked = true;
                                break;
                            case "no":
                                fPick.rad365.Checked = true;
                                fPick.rad2016.Checked = false;
                                break;

                        }
                        fPick.chkDontAsk.Checked = (kSettings.Get("DontAsk", "yes") == "yes");
                        DialogResult dRes = fPick.ShowDialog();
                        switch (dRes)
                        {
                            case DialogResult.Yes:
                                kSettings.Set("OpenInOutlook", "no");
                                kSettings.Set("DontAsk", "yes");
                                break;
                            case DialogResult.OK:
                                kSettings.Set("OpenInOutlook", "no");
                                kSettings.Set("DontAsk", "no");
                                break;
                            case DialogResult.No:
                                kSettings.Set("OpenInOutlook", "yes");
                                kSettings.Set("DontAsk", "yes");
                                break;
                            case DialogResult.Cancel:
                                kSettings.Set("OpenInOutlook", "yes");
                                kSettings.Set("DontAsk", "no");
                                break;
                        }
                    }
                    if (kSettings.Get("OpenInOutlook", "notconfig") == "no")
                    {
                        string mailtoURL = "https://outlook.office.com/owa/?realm=" + currentUserDomain + "&path=/mail/action/compose&to=" + mailTo + "&subject=" + mailSubject + "&body=" + mailBody;
                        OpenInDefaultBrowser(mailtoURL);
                    }
                    else
                    {
                        SendOutlookMail(mailTo, mailSubject, mailBody);
                    }
                    kSettings.saveXML();

                }
            }
            catch
            {
            }
            
        }

        public static Boolean SendOutlookMail(string recipient, string subject, string body)
        {
            try
            {
                Outlook.Application outlookApp = new Outlook.Application();
                Outlook._MailItem oMailItem = (Outlook._MailItem)outlookApp.CreateItem(Outlook.OlItemType.olMailItem);
                Outlook.Inspector oInspector = oMailItem.GetInspector;

                Outlook.Recipients oRecips = (Outlook.Recipients)oMailItem.Recipients;
                Outlook.Recipient oRecip = (Outlook.Recipient)oRecips.Add(recipient);
                oRecip.Resolve();

                if (subject != "")
                {
                    oMailItem.Subject = subject;
                }
                if (body != "")
                {
                    oMailItem.Body = body;
                }
                oMailItem.Display(true);
                return true;
            }
            catch (Exception objEx)
            {
                MessageBox.Show(objEx.ToString());
                return false;
            }
        }



        /// <summary>
        ///     Opens a local file or url in the default web browser.
        ///     Can be used both for opening urls, or html readme docs.
        ///     Credit: https://stackoverflow.com/questions/13621467/how-to-find-default-web-browser-using-c
        /// </summary>
        /// <param name="pathOrUrl">Path of the local file or url</param>
        /// <returns>False if the default browser could not be opened.</returns>
        public static Boolean OpenInDefaultBrowser(String pathOrUrl)
        {
            // Trim any surrounding quotes and spaces.
            pathOrUrl = pathOrUrl.Trim().Trim('"').Trim();
            // Default protocol to "http"
            String protocol = Uri.UriSchemeHttp;
            // Correct the protocol to that in the actual url
            if (Regex.IsMatch(pathOrUrl, "^[a-z]+" + Regex.Escape(Uri.SchemeDelimiter), RegexOptions.IgnoreCase))
            {
                Int32 schemeEnd = pathOrUrl.IndexOf(Uri.SchemeDelimiter, StringComparison.Ordinal);
                if (schemeEnd > -1)
                    protocol = pathOrUrl.Substring(0, schemeEnd).ToLowerInvariant();
            }
            // Surround with quotes
            //   pathOrUrl = "\"" + pathOrUrl + "\"";
            Object fetchedVal;
            String defBrowser = null;
            // Look up user choice translation of protocol to program id
            using (RegistryKey userDefBrowserKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\Shell\Associations\UrlAssociations\" + protocol + @"\UserChoice"))
                if (userDefBrowserKey != null && (fetchedVal = userDefBrowserKey.GetValue("Progid")) != null)
                    // Programs are looked up the same way as protocols in the later code, so we just overwrite the protocol variable.
                    protocol = fetchedVal as String;
            // Look up protocol (or programId from UserChoice) in the registry, in priority order.
            // Current User registry
            using (RegistryKey defBrowserKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Classes\" + protocol + @"\shell\open\command"))
                if (defBrowserKey != null && (fetchedVal = defBrowserKey.GetValue(null)) != null)
                    defBrowser = fetchedVal as String;
            // Local Machine registry
            if (defBrowser == null)
                using (RegistryKey defBrowserKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Classes\" + protocol + @"\shell\open\command"))
                    if (defBrowserKey != null && (fetchedVal = defBrowserKey.GetValue(null)) != null)
                        defBrowser = fetchedVal as String;
            // Root registry
            if (defBrowser == null)
                using (RegistryKey defBrowserKey = Registry.ClassesRoot.OpenSubKey(protocol + @"\shell\open\command"))
                    if (defBrowserKey != null && (fetchedVal = defBrowserKey.GetValue(null)) != null)
                        defBrowser = fetchedVal as String;
            // Nothing found. Return.
            if (String.IsNullOrEmpty(defBrowser))
                return false;
            String defBrowserProcess;
            // Parse browser parameters. This code first assembles the full command line, and then splits it into the program and its parameters.
            Boolean hasArg = false;
            if (defBrowser.Contains("%1"))
            {
                // If url in the command line is surrounded by quotes, ignore those; our url already has quotes.
                if (defBrowser.Contains("\"%1\""))
                    defBrowser = defBrowser.Replace("\"%1\"", pathOrUrl);
                else
                    defBrowser = defBrowser.Replace("%1", pathOrUrl);
                hasArg = true;
            }
            Int32 spIndex;
            if (defBrowser[0] == '"')
                defBrowserProcess = defBrowser.Substring(0, defBrowser.IndexOf('"', 1) + 2).Trim();
            else if ((spIndex = defBrowser.IndexOf(" ", StringComparison.Ordinal)) > -1)
                defBrowserProcess = defBrowser.Substring(0, spIndex).Trim();
            else
                defBrowserProcess = defBrowser;

            String defBrowserArgs = defBrowser.Substring(defBrowserProcess.Length).TrimStart();
            // Not sure if this is possible / allowed, but better support it anyway.
            if (!hasArg)
            {
                if (defBrowserArgs.Length > 0)
                    defBrowserArgs += " ";
                defBrowserArgs += pathOrUrl;
            }
            // Run the process.
            defBrowserProcess = defBrowserProcess.Trim('"');
            if (!File.Exists(defBrowserProcess))
                return false;
            ProcessStartInfo psi = new ProcessStartInfo(defBrowserProcess, defBrowserArgs);
            psi.WorkingDirectory = Path.GetDirectoryName(defBrowserProcess);
            Process.Start(psi);
            return true;
        }
    }
}
