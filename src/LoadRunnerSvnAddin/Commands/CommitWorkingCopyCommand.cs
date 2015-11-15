using System;
using System.IO;
using System.Linq;
using MyLoadTest.LoadRunnerSvnAddin.Gui;

namespace MyLoadTest.LoadRunnerSvnAddin.Commands
{
    public class CommitWorkingCopyCommand : SubversionCommand
    {
        protected override void Run(string fileName)
        {
            SvnGuiWrapper.Commit(fileName, WatchProjects().Callback);
        }

        protected override FileSystemInfo GetSelectedNodeFileSystemInfo()
        {
            var fileSystemInfo = base.GetSelectedNodeFileSystemInfo();
            return fileSystemInfo?.GetWorkingCopyRoot();
        }
    }
}