using System;
using System.Linq;
using MyLoadTest.LoadRunnerSvnAddin.Gui;

namespace MyLoadTest.LoadRunnerSvnAddin.Commands
{
    public class IgnoreCommand : SubversionCommand
    {
        protected override void Run(string filename)
        {
            SvnGuiWrapper.Ignore(filename, WatchProjects().Callback);
        }
    }
}