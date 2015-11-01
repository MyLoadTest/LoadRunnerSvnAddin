using System;
using System.Linq;
using MyLoadTest.LoadRunnerSvnAddin.Gui;

namespace MyLoadTest.LoadRunnerSvnAddin.Commands
{
    public class AddCommand : SubversionCommand
    {
        protected override void Run(string filename)
        {
            SvnGuiWrapper.Add(filename, WatchProjects().Callback);
        }
    }
}