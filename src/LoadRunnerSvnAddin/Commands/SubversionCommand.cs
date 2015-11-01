// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using MyLoadTest.LoadRunnerSvnAddin.Gui.ProjectBrowserVisitor;

namespace MyLoadTest.LoadRunnerSvnAddin.Commands
{
    public abstract class SubversionCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            var node = ProjectBrowserPad.Instance.SelectedNode;
            if (node != null)
            {
                string nodeFileName = null;
                if (node is DirectoryNode)
                {
                    nodeFileName = ((DirectoryNode)node).Directory;
                }
                else if (node is FileNode)
                {
                    nodeFileName = ((FileNode)node).FileName;
                }
                else if (node is SolutionNode)
                {
                    nodeFileName = ((SolutionNode)node).Solution.Directory;
                }
                if (nodeFileName != null)
                {
                    var unsavedFiles = new List<OpenedFile>();
                    foreach (var file in FileService.OpenedFiles)
                    {
                        if (file.IsDirty && !file.IsUntitled)
                        {
                            if (string.IsNullOrEmpty(file.FileName))
                                continue;
                            if (FileUtility.IsUrl(file.FileName))
                                continue;
                            if (FileUtility.IsBaseDirectory(nodeFileName, file.FileName))
                            {
                                unsavedFiles.Add(file);
                            }
                        }
                    }
                    if (unsavedFiles.Count > 0)
                    {
                        if (MessageService.ShowCustomDialog(
                            MessageService.DefaultMessageBoxTitle,
                            "${res:AddIns.Subversion.SVNRequiresSavingFiles}",
                            0, 1,
                            "${res:AddIns.Subversion.SaveFiles}", "${res:Global.CancelButtonText}")
                            == 0)
                        {
                            // Save
                            foreach (var file in unsavedFiles)
                            {
                                ICSharpCode.SharpDevelop.Commands.SaveFile.Save(file);
                            }
                        }
                        else
                        {
                            // Cancel
                            return;
                        }
                    }

                    // now run the actual operation:
                    Run(nodeFileName);
                }
            }
        }

        protected ProjectWatcher WatchProjects()
        {
            return new ProjectWatcher(ProjectService.OpenSolution);
        }

        private static void CallbackInvoked()
        {
            OverlayIconManager.ClearStatusCache();
            var node = ProjectBrowserPad.Instance.SelectedNode;
            if (node != null)
            {
                OverlayIconManager.EnqueueRecursive(node);
            }
        }

        protected abstract void Run(string filename);

        private struct ProjectEntry
        {
            private readonly string _fileName;
            private readonly long _size;
            private readonly DateTime _writeTime;

            public ProjectEntry(FileInfo file)
            {
                _fileName = file.FullName;
                if (file.Exists)
                {
                    _size = file.Length;
                    _writeTime = file.LastWriteTime;
                }
                else
                {
                    _size = -1;
                    _writeTime = DateTime.MinValue;
                }
            }

            public bool HasFileChanged()
            {
                var file = new FileInfo(_fileName);
                long newSize;
                DateTime newWriteTime;
                if (file.Exists)
                {
                    newSize = file.Length;
                    newWriteTime = file.LastWriteTime;
                }
                else
                {
                    newSize = -1;
                    newWriteTime = DateTime.MinValue;
                }
                return _size != newSize || _writeTime != newWriteTime;
            }
        }

        /// <summary>
        /// Remembers a list of file sizes and last write times. If a project
        /// changed during the operation, suggest that the user reloads the solution.
        /// </summary>
        protected sealed class ProjectWatcher
        {
            private readonly List<ProjectEntry> _list = new List<ProjectEntry>();
            private readonly Solution _solution;

            internal ProjectWatcher(Solution solution)
            {
                this._solution = solution;
                if (AddInOptions.AutomaticallyReloadProject && solution != null)
                {
                    _list.Add(new ProjectEntry(new FileInfo(solution.FileName)));
                    foreach (var p in solution.Projects)
                    {
                        _list.Add(new ProjectEntry(new FileInfo(p.FileName)));
                    }
                }
            }

            public void Callback()
            {
                WorkbenchSingleton.SafeThreadAsyncCall(CallbackInvoked);
            }

            private void CallbackInvoked()
            {
                SubversionCommand.CallbackInvoked();
                if (ProjectService.OpenSolution != _solution)
                    return;
                if (!_list.TrueForAll(projectEntry => !projectEntry.HasFileChanged()))
                {
                    // if at least one project was changed:
                    if (MessageService.ShowCustomDialog(
                        MessageService.DefaultMessageBoxTitle,
                        "${res:ICSharpCode.SharpDevelop.Project.SolutionAlteredExternallyMessage}",
                        0, 1,
                        "${res:ICSharpCode.SharpDevelop.Project.ReloadSolution}", "${res:ICSharpCode.SharpDevelop.Project.KeepOldSolution}")
                        == 0)
                    {
                        ProjectService.LoadSolution(_solution.FileName);
                    }
                }
            }
        }
    }
}