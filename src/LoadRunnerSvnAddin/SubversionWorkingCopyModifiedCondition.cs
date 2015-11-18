// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace MyLoadTest.LoadRunnerSvnAddin
{
    public sealed class SubversionWorkingCopyModifiedCondition : IConditionEvaluator
    {
        private static readonly HashSet<StatusKind> Modified =
            new HashSet<StatusKind>(new[] { StatusKind.Added, StatusKind.Modified, StatusKind.Replaced });

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

            using (var client = new SvnClientWrapper())
            {
                var multiStatus = client.MultiStatus(workingCopyRoot);
                var result = multiStatus.Any(pair => Modified.Contains(pair.Value.TextStatus));
                return result;
            }
        }
    }
}