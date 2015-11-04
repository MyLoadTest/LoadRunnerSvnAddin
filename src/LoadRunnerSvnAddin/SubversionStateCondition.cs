// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using MyLoadTest.LoadRunnerSvnAddin.Gui.ProjectBrowserVisitor;

namespace MyLoadTest.LoadRunnerSvnAddin
{
    public sealed class SubversionStateCondition : IConditionEvaluator
    {
        public bool IsValid(object caller, Condition condition)
        {
            if (condition == null)
            {
                return false;
            }

            var fileSystemInfo = ProjectBrowserPad.Instance?.SelectedNode?.GetNodeFileSystemInfo();
            if (fileSystemInfo == null)
            {
                return false;
            }

            var itemProperty = condition.Properties["item"];
            if (fileSystemInfo is FileInfo)
            {
                if (itemProperty == "Folder")
                {
                    return false;
                }
            }
            else if (fileSystemInfo is DirectoryInfo)
            {
                if (itemProperty == "File")
                {
                    return false;
                }
            }

            return TestCondition(condition, fileSystemInfo);
        }

        private static bool TestCondition(Condition condition, FileSystemInfo fileSystemInfo)
        {
            var allowedStatuses = condition.Properties["state"].Split(';');
            if (allowedStatuses.Length == 0 || (allowedStatuses.Length == 1 && allowedStatuses[0].Length == 0))
            {
                return true;
            }

            var status = fileSystemInfo.CanBeVersionControlledItem()
                ? OverlayIconManager.GetStatus(fileSystemInfo)
                : StatusKind.Unversioned;

            /*if (status == "Unversioned") {
                PropertyDictionary pd = SvnClient.Instance.Client.PropGet("svn:ignore", Path.GetDirectoryName(fileName), Revision.Working, Recurse.None);
                if (pd != null) {
                    string shortFileName = Path.GetFileName(fileName);
                    foreach (Property p in pd.Values) {
                        using (StreamReader r = new StreamReader(new MemoryStream(p.Data))) {
                            string line;
                            while ((line = r.ReadLine()) != null) {
                                if (string.Equals(line, shortFileName, StringComparison.OrdinalIgnoreCase)) {
                                    status = "Ignored";
                                    break;
                                }
                            }
                        }
                    }
                }
            }*/

            //LoggingService.Debug("Status of " + fileName + " is " + status);
            return Array.IndexOf(allowedStatuses, status.ToString()) >= 0;
        }
    }
}