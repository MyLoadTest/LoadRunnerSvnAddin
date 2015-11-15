using System;
using System.Linq;
using MyLoadTest.LoadRunnerSvnAddin.Gui;

namespace MyLoadTest.LoadRunnerSvnAddin.Commands
{
    public class RepoBrowserCommand : SubversionCommand
    {
        protected override void Run(string fileName)
        {
            SvnGuiWrapper.RepoBrowser(fileName, null);
        }
    }
}