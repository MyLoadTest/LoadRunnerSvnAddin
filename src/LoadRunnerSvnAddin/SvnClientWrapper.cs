// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using MyLoadTest.LoadRunnerSvnAddin.Gui.ProjectBrowserVisitor;
using SharpSvn;
using SharpSvn.UI;

namespace MyLoadTest.LoadRunnerSvnAddin
{
    /// <summary>
    /// A wrapper around the subversion library.
    /// </summary>
    public sealed class SvnClientWrapper : IDisposable
    {
        #region status->string conversion

        private static string GetKindString(SvnNodeKind kind)
        {
            switch (kind)
            {
                case SvnNodeKind.Directory:
                    return "directory ";

                case SvnNodeKind.File:
                    return "file ";

                default:
                    return null;
            }
        }

        public static string GetActionString(SvnChangeAction action)
        {
            switch (action)
            {
                case SvnChangeAction.Add:
                    return GetActionString(SvnNotifyAction.CommitAdded);

                case SvnChangeAction.Delete:
                    return GetActionString(SvnNotifyAction.CommitDeleted);

                case SvnChangeAction.Modify:
                    return GetActionString(SvnNotifyAction.CommitModified);

                case SvnChangeAction.Replace:
                    return GetActionString(SvnNotifyAction.CommitReplaced);

                default:
                    return "unknown";
            }
        }

        private static string GetActionString(SvnNotifyAction action)
        {
            switch (action)
            {
                case SvnNotifyAction.Add:
                case SvnNotifyAction.CommitAdded:
                    return "added";

                case SvnNotifyAction.Copy:
                    return "copied";

                case SvnNotifyAction.Delete:
                case SvnNotifyAction.UpdateDelete:
                case SvnNotifyAction.CommitDeleted:
                    return "deleted";

                case SvnNotifyAction.Restore:
                    return "restored";

                case SvnNotifyAction.Revert:
                    return "reverted";

                case SvnNotifyAction.RevertFailed:
                    return "revert failed";

                case SvnNotifyAction.Resolved:
                    return "resolved";

                case SvnNotifyAction.Skip:
                    return "skipped";

                case SvnNotifyAction.UpdateUpdate:
                    return "updated";

                case SvnNotifyAction.UpdateExternal:
                    return "updated external";

                case SvnNotifyAction.CommitModified:
                    return "modified";

                case SvnNotifyAction.CommitReplaced:
                    return "replaced";

                case SvnNotifyAction.LockFailedLock:
                    return "lock failed";

                case SvnNotifyAction.LockFailedUnlock:
                    return "unlock failed";

                case SvnNotifyAction.LockLocked:
                    return "locked";

                case SvnNotifyAction.LockUnlocked:
                    return "unlocked";

                default:
                    return "unknown";
            }
        }

        #endregion

        #region Cancel support

        private bool _cancel;

        public void Cancel()
        {
            _cancel = true;
        }

        private void client_Cancel(object sender, SvnCancelEventArgs e)
        {
            e.Cancel = _cancel;
        }

        #endregion

        private SvnClient _client;

        public SvnClientWrapper()
        {
            Debug("SVN: Create SvnClient instance");

            _client = new SvnClient();
            _client.Notify += client_Notify;
            _client.Cancel += client_Cancel;
        }

        public void Dispose()
        {
            _client?.Dispose();
            _client = null;
        }

        #region Authorization

        private bool _authorizationEnabled;
        private bool _allowInteractiveAuthorization;

        public void AllowInteractiveAuthorization()
        {
            CheckNotDisposed();
            if (!_allowInteractiveAuthorization)
            {
                _allowInteractiveAuthorization = true;
                SvnUI.Bind(_client, WorkbenchSingleton.MainWin32Window);
            }
        }

        private void OpenAuth()
        {
            if (_authorizationEnabled)
                return;
            _authorizationEnabled = true;
        }

        #endregion

        #region Notifications

        public event EventHandler<SubversionOperationEventArgs> OperationStarted;

        public event EventHandler OperationFinished;

        public event EventHandler<NotificationEventArgs> Notify;

        private void client_Notify(object sender, SvnNotifyEventArgs e)
        {
            Notify?.Invoke(
                this,
                new NotificationEventArgs
                {
                    Action = GetActionString(e.Action),
                    Kind = GetKindString(e.NodeKind),
                    Path = e.Path
                });
        }

        #endregion

        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        private static void Debug(string text)
        {
            LoggingService.Debug(text);
        }

        private void CheckNotDisposed()
        {
            if (_client == null)
                throw new ObjectDisposedException(GetType().FullName);
        }

        private void BeforeWriteOperation(string operationName)
        {
            BeforeReadOperation(operationName);
            ClearStatusCache();
        }

        private void BeforeReadOperation(string operationName)
        {
            // before any subversion operation, ensure the object is not disposed
            // and register authorization if necessary
            CheckNotDisposed();
            OpenAuth();
            _cancel = false;

            OperationStarted?.Invoke(this, new SubversionOperationEventArgs { Operation = operationName });
        }

        private void AfterOperation()
        {
            // after any subversion operation, clear the memory pool
            OperationFinished?.Invoke(this, EventArgs.Empty);
        }

        // We cache SingleStatus results because WPF asks our Condition several times
        // per menu entry; and it would be extremely slow to hit the hard disk every time (SD2-1672)
        private readonly Dictionary<string, Status> _statusCache = new Dictionary<string, Status>(StringComparer.OrdinalIgnoreCase);

        public void ClearStatusCache()
        {
            CheckNotDisposed();
            _statusCache.Clear();
        }

        public Status SingleStatus(string filename)
        {
            filename = FileUtility.NormalizePath(filename);
            Status result;
            if (_statusCache.TryGetValue(filename, out result))
            {
                Debug("SVN: SingleStatus(" + filename + ") = cached " + result.TextStatus);
                return result;
            }
            Debug("SVN: SingleStatus(" + filename + ")");
            BeforeReadOperation("stat");
            try
            {
                var args = new SvnStatusArgs
                {
                    Revision = SvnRevision.Working,
                    RetrieveAllEntries = true,
                    RetrieveIgnoredEntries = true,
                    Depth = SvnDepth.Empty
                };
                _client.Status(
                    filename, args,
                    delegate (object sender, SvnStatusEventArgs e)
                    {
                        Debug("SVN: SingleStatus.callback(" + e.FullPath + "," + e.LocalContentStatus + ")");
                        System.Diagnostics.Debug.Assert(filename.Equals(e.FullPath, StringComparison.OrdinalIgnoreCase));
                        result = new Status
                        {
                            Copied = e.LocalCopied,
                            TextStatus = ToStatusKind(e.LocalContentStatus)
                        };
                    }
                );
                if (result == null)
                {
                    result = new Status
                    {
                        TextStatus = StatusKind.None
                    };
                }
                _statusCache.Add(filename, result);
                return result;
            }
            catch (SvnException ex)
            {
                throw new SvnClientException(ex);
            }
            finally
            {
                AfterOperation();
            }
        }

        private static SvnDepth ConvertDepth(Recurse recurse)
        {
            if (recurse == Recurse.Full)
                return SvnDepth.Infinity;
            else
                return SvnDepth.Empty;
        }

        public void Add(string filename, Recurse recurse)
        {
            Debug("SVN: Add(" + filename + ", " + recurse + ")");
            BeforeWriteOperation("add");
            try
            {
                _client.Add(filename, ConvertDepth(recurse));
            }
            catch (SvnException ex)
            {
                throw new SvnClientException(ex);
            }
            finally
            {
                AfterOperation();
            }
        }

        public string GetPropertyValue(string fileName, string propertyName)
        {
            Debug("SVN: GetPropertyValue(" + fileName + ", " + propertyName + ")");
            BeforeReadOperation("propget");
            try
            {
                string propertyValue;
                if (_client.GetProperty(fileName, propertyName, out propertyValue))
                    return propertyValue;
                else
                    return null;
            }
            catch (SvnException ex)
            {
                throw new SvnClientException(ex);
            }
            finally
            {
                AfterOperation();
            }
        }

        public void SetPropertyValue(string fileName, string propertyName, string newPropertyValue)
        {
            Debug("SVN: SetPropertyValue(" + fileName + ", " + propertyName + ", " + newPropertyValue + ")");
            BeforeWriteOperation("propset");
            try
            {
                if (newPropertyValue != null)
                    _client.SetProperty(fileName, propertyName, newPropertyValue);
                else
                    _client.DeleteProperty(fileName, propertyName);
            }
            catch (SvnException ex)
            {
                throw new SvnClientException(ex);
            }
            finally
            {
                AfterOperation();
            }
        }

        public void Delete(string[] files, bool force)
        {
            Debug("SVN: Delete(" + string.Join(",", files) + ", " + force + ")");
            BeforeWriteOperation("delete");
            try
            {
                _client.Delete(
                    files,
                    new SvnDeleteArgs
                    {
                        Force = force
                    });
            }
            catch (SvnException ex)
            {
                throw new SvnClientException(ex);
            }
            finally
            {
                AfterOperation();
            }
        }

        public void Revert(string[] files, Recurse recurse)
        {
            Debug("SVN: Revert(" + string.Join(",", files) + ", " + recurse + ")");
            BeforeWriteOperation("revert");
            try
            {
                _client.Revert(
                    files,
                    new SvnRevertArgs
                    {
                        Depth = ConvertDepth(recurse)
                    });
            }
            catch (SvnException ex)
            {
                throw new SvnClientException(ex);
            }
            finally
            {
                AfterOperation();
            }
        }

        public void Move(string from, string to, bool force)
        {
            Debug("SVN: Move(" + from + ", " + to + ", " + force + ")");
            BeforeWriteOperation("move");
            try
            {
                _client.Move(
                    from, to,
                    new SvnMoveArgs
                    {
                        Force = force
                    });
            }
            catch (SvnException ex)
            {
                throw new SvnClientException(ex);
            }
            finally
            {
                AfterOperation();
            }
        }

        public void Copy(string from, string to)
        {
            Debug("SVN: Copy(" + from + ", " + to);
            BeforeWriteOperation("copy");
            try
            {
                _client.Copy(from, to);
            }
            catch (SvnException ex)
            {
                throw new SvnClientException(ex);
            }
            finally
            {
                AfterOperation();
            }
        }

        public void AddToIgnoreList(string directory, params string[] filesToIgnore)
        {
            Debug("SVN: AddToIgnoreList(" + directory + ", " + string.Join(",", filesToIgnore) + ")");
            var propertyValue = GetPropertyValue(directory, "svn:ignore");
            var b = new StringBuilder();
            if (propertyValue != null)
            {
                using (var r = new StringReader(propertyValue))
                {
                    string line;
                    while ((line = r.ReadLine()) != null)
                    {
                        if (line.Length > 0)
                        {
                            b.AppendLine(line);
                        }
                    }
                }
            }
            foreach (var file in filesToIgnore)
                b.AppendLine(file);
            SetPropertyValue(directory, "svn:ignore", b.ToString());
        }

        public void Log(string[] paths, Revision start, Revision end,
                        int limit, bool discoverChangePaths, bool strictNodeHistory,
                        Action<LogMessage> logMessageReceiver)
        {
            Debug("SVN: Log({" + string.Join(",", paths) + "}, " + start + ", " + end +
                  ", " + limit + ", " + discoverChangePaths + ", " + strictNodeHistory + ")");
            BeforeReadOperation("log");
            try
            {
                _client.Log(
                    paths,
                    new SvnLogArgs
                    {
                        Start = start,
                        End = end,
                        Limit = limit,
                        RetrieveChangedPaths = discoverChangePaths,
                        StrictNodeHistory = strictNodeHistory
                    },
                    delegate (object sender, SvnLogEventArgs e)
                    {
                        try
                        {
                            Debug("SVN: Log: Got revision " + e.Revision);
                            var msg = new LogMessage
                            {
                                Revision = e.Revision,
                                Author = e.Author,
                                Date = e.Time,
                                Message = e.LogMessage
                            };
                            if (discoverChangePaths)
                            {
                                msg.ChangedPaths = new List<ChangedPath>();
                                foreach (var entry in e.ChangedPaths)
                                {
                                    msg.ChangedPaths.Add(new ChangedPath
                                    {
                                        Path = entry.Path,
                                        CopyFromPath = entry.CopyFromPath,
                                        CopyFromRevision = entry.CopyFromRevision,
                                        Action = entry.Action
                                    });
                                }
                            }
                            logMessageReceiver(msg);
                        }
                        catch (Exception ex)
                        {
                            MessageService.ShowException(ex);
                        }
                    }
                );
                Debug("SVN: Log finished");
            }
            catch (SvnOperationCanceledException)
            {
                // allow cancel without exception
            }
            catch (SvnException ex)
            {
                throw new SvnClientException(ex);
            }
            finally
            {
                AfterOperation();
            }
        }

        public Stream OpenBaseVersion(string fileName)
        {
            var stream = new MemoryStream();
            if (!this._client.Write(fileName, stream, new SvnWriteArgs { Revision = SvnRevision.Base, ThrowOnError = false }))
                return null;
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        public Stream OpenCurrentVersion(string fileName)
        {
            var stream = new MemoryStream();
            if (!this._client.Write(fileName, stream, new SvnWriteArgs { Revision = SvnRevision.Working }))
                return null;
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        public static bool IsInSourceControl(string fileName)
        {
            if (!LocalHelper.CanBeVersionControlledFile(fileName))
            {
                return false;
            }

            var status = OverlayIconManager.GetStatus(new FileInfo(fileName));
            return status != StatusKind.None && status != StatusKind.Unversioned && status != StatusKind.Ignored;
        }

        private static StatusKind ToStatusKind(SvnStatus kind)
        {
            switch (kind)
            {
                case SvnStatus.Added:
                    return StatusKind.Added;

                case SvnStatus.Conflicted:
                    return StatusKind.Conflicted;

                case SvnStatus.Deleted:
                    return StatusKind.Deleted;

                case SvnStatus.External:
                    return StatusKind.External;

                case SvnStatus.Ignored:
                    return StatusKind.Ignored;

                case SvnStatus.Incomplete:
                    return StatusKind.Incomplete;

                case SvnStatus.Merged:
                    return StatusKind.Merged;

                case SvnStatus.Missing:
                    return StatusKind.Missing;

                case SvnStatus.Modified:
                    return StatusKind.Modified;

                case SvnStatus.Normal:
                    return StatusKind.Normal;

                case SvnStatus.NotVersioned:
                    return StatusKind.Unversioned;

                case SvnStatus.Obstructed:
                    return StatusKind.Obstructed;

                case SvnStatus.Replaced:
                    return StatusKind.Replaced;

                default:
                    return StatusKind.None;
            }
        }
    }
}