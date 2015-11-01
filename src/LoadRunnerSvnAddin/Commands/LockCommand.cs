using System;
using System.Linq;
using MyLoadTest.LoadRunnerSvnAddin.Gui;

namespace MyLoadTest.LoadRunnerSvnAddin.Commands
{
    public class LockCommand : SubversionCommand
    {
        protected override void Run(string filename)
        {
            SvnGuiWrapper.Lock(filename, null);
        }
    }
}