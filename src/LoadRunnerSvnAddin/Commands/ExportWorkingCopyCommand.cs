using System;
using System.Linq;
using MyLoadTest.LoadRunnerSvnAddin.Gui;

namespace MyLoadTest.LoadRunnerSvnAddin.Commands
{
    public class ExportWorkingCopyCommand : SubversionCommand
    {
        protected override void Run(string fileName)
        {
            SvnGuiWrapper.Export(fileName, null);
        }
    }
}