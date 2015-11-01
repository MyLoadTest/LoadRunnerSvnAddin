using System;
using System.Linq;
using ICSharpCode.Core;
using MyLoadTest.LoadRunnerSvnAddin.Gui;

namespace MyLoadTest.LoadRunnerSvnAddin.Commands
{
    public class CheckoutCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            SvnGuiWrapper.ShowCheckoutDialog(null);
        }
    }
}