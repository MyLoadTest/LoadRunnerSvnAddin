using System;
using System.Linq;
using MyLoadTest.LoadRunnerSvnAddin.Gui;

namespace MyLoadTest.LoadRunnerSvnAddin.Commands
{
    public class MergeCommand : SubversionCommand
    {
        protected override void Run(string filename)
        {
            SvnGuiWrapper.Merge(filename, WatchProjects().Callback);
        }
    }
}