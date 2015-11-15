using System;
using System.Linq;
using MyLoadTest.LoadRunnerSvnAddin.Gui;

namespace MyLoadTest.LoadRunnerSvnAddin.Commands
{
    public class MergeCommand : SubversionCommand
    {
        protected override void Run(string fileName)
        {
            SvnGuiWrapper.Merge(fileName, WatchProjects().Callback);
        }
    }
}