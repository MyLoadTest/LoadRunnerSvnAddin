using System;
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace MyLoadTest.LoadRunnerSvnAddin
{
    /// <summary>
    /// Gets if a folder is under version control
    /// </summary>
    public sealed class SubversionIsControlledCondition : IConditionEvaluator
    {
        public bool IsValid(object caller, Condition condition)
        {
            var node = ProjectBrowserPad.Instance?.SelectedNode;

            var fileSystemInfo = node?.GetNodeFileSystemInfo();
            return fileSystemInfo != null && fileSystemInfo.CanBeVersionControlledItem();
        }
    }
}