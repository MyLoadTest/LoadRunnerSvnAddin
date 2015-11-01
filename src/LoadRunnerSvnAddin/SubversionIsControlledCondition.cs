using System;
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using MyLoadTest.LoadRunnerSvnAddin.Commands;

namespace MyLoadTest.LoadRunnerSvnAddin
{
    /// <summary>
    /// Gets if a folder is under version control
    /// </summary>
    public class SubversionIsControlledCondition : IConditionEvaluator
    {
        public bool IsValid(object caller, Condition condition)
        {
            var node = ProjectBrowserPad.Instance.SelectedNode as FileNode;
            if (node != null)
            {
                return RegisterEventsCommand.CanBeVersionControlledFile(node.FileName);
            }
            var dir = ProjectBrowserPad.Instance.SelectedNode as DirectoryNode;
            if (dir != null)
            {
                return RegisterEventsCommand.CanBeVersionControlledDirectory(dir.Directory);
            }
            var sol = ProjectBrowserPad.Instance.SelectedNode as SolutionNode;
            if (sol != null)
            {
                return RegisterEventsCommand.CanBeVersionControlledDirectory(sol.Solution.Directory);
            }
            return false;
        }
    }
}