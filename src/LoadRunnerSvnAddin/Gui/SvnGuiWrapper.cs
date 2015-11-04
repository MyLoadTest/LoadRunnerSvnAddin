﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using Microsoft.Win32;

namespace MyLoadTest.LoadRunnerSvnAddin.Gui
{
    /// <summary>
    /// Wraps commands opening a dialog window.
    /// The current implementation launches TortoiseSVN.
    /// </summary>
    public static class SvnGuiWrapper
    {
        private static string GetPathFromRegistry(RegistryHive hive, string valueName)
        {
            var view = Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Default;
            using (var baseKey = RegistryKey.OpenBaseKey(hive, view))
            {
                using (var key = baseKey.OpenSubKey("SOFTWARE\\TortoiseSVN"))
                {
                    return key?.GetValue(valueName) as string;
                }
            }
        }

        private static string GetPathFromRegistry(string valueName)
        {
            return GetPathFromRegistry(RegistryHive.CurrentUser, valueName)
                ?? GetPathFromRegistry(RegistryHive.LocalMachine, valueName);
        }

        private static void Proc(string command, string fileName, MethodInvoker callback)
        {
            Proc(command, fileName, callback, null);
        }

        private static void Proc(string command, string fileName, MethodInvoker callback, string argument)
        {
            var path = GetPathFromRegistry("ProcPath");
            if (path == null)
            {
                using (var form = new TortoiseSvnNotFoundForm())
                {
                    form.ShowDialog(WorkbenchSingleton.MainWin32Window);
                }

                return;
            }

            try
            {
                var arguments = new StringBuilder();
                arguments.Append("/command:");
                arguments.Append(command);
                if (fileName != null)
                {
                    arguments.Append(" /notempfile ");
                    arguments.Append(" /path:\"");
                    arguments.Append(fileName);
                    arguments.Append('"');
                }
                if (argument != null)
                {
                    arguments.Append(' ');
                    arguments.Append(argument);
                }

                var p = new Process
                {
                    StartInfo =
                    {
                        FileName = path,
                        Arguments = arguments.ToString(),
                        UseShellExecute = false
                        ////RedirectStandardError = true,
                        ////RedirectStandardOutput = true
                    },
                    EnableRaisingEvents = true
                };

                p.Exited += delegate
                {
                    p.Dispose();
                    callback?.Invoke();
                };

                ////p.OutputDataReceived += delegate(object sender, DataReceivedEventArgs e) {
                ////	SvnClient.Instance.SvnCategory.AppendText(e.Data);
                ////};
                ////p.ErrorDataReceived += delegate(object sender, DataReceivedEventArgs e) {
                ////	SvnClient.Instance.SvnCategory.AppendText(e.Data);
                ////};

                p.Start();
            }
            catch (Exception ex)
            {
                MessageService.ShowError(ex.Message);
            }
        }

        public static void ShowCheckoutDialog(MethodInvoker callback)
        {
            Proc("checkout", null, callback);
        }

        public static void ShowExportDialog(MethodInvoker callback)
        {
            Proc("export", null, callback);
        }

        public static void Update(string fileName, MethodInvoker callback)
        {
            Proc("update", fileName, callback);
        }

        public static void ApplyPatch(string fileName, MethodInvoker callback)
        {
            //Proc("applypatch", fileName, callback);
            // TODO: Applying patches is not implemented.
            MessageService.ShowMessage("Applying patches is not implemented.");
        }

        public static void CreatePatch(string fileName, MethodInvoker callback)
        {
            Proc("createpatch", fileName, callback);
        }

        public static void Revert(string fileName, MethodInvoker callback)
        {
            Proc("revert", fileName, callback);
        }

        public static void Commit(string fileName, MethodInvoker callback)
        {
            Proc("commit", fileName, callback);
        }

        public static void Add(string fileName, MethodInvoker callback)
        {
            Proc("add", fileName, callback);
        }

        public static void Ignore(string fileName, MethodInvoker callback)
        {
            Proc("ignore", fileName, callback);
        }

        public static void ShowSvnHelp()
        {
            Proc("help", null, null);
        }

        public static void ShowSvnSettings()
        {
            Proc("settings", null, null);
        }

        public static void ShowSvnAbout()
        {
            Proc("about", null, null);
        }

        public static void Diff(string fileName, MethodInvoker callback)
        {
            Proc("diff", fileName, callback);
        }

        public static void ConflictEditor(string fileName, MethodInvoker callback)
        {
            Proc("conflicteditor", fileName, callback);
        }

        public static void ResolveConflict(string fileName, MethodInvoker callback)
        {
            Proc("resolve", fileName, callback);
        }

        public static void ShowLog(string fileName, MethodInvoker callback)
        {
            Proc("log", fileName, callback);
        }

        public static void Cleanup(string fileName, MethodInvoker callback)
        {
            Proc("cleanup", fileName, callback);
        }

        public static void RevisionGraph(string fileName, MethodInvoker callback)
        {
            Proc("revisiongraph", fileName, callback);
        }

        public static void RepoStatus(string fileName, MethodInvoker callback)
        {
            Proc("repostatus", fileName, callback);
        }

        public static void RepoBrowser(string fileName, MethodInvoker callback)
        {
            Proc("repobrowser", fileName, callback);
        }

        public static void UpdateToRevision(string fileName, MethodInvoker callback)
        {
            Proc("update", fileName, callback, "/rev");
        }

        public static void Export(string fileName, MethodInvoker callback)
        {
            Proc("export", fileName, callback);
        }

        public static void Branch(string fileName, MethodInvoker callback)
        {
            Proc("copy", fileName, callback);
        }

        public static void Lock(string fileName, MethodInvoker callback)
        {
            Proc("lock", fileName, callback);
        }

        public static void Blame(string fileName, MethodInvoker callback)
        {
            Proc("blame", fileName, callback);
        }

        public static void Switch(string fileName, MethodInvoker callback)
        {
            Proc("switch", fileName, callback);
        }

        public static void Merge(string fileName, MethodInvoker callback)
        {
            Proc("merge", fileName, callback);
        }

        public static void Relocate(string fileName, MethodInvoker callback)
        {
            Proc("relocate", fileName, callback);
        }
    }
}