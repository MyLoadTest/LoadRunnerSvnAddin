// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Project;

namespace MyLoadTest.LoadRunnerSvnAddin.Gui.ProjectBrowserVisitor
{
    /// <summary>
    /// Static class managing the retrieval of the Subversion icons on a worker thread.
    /// </summary>
    public static class OverlayIconManager
    {
        private static readonly Queue<AbstractProjectBrowserTreeNode> Queue = new Queue<AbstractProjectBrowserTreeNode>();
        private static readonly Image[] StatusIcons = new Image[10];
        private static readonly object ClientLock = new object();
        private static readonly Bitmap StatusImages = WinFormsResourceService.GetBitmap("Icons.Svn.StatusImages");
        private static bool _threadRunning;
        private static SvnClientWrapper _client;
        private static bool _subversionDisabled;

        [SuppressMessage("ReSharper", "UnusedMember.Local",
            Justification = "Left as is from original SharpDevelop Svn Add-in")]
        private enum StatusIcon
        {
            Empty = 0,
            OK,
            Added,
            Deleted,
            Info,
            Empty2,
            Exclamation,
            PropertiesModified,
            Unknown,
            Modified
        }

        private static Image GetImage(StatusIcon status)
        {
            var index = (int)status;
            if (StatusIcons[index] == null)
            {
                var smallImage = new Bitmap(7, 10);
                using (var g = Graphics.FromImage(smallImage))
                {
                    //g.DrawImageUnscaled(statusImages, -index * 7, -3);
                    var srcRect = new Rectangle(index * 7, 3, 7, 10);
                    var destRect = new Rectangle(0, 0, 7, 10);
                    g.DrawImage(StatusImages, destRect, srcRect, GraphicsUnit.Pixel);

                    //g.DrawLine(Pens.Black, 0, 0, 7, 10);
                }

                StatusIcons[index] = smallImage;
            }

            return StatusIcons[index];
        }

        public static Image GetImage(StatusKind status)
        {
            switch (status)
            {
                case StatusKind.Added:
                    return GetImage(StatusIcon.Added);

                case StatusKind.Deleted:
                    return GetImage(StatusIcon.Deleted);

                case StatusKind.Modified:
                case StatusKind.Replaced:
                    return GetImage(StatusIcon.Modified);

                case StatusKind.Normal:
                    return GetImage(StatusIcon.OK);

                case StatusKind.Conflicted:
                case StatusKind.Obstructed:
                    return GetImage(StatusIcon.Exclamation);

                default:
                    return null;
            }
        }

        public static void Enqueue(AbstractProjectBrowserTreeNode node)
        {
            if (_subversionDisabled)
                return;
            lock (Queue)
            {
                Queue.Enqueue(node);
                if (!_threadRunning)
                {
                    _threadRunning = true;
                    ThreadPool.QueueUserWorkItem(Run);
                }
            }
        }

        public static void EnqueueRecursive(AbstractProjectBrowserTreeNode node)
        {
            if (_subversionDisabled)
                return;
            lock (Queue)
            {
                Queue.Enqueue(node);

                // use breadth-first search
                var q = new Queue<AbstractProjectBrowserTreeNode>();
                q.Enqueue(node);
                while (q.Count > 0)
                {
                    node = q.Dequeue();
                    foreach (TreeNode n in node.Nodes)
                    {
                        node = n as AbstractProjectBrowserTreeNode;
                        if (node != null)
                        {
                            q.Enqueue(node);
                            Queue.Enqueue(node);
                        }
                    }
                }

                if (!_threadRunning)
                {
                    _threadRunning = true;
                    ThreadPool.QueueUserWorkItem(Run);
                }
            }
        }

        public static bool SubversionDisabled => _subversionDisabled;

        private static void Run(object state)
        {
            LoggingService.Debug("SVN: OverlayIconManager Thread started");

            // sleep a tiny bit to give main thread time to add more jobs to the queue
            Thread.Sleep(2);
            while (true)
            {
                if (ICSharpCode.SharpDevelop.ParserService.LoadSolutionProjectsThreadRunning)
                {
                    // Run OverlayIconManager much more slowly while solution is being loaded.
                    // This prevents the disk from seeking too much
                    Thread.Sleep(100);
                }
                AbstractProjectBrowserTreeNode node;
                lock (Queue)
                {
                    if (Queue.Count == 0)
                    {
                        _threadRunning = false;
                        ClearStatusCache();
                        LoggingService.Debug("SVN: OverlayIconManager Thread finished");
                        return;
                    }
                    node = Queue.Dequeue();
                }
                try
                {
                    RunStep(node);
                }
                catch (Exception ex)
                {
                    MessageService.ShowException(ex);
                }
            }
        }

        public static void ClearStatusCache()
        {
            lock (ClientLock)
            {
                _client?.ClearStatusCache();
            }
        }

        public static StatusKind GetStatus(string fileName)
        {
            lock (ClientLock)
            {
                if (_subversionDisabled)
                    return StatusKind.None;

                //Console.WriteLine(fileName);

                if (_client == null)
                {
                    try
                    {
                        _client = new SvnClientWrapper();
                    }
                    catch (Exception ex)
                    {
                        _subversionDisabled = true;
                        ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.SafeThreadAsyncCall(
                            MessageService.ShowWarning,
                            "Error initializing Subversion library:\n" + ex);

                        return StatusKind.None;
                    }
                }

                try
                {
                    return _client.SingleStatus(fileName).TextStatus;
                }
                catch (SvnClientException ex)
                {
                    LoggingService.Warn("Error getting status of " + fileName, ex);
                    return StatusKind.None;
                }
            }
        }

        private static void RunStep(AbstractProjectBrowserTreeNode node)
        {
            if (node.IsDisposed)
                return;

            var fileNode = node as FileNode;
            StatusKind status;
            if (fileNode != null)
            {
                status = GetStatus(fileNode.FileName);
            }
            else
            {
                var directoryNode = node as DirectoryNode;
                if (directoryNode != null)
                {
                    status = GetStatus(directoryNode.Directory);
                }
                else
                {
                    var solNode = node as SolutionNode;
                    if (solNode != null)
                    {
                        status = GetStatus(solNode.Solution.Directory);
                    }
                    else
                    {
                        return;
                    }
                }
            }

            ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.SafeThreadAsyncCall(
                delegate
                {
                    node.Overlay = GetImage(status);
                });
        }
    }
}