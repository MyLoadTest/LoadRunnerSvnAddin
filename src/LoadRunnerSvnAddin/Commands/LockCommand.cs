using System;
using System.Linq;
using MyLoadTest.LoadRunnerSvnAddin.Gui;

namespace MyLoadTest.LoadRunnerSvnAddin.Commands
{
    public class LockCommand : SubversionCommand
    {
        protected override void Run(string fileName)
        {
            SvnGuiWrapper.Lock(fileName, null);
        }
    }
}