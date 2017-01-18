using PoGo.NecroBot.Logic.Common;
using PoGo.NecroBot.Logic.Event;
using PoGo.NecroBot.Logic.Event.UI;
using PoGo.NecroBot.Logic.Logging;
using PoGo.NecroBot.Logic.State;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PoGo.NecroBot.Logic.Forms
{

    public partial class AutoUpdateForm : Form
    {
        private string CHANGE_LOGS = "https://raw.githubusercontent.com/Necrobot-Private/NecroBot/master/changelogs/v{0}.rft";

        public string LatestVersion { get; set; }

        public string CurrentVersion { get; set; }

        public bool AutoUpdate { get; set; }
        public string DownloadLink { get; set; }
        public string Destination { get; set; }

        public ISession Session { get; set; }
        public AutoUpdateForm()
        {
            InitializeComponent();
        }

        private void AutoUpdateForm_Load(object sender, EventArgs e)
        {
            this.richTextBox1.SetInnerMargins(25, 25, 25, 25);
            this.lblCurrent.Text = CurrentVersion;
            this.lblLatest.Text = LatestVersion;
            var changelog = string.Format(CHANGE_LOGS, LatestVersion);
            var tempPath = Path.GetTempPath() + Path.GetFileName(changelog);
            LoadChangeLogs(changelog, tempPath);
            if (AutoUpdate)
            {
                btnUpdate.Enabled = false;
                btnUpdate.Text = "Downloading...";
                StartDownload();
            }
        }

        private void LoadChangeLogs(string changelog, string tempPath)
        {
            Task.Run(async () =>
            {
                await Task.Delay(2000);

                using (WebClient client = new WebClient())
                {
                    try
                    {
                        client.DownloadFile(changelog, tempPath);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }

            }).ContinueWith((t) =>
            {
                //load content
                this.Invoke(new Action(() =>
                {
                    if (t.Result)
                    {
                        this.richTextBox1.LoadFile(tempPath);
                    }
                    else
                    {
                        richTextBox1.Text = "No change logs....";
                    }
                }));

            });
            ;
        }

        public bool DownloadFile(string url, string dest)
        {
            Session.EventDispatcher.Send(new UpdateEvent
            {
                Message = Session.Translation.GetTranslation(TranslationString.DownloadingUpdate)
            });

            using (var client = new WebClient())
            {
                try
                {
                    client.DownloadFileCompleted += Client_DownloadFileCompleted;
                    client.DownloadProgressChanged += Client_DownloadProgressChanged;

                    client.DownloadFileAsync(new Uri(url), dest);
                    Logger.Write(dest, LogLevel.Info);
                }
                catch
                {
                    this.Close();
                }
                return true;
            }
        }

        private void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            Session.EventDispatcher.Send(new UpdateEvent
            {
                Message = Session.Translation.GetTranslation(TranslationString.FinishedDownloadingRelease)
            });

            this.Invoke(new Action(() =>
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }));
        }

        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            this.Invoke(new Action(() =>
            {
                progressBar1.Value = e.ProgressPercentage;
            }));
        }


        public void StartDownload()
        {
            Session.EventDispatcher.Send(new StatusBarEvent($"Auto update v{LatestVersion}, downloading.. .{DownloadLink}"));
            Logger.Write(DownloadLink, LogLevel.Info);
            DownloadFile(DownloadLink, Destination);

        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            btnUpdate.Text = "Downloading...";
            btnUpdate.Enabled = false;
            StartDownload();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
