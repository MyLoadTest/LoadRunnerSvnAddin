using System;
using System.Linq;
using MyLoadTest.LoadRunnerSvnAddin.Gui;

namespace MyLoadTest.LoadRunnerSvnAddin.Commands
{
    public class UpdateCommand : SubversionCommand
    {
        protected override void Run(string filename)
        {
            SvnGuiWrapper.Update(filename, WatchProjects().Callback);
        }
    }
}