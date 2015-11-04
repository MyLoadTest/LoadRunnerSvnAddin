using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using HP.LR.Vugen.BackEnd.Project.ProjectSystem.ScriptItems;
using HP.Utt.ProjectSystem;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using MyLoadTest.LoadRunnerSvnAddin.Gui.ProjectBrowserVisitor;

namespace MyLoadTest.LoadRunnerSvnAddin
{
    internal static class LocalHelper
    {
        #region Public Methods

        public static FileSystemInfo GetNodeFileSystemInfo(this ExtTreeNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            if (node.IsDisposed)
            {
                return null;
            }

            var result = ((node as UttBaseTreeNode)?.Item as FileBasedScriptItem)?.FullFileName.ToFileInfo()
                ?? (node as FileNode)?.FileName.ToFileInfo()
                    ?? (node as SolutionNode)?.Solution?.Directory.ToDirectoryInfo()
                        ?? (node as UttProjectNode)?.Project?.FileName.ToFileInfo()
                            ?? (FileSystemInfo)(node as DirectoryNode)?.Directory.ToDirectoryInfo();

            return result;
        }

        public static StatusKind? GetNodeStatus(this ExtTreeNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            if (node.IsDisposed)
            {
                return null;
            }

            var fileSystemInfo = GetNodeFileSystemInfo(node);

            return fileSystemInfo == null ? default(StatusKind?) : OverlayIconManager.GetStatus(fileSystemInfo);
        }

        public static bool CanBeVersionControlledItem(this FileSystemInfo fileSystemInfo)
        {
            var fileInfo = fileSystemInfo as FileInfo;
            if (fileInfo != null)
            {
                return CanBeVersionControlledFile(fileInfo.FullName);
            }

            var directoryInfo = fileSystemInfo as DirectoryInfo;
            if (directoryInfo != null)
            {
                return CanBeVersionControlledDirectory(directoryInfo.FullName);
            }

            return false;
        }

        public static bool CanBeVersionControlledFile(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            return !OverlayIconManager.SubversionDisabled
                && CanBeVersionControlledDirectory(Path.GetDirectoryName(filePath));
        }

        public static bool CanBeVersionControlledDirectory(string directoryPath)
        {
            if (directoryPath == null)
            {
                throw new ArgumentNullException(nameof(directoryPath));
            }

            if (OverlayIconManager.SubversionDisabled)
            {
                return false;
            }

            return Directory.Exists(Path.Combine(directoryPath, ".svn"))
                || Directory.Exists(Path.Combine(directoryPath, "_svn"));
        }

        #endregion

        #region Private Methods

        private static FileInfo ToFileInfo(this string filePath)
        {
            return string.IsNullOrEmpty(filePath) ? null : new FileInfo(filePath);
        }

        private static DirectoryInfo ToDirectoryInfo(this string directoryPath)
        {
            return string.IsNullOrEmpty(directoryPath) ? null : new DirectoryInfo(directoryPath);
        }

        #endregion
    }
}