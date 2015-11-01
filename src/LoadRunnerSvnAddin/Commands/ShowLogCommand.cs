using System;
using System.Linq;
using MyLoadTest.LoadRunnerSvnAddin.Gui;

namespace MyLoadTest.LoadRunnerSvnAddin.Commands
{
    public class ShowLogCommand : SubversionCommand
    {
        protected override void Run(string filename)
        {
            SvnGuiWrapper.ShowLog(filename, WatchProjects().Callback);
        }
    }
}