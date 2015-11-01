using System;
using System.Linq;
using ICSharpCode.Core;
using MyLoadTest.LoadRunnerSvnAddin.Gui;

namespace MyLoadTest.LoadRunnerSvnAddin.Commands
{
    public class HelpCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            SvnGuiWrapper.ShowSvnHelp();
        }
    }
}