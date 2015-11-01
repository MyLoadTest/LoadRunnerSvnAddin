// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace MyLoadTest.LoadRunnerSvnAddin.Gui
{
	public class TortoiseSvnNotFoundForm : ToolNotFoundDialog
	{
		public TortoiseSvnNotFoundForm()
			: base(StringParser.Parse("${res:AddIns.Subversion.TortoiseSVNRequired}"), "http://tortoisesvn.net/", null)
		{
		}
	}
}
