using System;
using System.Linq;
using MyLoadTest.LoadRunnerSvnAddin.Gui;

namespace MyLoadTest.LoadRunnerSvnAddin.Commands
{
    public class BlameCommand : SubversionCommand
    {
        protected override void Run(string filename)
        {
            SvnGuiWrapper.Blame(filename, null);
        }
    }
}