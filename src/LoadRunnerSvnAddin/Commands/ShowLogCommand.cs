using System;
using System.Linq;
using MyLoadTest.LoadRunnerSvnAddin.Gui;

namespace MyLoadTest.LoadRunnerSvnAddin.Commands
{
    public class ShowLogCommand : SubversionCommand
    {
        protected override void Run(string fileName)
        {
            SvnGuiWrapper.ShowLog(fileName, WatchProjects().Callback);
        }
    }
}