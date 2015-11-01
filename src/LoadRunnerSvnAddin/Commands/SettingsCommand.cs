using System;
using System.Linq;
using ICSharpCode.Core;
using MyLoadTest.LoadRunnerSvnAddin.Gui;

namespace MyLoadTest.LoadRunnerSvnAddin.Commands
{
    public class SettingsCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            SvnGuiWrapper.ShowSvnSettings();
        }
    }
}