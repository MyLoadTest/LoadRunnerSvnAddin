using System;
using System.Linq;
using MyLoadTest.LoadRunnerSvnAddin.Gui;

namespace MyLoadTest.LoadRunnerSvnAddin.Commands
{
    public class RelocateCommand : SubversionCommand
    {
        protected override void Run(string filename)
        {
            SvnGuiWrapper.Relocate(filename, WatchProjects().Callback);
        }
    }
}