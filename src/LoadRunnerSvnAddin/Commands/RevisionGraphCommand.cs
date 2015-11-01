using System;
using System.Linq;
using MyLoadTest.LoadRunnerSvnAddin.Gui;

namespace MyLoadTest.LoadRunnerSvnAddin.Commands
{
    public class RevisionGraphCommand : SubversionCommand
    {
        protected override void Run(string filename)
        {
            SvnGuiWrapper.RevisionGraph(filename, null);
        }
    }
}