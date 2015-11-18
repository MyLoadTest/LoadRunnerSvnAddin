using System;
using System.Linq;

namespace MyLoadTest.LoadRunnerSvnAddin
{
    public sealed class Status
    {
        public static readonly Status None = new Status(StatusKind.None, false);

        public Status(StatusKind textStatus, bool copied)
        {
            TextStatus = textStatus;
            Copied = copied;
        }

        public bool Copied
        {
            get;
        }

        public StatusKind TextStatus
        {
            get;
        }

        public override string ToString()
        {
            return $@"{{ {TextStatus}{(Copied ? " (Copied)" : string.Empty)} }}";
        }
    }
}