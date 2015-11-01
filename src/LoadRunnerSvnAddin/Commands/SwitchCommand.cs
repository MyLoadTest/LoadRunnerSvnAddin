using System;
using System.Linq;
using MyLoadTest.LoadRunnerSvnAddin.Gui;

namespace MyLoadTest.LoadRunnerSvnAddin.Commands
{
    public class SwitchCommand : SubversionCommand
    {
        protected override void Run(string filename)
        {
            SvnGuiWrapper.Switch(filename, WatchProjects().Callback);
        }
    }
}