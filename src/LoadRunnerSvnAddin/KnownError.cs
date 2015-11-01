using System;
using System.Linq;
using SharpSvn;

namespace MyLoadTest.LoadRunnerSvnAddin
{
    public enum KnownError
    {
        FileNotFound = SvnErrorCode.SVN_ERR_FS_NOT_FOUND,
        CannotDeleteFileWithLocalModifications = SvnErrorCode.SVN_ERR_CLIENT_MODIFIED,
        CannotDeleteFileNotUnderVersionControl = SvnErrorCode.SVN_ERR_UNVERSIONED_RESOURCE
    }
}