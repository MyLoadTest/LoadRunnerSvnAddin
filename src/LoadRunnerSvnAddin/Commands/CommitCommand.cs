using System;
using System.Linq;
using MyLoadTest.LoadRunnerSvnAddin.Gui;

namespace MyLoadTest.LoadRunnerSvnAddin.Commands
{
    public class CommitCommand : SubversionCommand
    {
        protected override void Run(string filename)
        {
            SvnGuiWrapper.Commit(filename, WatchProjects().Callback);
        }
    }
}