﻿#region File Descrption

// /////////////////////////////////////////////////////////////////////////////
// 
// Project: RobertLw.Tools.VsSolutionRename.RobertLw.Tools.VsSolutionRename
// File:    Operator.cs
// 
// Create by Robert.L at 2012/12/26 16:29
// 
// /////////////////////////////////////////////////////////////////////////////

#endregion

using System;
using System.IO;
using System.Linq;
using System.Text;
using Ionic.Zip;
using RobertLw.Tools.VsSolutionRename.CommandLine;
using RobertLw.Tools.VsSolutionRename.PathPaser;


namespace RobertLw.Tools.VsSolutionRename
{
    public class Operator
    {
        private readonly string destinationPath;
        private readonly string destinationZip;
        private readonly string sourceZip;
        private string sourcePath;

        public Operator(ParameterWrap parameter)
        {
            Parameter = parameter;

            if (Parameter.IsZipSource)
            {
                sourceZip = Parameter.Source;
            }
            else
            {
                sourcePath = Parameter.Source;
            }
            if (Parameter.IsZipDestination)
            {
                destinationZip = Parameter.Destination;
                if (File.Exists(destinationZip)) File.Delete(destinationZip);

                destinationPath = GetRandomTempPath();
                if (Directory.Exists(destinationPath)) Directory.Delete(destinationPath, true);
            }
            else
            {
                destinationPath = Parameter.Destination;
                if (Directory.Exists(destinationPath)) throw new Exception("Destination path is exists.");
            }
        }

        public ParameterWrap Parameter { get; private set; }

        public void Doit()
        {
            Unzip();
            CleanCopyRename();
            Zip();
        }

        private void Unzip()
        {
            if (!Parameter.IsZipSource) return;

            string dir = GetRandomTempPath();

            using (ZipFile zip = ZipFile.Read(sourceZip))
            {
                zip.ExtractAll(dir);
            }

            sourcePath = dir;
        }

        private void CleanCopyRename()
        {
            var src = new Enumerator(sourcePath);
            string oldname = Parameter.SourceSolutionName;
            string newname = Parameter.DestinationName;
            if (newname.ToLower().EndsWith(".zip"))
            {
                newname = newname.Substring(0, newname.Length - 4);
            }

            if (Directory.Exists(destinationPath))
            {
                throw new Exception("destination directory is exist.");
            }

            foreach (string d in src.Directories())
            {
                string np = src.RelativePath(d);
                if (Parameter.NeedRename)
                {
                    np = np.Replace(oldname, newname);
                }
                Directory.CreateDirectory(Path.Combine(destinationPath, np));
            }

            foreach (string f in src.Files())
            {
                string nf = src.RelativePath(f);
                if (Parameter.NeedRename)
                {
                    nf = nf.Replace(oldname, newname);
                }

                string fp = Path.Combine(destinationPath, nf);
                File.Copy(f, fp);

                if (Parameter.NeedRename)
                {
                    File.WriteAllLines(
                        fp,
                        File.ReadAllLines(fp).Select(l => l.Replace(oldname, newname)),
                        Encoding.UTF8
                        );
                }
            }
        }

        private void Zip()
        {
            if (!Parameter.IsZipDestination) return;

            using (var zip = new ZipFile(destinationZip))
            {
                var src = new Enumerator(destinationPath);

                foreach (string file in src.Files())
                {
                    string p = Path.GetDirectoryName(file);
                    zip.AddFile(file, src.RelativePath(p));
                }

                zip.Save();
            }
        }

        private string GetRandomTempPath()
        {
            string p = Path.GetTempPath();
            string f = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) ??
                       DateTime.Now.ToString("yyyyMMddhhmmss");

            return Path.Combine(p, f);
        }
    }
}