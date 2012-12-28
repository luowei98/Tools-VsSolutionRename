#region File Descrption

// /////////////////////////////////////////////////////////////////////////////
// 
// Project: RobertLw.Tools.VsSolutionRename.RobertLw.Tools.VsSolutionRename
// File:    Parameters.cs
// 
// Create by Robert.L at 2012/10/22 12:19
// 
// /////////////////////////////////////////////////////////////////////////////

#endregion

using System.Collections.Generic;
using System.ComponentModel;
using CommandLine;
using CommandLine.Text;


namespace RobertLw.Tools.RobertLw.Tools.VsSolutionRename.CommandLine
{
    public sealed class Parameters : CommandLineOptionsBase
    {
        [Option("z", null, DefaultValue = false, HelpText = "Pack source files to zip file.")]
        public bool Zip { get; set; }

        [Option("x", null, DefaultValue = false, HelpText = "unpack zip file to destation path.")]
        public bool Extract { get; set; }

        [Option("r", null, DefaultValue = "", HelpText = "Solution new name to be rename.")]
        public string NewName { get; set; }

        [ValueList(typeof (List<string>))]
        [DefaultValue(null)]
        public IList<string> SourceDestination { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, current => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}
