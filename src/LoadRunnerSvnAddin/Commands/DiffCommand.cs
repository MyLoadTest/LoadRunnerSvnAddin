using System;
using System.Linq;
using MyLoadTest.LoadRunnerSvnAddin.Gui;

namespace MyLoadTest.LoadRunnerSvnAddin.Commands
{
    public class DiffCommand : SubversionCommand
    {
        protected override void Run(string filename)
        {
            SvnGuiWrapper.Diff(filename, WatchProjects().Callback);
        }
    }
}