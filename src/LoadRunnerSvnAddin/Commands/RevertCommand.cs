using System;
using System.Linq;
using MyLoadTest.LoadRunnerSvnAddin.Gui;

namespace MyLoadTest.LoadRunnerSvnAddin.Commands
{
    public class RevertCommand : SubversionCommand
    {
        protected override void Run(string fileName)
        {
            SvnGuiWrapper.Revert(fileName, WatchProjects().Callback);
        }
    }
}