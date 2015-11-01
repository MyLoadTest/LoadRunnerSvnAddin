using System;
using System.Linq;
using MyLoadTest.LoadRunnerSvnAddin.Gui;

namespace MyLoadTest.LoadRunnerSvnAddin.Commands
{
    public class ExportWorkingCopyCommand : SubversionCommand
    {
        protected override void Run(string filename)
        {
            SvnGuiWrapper.Export(filename, null);
        }
    }
}