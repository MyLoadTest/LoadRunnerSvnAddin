using System;
using System.Linq;
using ICSharpCode.SharpDevelop.Project;
using MyLoadTest.LoadRunnerSvnAddin.Gui.ProjectBrowserVisitor;

namespace MyLoadTest.LoadRunnerSvnAddin.Commands
{
    public sealed class RefreshAllStatusCommand : SubversionCommand
    {
        protected override void Run(string fileName)
        {
            OverlayIconManager.ClearStatusCache();

            var node = ProjectBrowserPad.Instance?.SolutionNode;
            if (node != null)
            {
                OverlayIconManager.EnqueueRecursive(node);
            }
        }
    }
}