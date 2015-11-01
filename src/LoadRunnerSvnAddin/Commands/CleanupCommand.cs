using System;
using System.Linq;
using MyLoadTest.LoadRunnerSvnAddin.Gui;

namespace MyLoadTest.LoadRunnerSvnAddin.Commands
{
    public class CleanupCommand : SubversionCommand
    {
        protected override void Run(string filename)
        {
            SvnGuiWrapper.Cleanup(filename, null);
        }
    }
}