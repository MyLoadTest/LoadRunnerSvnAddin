using System;
using System.Linq;

namespace MyLoadTest.LoadRunnerSvnAddin
{
    public class NotificationEventArgs : EventArgs
    {
        public string Action;
        public string Kind;
        public string Path;
    }
}