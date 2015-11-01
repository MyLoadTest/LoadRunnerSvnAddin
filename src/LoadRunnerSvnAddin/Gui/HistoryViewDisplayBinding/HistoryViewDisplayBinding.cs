﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Linq;
using ICSharpCode.SharpDevelop;

namespace MyLoadTest.LoadRunnerSvnAddin.Gui.HistoryViewDisplayBinding
{
    public class HistoryViewDisplayBinding : ISecondaryDisplayBinding
    {
        /// <summary>
        /// When you return true for this property, the CreateSecondaryViewContent method
        /// is called again after the LoadSolutionProjects thread has finished.
        /// </summary>
        public bool ReattachWhenParserServiceIsReady => false;

        public ICSharpCode.SharpDevelop.Gui.IViewContent[] CreateSecondaryViewContent(
            ICSharpCode.SharpDevelop.Gui.IViewContent viewContent)
        {
            return new ICSharpCode.SharpDevelop.Gui.IViewContent[] { new HistoryView(viewContent) };
        }

        public bool CanAttachTo(ICSharpCode.SharpDevelop.Gui.IViewContent content)
        {
            if (!AddInOptions.UseHistoryDisplayBinding)
            {
                return false;
            }

            var file = content.PrimaryFile;
            if (file == null || file.IsUntitled || !File.Exists(file.FileName))
            {
                return false;
            }

            return SvnClientWrapper.IsInSourceControl(file.FileName);
        }
    }
}