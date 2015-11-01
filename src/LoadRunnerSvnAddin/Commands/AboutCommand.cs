using System;
using System.Linq;
using ICSharpCode.Core;
using MyLoadTest.LoadRunnerSvnAddin.Gui;

namespace MyLoadTest.LoadRunnerSvnAddin.Commands
{
    public class AboutCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            SvnGuiWrapper.ShowSvnAbout();
        }
    }
}