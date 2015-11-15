using System;
using System.Linq;
using MyLoadTest.LoadRunnerSvnAddin.Gui;

namespace MyLoadTest.LoadRunnerSvnAddin.Commands
{
    public class EditConflictsCommand : SubversionCommand
    {
        protected override void Run(string fileName)
        {
            SvnGuiWrapper.ConflictEditor(fileName, WatchProjects().Callback);
        }
    }
}