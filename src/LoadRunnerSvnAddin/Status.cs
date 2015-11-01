using System;
using System.Linq;

namespace MyLoadTest.LoadRunnerSvnAddin
{
    public class Status
    {
        public bool Copied
        { get; set; }

        public StatusKind TextStatus
        { get; set; }
    }
}