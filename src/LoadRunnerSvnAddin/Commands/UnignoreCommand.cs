using System;
using System.IO;
using System.Linq;
using System.Text;
using ICSharpCode.Core;

namespace MyLoadTest.LoadRunnerSvnAddin.Commands
{
    public class UnignoreCommand : SubversionCommand
    {
        protected override void Run(string fileName)
        {
            using (var client = new SvnClientWrapper())
            {
                var propertyValue = client.GetPropertyValue(Path.GetDirectoryName(fileName), "svn:ignore");
                if (propertyValue != null)
                {
                    var watcher = WatchProjects();
                    var shortFileName = Path.GetFileName(fileName);
                    var b = new StringBuilder();
                    using (var r = new StringReader(propertyValue))
                    {
                        string line;
                        while ((line = r.ReadLine()) != null)
                        {
                            if (!string.Equals(line, shortFileName, StringComparison.OrdinalIgnoreCase))
                            {
                                b.AppendLine(line);
                            }
                        }
                    }
                    client.SetPropertyValue(Path.GetDirectoryName(fileName), "svn:ignore", b.ToString());
                    MessageService.ShowMessageFormatted("${res:AddIns.Subversion.ItemRemovedFromIgnoreList}", shortFileName);
                    watcher.Callback();
                }
            }
        }
    }
}