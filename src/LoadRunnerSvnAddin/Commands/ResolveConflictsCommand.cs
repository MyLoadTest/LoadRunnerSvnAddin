using System;
using System.Linq;
using MyLoadTest.LoadRunnerSvnAddin.Gui;

namespace MyLoadTest.LoadRunnerSvnAddin.Commands
{
    public class ResolveConflictsCommand : SubversionCommand
    {
        protected override void Run(string fileName)
        {
            SvnGuiWrapper.ResolveConflict(fileName, WatchProjects().Callback);
        }
    }
}