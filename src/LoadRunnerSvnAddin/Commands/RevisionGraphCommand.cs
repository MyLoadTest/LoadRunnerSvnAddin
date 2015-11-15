using System;
using System.Linq;
using MyLoadTest.LoadRunnerSvnAddin.Gui;

namespace MyLoadTest.LoadRunnerSvnAddin.Commands
{
    public class RevisionGraphCommand : SubversionCommand
    {
        protected override void Run(string fileName)
        {
            SvnGuiWrapper.RevisionGraph(fileName, null);
        }
    }
}