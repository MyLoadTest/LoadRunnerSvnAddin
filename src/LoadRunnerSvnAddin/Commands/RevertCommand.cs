using System;
using System.Linq;
using MyLoadTest.LoadRunnerSvnAddin.Gui;

namespace MyLoadTest.LoadRunnerSvnAddin.Commands
{
    public class RevertCommand : SubversionCommand
    {
        protected override void Run(string filename)
        {
            SvnGuiWrapper.Revert(filename, WatchProjects().Callback);
        }
    }
}