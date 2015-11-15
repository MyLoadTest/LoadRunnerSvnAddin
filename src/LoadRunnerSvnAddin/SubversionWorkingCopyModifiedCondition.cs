// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using MyLoadTest.LoadRunnerSvnAddin.Gui.ProjectBrowserVisitor;

namespace MyLoadTest.LoadRunnerSvnAddin
{
    public sealed class SubversionWorkingCopyModifiedCondition : IConditionEvaluator
    {
        public bool IsValid(object caller, Condition condition)
        {
            if (condition == null)
            {
                return false;
            }

            var selectedNode = ProjectBrowserPad.Instance?.SelectedNode;
            var fileSystemInfo = selectedNode?.GetNodeFileSystemInfo();
            var workingCopyRoot = fileSystemInfo?.GetWorkingCopyRoot();

            if (workingCopyRoot == null)
            {
                return false;
            }

            var statusKind = OverlayIconManager.GetStatus(fileSystemInfo);

            return statusKind == StatusKind.Added
                || statusKind == StatusKind.Modified
                || statusKind == StatusKind.Replaced;
        }
    }
}