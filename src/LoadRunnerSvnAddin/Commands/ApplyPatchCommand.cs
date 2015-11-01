using System;
using System.Linq;
using MyLoadTest.LoadRunnerSvnAddin.Gui;

namespace MyLoadTest.LoadRunnerSvnAddin.Commands
{
    public class ApplyPatchCommand : SubversionCommand
    {
        protected override void Run(string filename)
        {
            SvnGuiWrapper.ApplyPatch(filename, WatchProjects().Callback);
        }
    }
}