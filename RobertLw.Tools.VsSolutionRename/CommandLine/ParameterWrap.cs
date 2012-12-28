#region File Descrption
// /////////////////////////////////////////////////////////////////////////////
// 
// Project: RobertLw.Tools.VsSolutionRename.RobertLw.Tools.VsSolutionRename
// File:    ParameterWrap.cs
// 
// Create by Robert.L at 2012/10/26 10:47
// 
// /////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.IO;
using System.Linq;
using CommandLine;
using Ionic.Zip;


namespace RobertLw.Tools.RobertLw.Tools.VsSolutionRename.CommandLine
{
    public class ParameterWrap
    {
        public bool IsGood { get; private set; }

        private readonly TextWriter message;
        public string Message
        {
            get { return message.ToString(); }
        }

        private readonly Parameters parameters;

        public string SourceSolutionName { get; private set; }

        public string SourcePath { get; private set; }
        public string SourceName { get; private set; }
        public string Source { get { return Path.Combine(SourcePath, SourceName); } }

        public string DestinationPath { get; private set; }
        public string DestinationName { get; private set; }
        public string Destination { get { return Path.Combine(DestinationPath, DestinationName); } }

        public bool IsZipSource { get; private set; }
        public bool IsZipDestination { get; private set;  }

        public bool NeedRename { get; private set; }
        public string NewName { get; private set; }

        public ZipFlag PackageFlag { get; private set; }

        public ParameterWrap(string[] args, TextWriter sw = null)
        {
            message = sw ?? new StringWriter();
            parameters = new Parameters();
            var parser = new CommandLineParser(new CommandLineParserSettings(message));

            if (!parser.ParseArguments(args, parameters)) return;

            if (parameters.SourceDestination == null || parameters.SourceDestination.Count == 0)
            {
                message.Write(parameters.GetUsage());
                return;
            }

            if (parameters.Zip && parameters.Extract)
            {
                message.Write(parameters.GetUsage());
                return;
            }
            
            if (parameters.Zip)
            {
                PackageFlag = ZipFlag.Zip;
            }
            else if (parameters.Extract)
            {
                PackageFlag = ZipFlag.Unzip;
            }
            else
            {
                PackageFlag = ZipFlag.None;
            }

            NewName = parameters.NewName;
            NeedRename = !string.IsNullOrEmpty(NewName);

            IsGood = GetSource() && GetDestination();
        }

        private bool GetSource()
        {
            var src = parameters.SourceDestination[0];

            var path = Path.GetFullPath(src);
            var pare = new DirectoryInfo(path).Parent;
            if (pare == null)
            {
                message.WriteLine(parameters.GetUsage());
                return false;
            }

            SourcePath = pare.FullName;
            SourceName = Path.GetFileName(path);

            var isdir = Directory.Exists(path);
            var isfile = File.Exists(path);

            if (!isdir && !isfile)
            {
                message.WriteLine(parameters.GetUsage());
                return false;
            }

            if (isfile)
            {
                if (!path.ToLower().EndsWith(".zip"))
                {
                    message.WriteLine(parameters.GetUsage());
                    return false;
                }

                IsZipSource = true;

                using (var zip = ZipFile.Read(Source))
                {
                    var f = zip.SingleOrDefault(i => !i.IsDirectory && i.FileName.ToLower().EndsWith(".sln"));
                    if (f == null)
                    {
                        message.WriteLine(parameters.GetUsage());
                        return false;
                    }
                    SourceSolutionName = f.FileName.Substring(0, f.FileName.Length - 4);
                }
            }
            else
            {
                var f = Directory.GetFiles(Source).SingleOrDefault(i => i.ToLower().EndsWith(".sln"));
                if (f == null)
                {
                    message.WriteLine(parameters.GetUsage());
                    return false;
                }
                SourceSolutionName = Path.GetFileNameWithoutExtension(f);
            }

            return true;
        }

        private bool GetDestination()
        {
            if (parameters.SourceDestination.Count == 1)
            {
                DestinationPath = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\');
            }
            else
            {
                var p = parameters.SourceDestination[1].TrimEnd('\\');
                DestinationPath = Path.GetFullPath(p);
            }

            IsZipDestination = PackageFlag == ZipFlag.Zip || (IsZipSource && PackageFlag == ZipFlag.None);

            DestinationName = NeedRename ? NewName : SourceSolutionName;
            if (IsZipDestination && !DestinationName.ToLower().EndsWith(".zip"))
            {
                DestinationName += ".zip";
            }

            if (SourcePath == DestinationPath && SourceName == DestinationName)
            {
                if (IsZipDestination)
                {
                    DestinationName = DestinationName.Insert(DestinationName.Length - 4, "New");
                }
                else
                {
                    DestinationName = DestinationName + "New";
                }
            }

            return true;
        }
    }

    public enum ZipFlag
    {
        None,
        Zip,
        Unzip,
    }
}
