// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using ICSharpCode.Core;

namespace MyLoadTest.LoadRunnerSvnAddin
{
    public class AddInOptions
    {
        public static readonly string OptionsProperty = "MyLoadTest.LoadRunnerSvnAddin.Options";

        private static readonly Properties Properties;

        static AddInOptions()
        {
            Properties = PropertyService.Get(OptionsProperty, new Properties());
        }

        public static bool AutomaticallyAddFiles
        {
            get
            {
                return Properties.Get("AutomaticallyAddFiles", true);
            }

            set
            {
                Properties.Set("AutomaticallyAddFiles", value);
            }
        }

        public static bool AutomaticallyDeleteFiles
        {
            get
            {
                return Properties.Get("AutomaticallyDeleteFiles", true);
            }

            set
            {
                Properties.Set("AutomaticallyDeleteFiles", value);
            }
        }

        public static bool AutomaticallyRenameFiles
        {
            get
            {
                return Properties.Get("AutomaticallyRenameFiles", true);
            }

            set
            {
                Properties.Set("AutomaticallyRenameFiles", value);
            }
        }

        public static bool AutomaticallyReloadProject
        {
            get
            {
                return Properties.Get("AutomaticallyReloadProject", true);
            }

            set
            {
                Properties.Set("AutomaticallyReloadProject", value);
            }
        }

        public static bool UseHistoryDisplayBinding
        {
            get
            {
                return Properties.Get("UseHistoryDisplayBinding", true);
            }

            set
            {
                Properties.Set("UseHistoryDisplayBinding", value);
            }
        }
    }
}