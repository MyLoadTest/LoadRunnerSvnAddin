// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Gui;

namespace MyLoadTest.LoadRunnerSvnAddin.Gui.HistoryViewDisplayBinding
{
    /// <summary>
    /// Description of InfoPanel.
    /// </summary>
    public partial class InfoPanel : UserControl
    {
        private readonly string _fileName;
        private long _lastRevision = -1;
        private ListViewItem _loadChangedPathsItem;
        private volatile bool _isLoadingChangedPaths;

        public InfoPanel(string fileName)
        {
            if (fileName == null)
                throw new ArgumentNullException(nameof(fileName));
            _fileName = fileName;

            InitializeComponent();

            revisionListView.SelectedIndexChanged += RevisionListViewSelectionChanged;
            commentRichTextBox.Font = WinFormsResourceService.DefaultMonospacedFont;
            commentRichTextBox.Enabled = false;
        }

        public void ShowError(Exception ex)
        {
            var txt = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                BackColor = SystemColors.Window
            };

            SvnClientException svn;
            txt.Text = "";
            while ((svn = ex as SvnClientException) != null)
            {
                txt.Text += svn.Message + Environment.NewLine;
                ex = svn.GetInnerException();
            }
            if (ex != null)
            {
                txt.Text += ex.ToString();
            }
            txt.Dock = DockStyle.Fill;
            revisionListView.Controls.Add(txt);
        }

        public void AddLogMessage(LogMessage logMessage)
        {
            if (_lastRevision < 0)
                _lastRevision = logMessage.Revision;
            var newItem = new ListViewItem(
                new[]
                {
                    logMessage.Revision.ToString(),
                    logMessage.Author,
                    logMessage.Date.ToString(CultureInfo.CurrentCulture),
                    logMessage.Message
                })
            {
                Tag = logMessage
            };

            revisionListView.Items.Add(newItem);
        }

        private void RevisionListViewSelectionChanged(object sender, EventArgs e)
        {
            changesListView.Items.Clear();
            if (revisionListView.SelectedItems.Count == 0)
            {
                commentRichTextBox.Text = "";
                commentRichTextBox.Enabled = false;
                return;
            }
            commentRichTextBox.Enabled = true;
            var item = revisionListView.SelectedItems[0];
            var logMessage = item.Tag as LogMessage;
            commentRichTextBox.Text = logMessage.Message;
            var changes = logMessage.ChangedPaths;
            if (changes == null)
            {
                changesListView.Items.Add("Loading...");
                if (!_isLoadingChangedPaths)
                {
                    _isLoadingChangedPaths = true;
                    _loadChangedPathsItem = item;
                    ThreadPool.QueueUserWorkItem(LoadChangedPaths);
                }
            }
            else
            {
                var pathWidth = 70;
                var copyFromWidth = 70;
                using (var g = CreateGraphics())
                {
                    foreach (var change in changes)
                    {
                        var path = change.Path;
                        path = path.Replace('\\', '/');
                        var size = g.MeasureString(path, changesListView.Font);
                        if (size.Width + 4 > pathWidth)
                            pathWidth = (int)size.Width + 4;
                        var copyFrom = change.CopyFromPath;
                        if (copyFrom == null)
                        {
                            copyFrom = string.Empty;
                        }
                        else
                        {
                            copyFrom = copyFrom + " : r" + change.CopyFromRevision;
                            size = g.MeasureString(copyFrom, changesListView.Font);
                            if (size.Width + 4 > copyFromWidth)
                                copyFromWidth = (int)size.Width + 4;
                        }
                        var newItem = new ListViewItem(new[] {
                                                                    SvnClientWrapper.GetActionString(change.Action),
                                                                    path,
                                                                    copyFrom
                                                                });
                        changesListView.Items.Add(newItem);
                    }
                }
                changesListView.Columns[1].Width = pathWidth;
                changesListView.Columns[2].Width = copyFromWidth;
            }
        }


        private void LoadChangedPaths(object state)
        {
            try
            {
                var logMessage = (LogMessage)_loadChangedPathsItem.Tag;
                using (var client = new SvnClientWrapper())
                {
                    client.AllowInteractiveAuthorization();
                    try
                    {
                        client.Log(new[] { _fileName },
                                   Revision.FromNumber(logMessage.Revision), // Revision start
                                   Revision.FromNumber(logMessage.Revision), // Revision end
                                   int.MaxValue,           // limit
                                   true,                   // bool discoverChangePath
                                   false,                  // bool strictNodeHistory
                                   ReceiveChangedPaths);
                    }
                    catch (SvnClientException ex)
                    {
                        if (ex.IsKnownError(KnownError.FileNotFound))
                        {
                            // This can happen when the file was renamed/moved so it cannot be found
                            // directly in the old revision. In that case, we do a full download of
                            // all revisions (so the file can be found in the new revision and svn can
                            // follow back its history).
                            client.Log(new[] { _fileName },
                                       Revision.FromNumber(1),            // Revision start
                                       Revision.FromNumber(_lastRevision), // Revision end
                                       int.MaxValue,           // limit
                                       true,                   // bool discoverChangePath
                                       false,                  // bool strictNodeHistory
                                       ReceiveAllChangedPaths);
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
                _loadChangedPathsItem = null;
                _isLoadingChangedPaths = false;
                WorkbenchSingleton.SafeThreadAsyncCall<object, EventArgs>(RevisionListViewSelectionChanged, null, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                MessageService.ShowException(ex);
            }
        }

        private void ReceiveChangedPaths(LogMessage logMessage)
        {
            _loadChangedPathsItem.Tag = logMessage;
        }

        private void ReceiveAllChangedPaths(LogMessage logMessage)
        {
            WorkbenchSingleton.SafeThreadAsyncCall(ReceiveAllChangedPathsInvoked, logMessage);
        }

        private void ReceiveAllChangedPathsInvoked(LogMessage logMessage)
        {
            foreach (ListViewItem item in revisionListView.Items)
            {
                var oldMessage = (LogMessage)item.Tag;
                if (oldMessage.Revision == logMessage.Revision)
                {
                    item.Tag = logMessage;
                    break;
                }
            }
        }
    }
}