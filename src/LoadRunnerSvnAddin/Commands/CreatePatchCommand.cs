using System;
using System.Linq;
using MyLoadTest.LoadRunnerSvnAddin.Gui;

namespace MyLoadTest.LoadRunnerSvnAddin.Commands
{
    public class CreatePatchCommand : SubversionCommand
    {
        protected override void Run(string fileName)
        {
            SvnGuiWrapper.CreatePatch(fileName, null);
        }
    }
}