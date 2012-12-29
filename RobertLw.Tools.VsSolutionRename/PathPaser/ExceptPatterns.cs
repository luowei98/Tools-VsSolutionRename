#region File Descrption

// /////////////////////////////////////////////////////////////////////////////
// 
// Project: RobertLw.Tools.VsSolutionRename.RobertLw.Tools.VsSolutionRename
// File:    ExceptPatterns.cs
// 
// Create by Robert.L at 2012/11/03 11:16
// 
// /////////////////////////////////////////////////////////////////////////////

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;


namespace RobertLw.Tools.VsSolutionRename.PathPaser
{
    public class ExceptPatterns
    {
        public ExceptPatterns(string file = null)
        {
            file = file ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ignore.cfg");

            Files = new List<Regex>();
            Folders = new List<Regex>();

            string[] lines = File.ReadAllLines(file);
            foreach (string l in lines)
            {
                if (l.EndsWith("/"))
                {
                    string i = l.Substring(0, l.Length - 1);
                    Folders.Add(new Regex(ParsePattern(i)));
                }
                else
                {
                    Files.Add(new Regex(ParsePattern(l)));
                }
            }
        }

        public List<Regex> Files { get; private set; }
        public List<Regex> Folders { get; private set; }

        private static string ParsePattern(string pattern)
        {
            string p = pattern.Replace(".", @"\.").Replace("*", ".*");
            return "^" + p + "$";
        }
    }
}