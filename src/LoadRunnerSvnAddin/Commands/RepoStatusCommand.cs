using System;
using System.Linq;
using MyLoadTest.LoadRunnerSvnAddin.Gui;

namespace MyLoadTest.LoadRunnerSvnAddin.Commands
{
    public class RepoStatusCommand : SubversionCommand
    {
        protected override void Run(string fileName)
        {
            SvnGuiWrapper.RepoStatus(fileName, null);
        }
    }
}