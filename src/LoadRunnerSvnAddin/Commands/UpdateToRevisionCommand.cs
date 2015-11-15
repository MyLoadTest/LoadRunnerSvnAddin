using System;
using System.Linq;
using MyLoadTest.LoadRunnerSvnAddin.Gui;

namespace MyLoadTest.LoadRunnerSvnAddin.Commands
{
    public class UpdateToRevisionCommand : SubversionCommand
    {
        protected override void Run(string fileName)
        {
            SvnGuiWrapper.UpdateToRevision(fileName, WatchProjects().Callback);
        }
    }
}