using System;
using System.IO;
using System.Linq;
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
            return GetWorkingCopyRoot(fileSystemInfo) != null;
        }

        public static bool CanBeVersionControlledFile(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            return CanBeVersionControlledDirectory(Path.GetDirectoryName(filePath));
        }

        public static bool CanBeVersionControlledDirectory(string directoryPath)
        {
            if (directoryPath == null)
            {
                throw new ArgumentNullException(nameof(directoryPath));
            }

            return GetWorkingCopyRoot(directoryPath) != null;
        }

        public static DirectoryInfo GetWorkingCopyRoot(this FileSystemInfo fileSystemInfo)
        {
            var fileInfo = fileSystemInfo as FileInfo;
            if (fileInfo != null)
            {
                return GetWorkingCopyRoot(Path.GetDirectoryName(fileInfo.FullName));
            }

            var directoryInfo = fileSystemInfo as DirectoryInfo;
            if (directoryInfo != null)
            {
                return GetWorkingCopyRoot(directoryInfo.FullName);
            }

            return null;
        }

        #endregion

        #region Private Methods

        private static DirectoryInfo GetWorkingCopyRoot(string directoryPath)
        {
            if (OverlayIconManager.SubversionDisabled)
            {
                return null;
            }

            try
            {
                if (!Path.IsPathRooted(directoryPath))
                {
                    return null;
                }
            }
            catch (ArgumentException)
            {
                return null;
            }

            var info = new DirectoryInfo(directoryPath);
            while (info != null)
            {
                if (Directory.Exists(Path.Combine(info.FullName, ".svn")))
                {
                    return info;
                }

                info = info.Parent;
            }

            return null;
        }

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