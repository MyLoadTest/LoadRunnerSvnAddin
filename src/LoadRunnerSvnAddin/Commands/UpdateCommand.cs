using System;
using System.Linq;
using MyLoadTest.LoadRunnerSvnAddin.Gui;

namespace MyLoadTest.LoadRunnerSvnAddin.Commands
{
    public class UpdateCommand : SubversionCommand
    {
        protected override void Run(string fileName)
        {
            SvnGuiWrapper.Update(fileName, WatchProjects().Callback);
        }
    }
}