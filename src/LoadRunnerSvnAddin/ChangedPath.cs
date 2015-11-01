using System;
using System.Linq;
using SharpSvn;

namespace MyLoadTest.LoadRunnerSvnAddin
{
    public class ChangedPath
    {
        public string Path;
        public string CopyFromPath;
        public long CopyFromRevision;

        /// <summary>
        /// change action ('A','D','R' or 'M')
        /// </summary>
        public SvnChangeAction Action;
    }
}