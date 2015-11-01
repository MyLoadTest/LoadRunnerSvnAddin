using System;
using System.Linq;

namespace MyLoadTest.LoadRunnerSvnAddin
{
    public enum StatusKind
    {
        None,
        Added,
        Conflicted,
        Deleted,
        Modified,
        Replaced,
        External,
        Ignored,
        Incomplete,
        Merged,
        Missing,
        Obstructed,
        Normal,
        Unversioned
    }
}