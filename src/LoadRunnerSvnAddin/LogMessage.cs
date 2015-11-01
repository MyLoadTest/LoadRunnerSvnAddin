using System;
using System.Collections.Generic;
using System.Linq;

namespace MyLoadTest.LoadRunnerSvnAddin
{
    public class LogMessage
    {
        public long Revision;
        public string Author;
        public DateTime Date;
        public string Message;

        public List<ChangedPath> ChangedPaths;
    }
}