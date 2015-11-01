// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using ICSharpCode.SharpDevelop.Gui;

namespace MyLoadTest.LoadRunnerSvnAddin
{
    /// <summary>
    /// Output pad category for subversion.
    /// </summary>
    public static class SvnMessageView
    {
        private static MessageViewCategory _category;

        /// <summary>
        /// Gets the subversion message view category.
        /// </summary>
        public static MessageViewCategory Category
        {
            get
            {
                if (_category == null)
                {
                    MessageViewCategory.Create(ref _category, "Subversion");
                }
                return _category;
            }
        }

        /// <summary>
        /// Appends a line to the svn message view.
        /// </summary>
        public static void AppendLine(string text)
        {
            Category.AppendLine(text);
        }

        public static void HandleNotifications(SvnClientWrapper client)
        {
            client.Notify += delegate (object sender, NotificationEventArgs e)
            {
                AppendLine(e.Kind + e.Action + " " + e.Path);
            };
            AsynchronousWaitDialog waitDialog = null;
            client.OperationStarted += delegate (object sender, SubversionOperationEventArgs e)
            {
                if (waitDialog == null)
                {
                    waitDialog = AsynchronousWaitDialog.ShowWaitDialog("svn " + e.Operation);

                    //					waitDialog.Cancelled += delegate {
                    //						client.Cancel();
                    //					};
                }
            };
            client.OperationFinished += delegate
            {
                if (waitDialog != null)
                {
                    waitDialog.Dispose();
                    waitDialog = null;
                }
            };
        }
    }
}