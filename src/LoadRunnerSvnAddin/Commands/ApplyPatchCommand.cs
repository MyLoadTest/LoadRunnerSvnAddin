using System;
using System.Linq;
using MyLoadTest.LoadRunnerSvnAddin.Gui;

namespace MyLoadTest.LoadRunnerSvnAddin.Commands
{
    public class ApplyPatchCommand : SubversionCommand
    {
        protected override void Run(string fileName)
        {
            SvnGuiWrapper.ApplyPatch(fileName, WatchProjects().Callback);
        }
    }
}