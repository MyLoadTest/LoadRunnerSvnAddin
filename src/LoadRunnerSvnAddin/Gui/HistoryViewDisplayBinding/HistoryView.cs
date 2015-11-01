// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using ICSharpCode.SharpDevelop.Gui;

namespace MyLoadTest.LoadRunnerSvnAddin.Gui.HistoryViewDisplayBinding
{
    public class HistoryView : AbstractSecondaryViewContent
    {
        private readonly HistoryViewPanel _historyViewPanel;

        public override object Control => _historyViewPanel;

        public HistoryView(IViewContent viewContent) : base(viewContent)
        {
            TabPageText = "${res:AddIns.Subversion.History}";
            _historyViewPanel = new HistoryViewPanel(viewContent.PrimaryFileName);
        }

        protected override void LoadFromPrimary()
        {
        }

        protected override void SaveToPrimary()
        {
        }

        public override bool IsViewOnly => true;
    }
}