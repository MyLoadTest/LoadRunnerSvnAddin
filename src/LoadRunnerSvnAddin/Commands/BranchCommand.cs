using System;
using System.Linq;
using MyLoadTest.LoadRunnerSvnAddin.Gui;

namespace MyLoadTest.LoadRunnerSvnAddin.Commands
{
    public class BranchCommand : SubversionCommand
    {
        protected override void Run(string filename)
        {
            SvnGuiWrapper.Branch(filename, WatchProjects().Callback);
        }
    }
}