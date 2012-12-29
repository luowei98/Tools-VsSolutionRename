#region File Descrption

// /////////////////////////////////////////////////////////////////////////////
// 
// Project: RobertLw.Tools.VsSolutionRename.RobertLw.Tools.VsSolutionRename
// File:    Enumerator.cs
// 
// Create by Robert.L at 2012/11/02 12:51
// 
// /////////////////////////////////////////////////////////////////////////////

#endregion

using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace RobertLw.Tools.VsSolutionRename.PathPaser
{
    public class Enumerator
    {
        public Enumerator(string directory)
        {
            BaseDirectory = directory;
            Excepts = new ExceptPatterns();
        }

        public string BaseDirectory { get; private set; }
        public ExceptPatterns Excepts { get; private set; }

        public IEnumerable<string> Files(string directory = null)
        {
            directory = directory ?? BaseDirectory;

            IEnumerable<string> subfiles = from dir in Directories(directory)
                                           from file in GetFiles(dir)
                                           select file;
            foreach (string file in subfiles)
            {
                yield return file;
            }

            foreach (string file in GetFiles(directory))
            {
                yield return file;
            }
        }

        private IEnumerable<string> GetFiles(string directory)
        {
            return from file in Directory.GetFiles(directory)
                   let f = Path.GetFileName(file)
                   where f != null
                   where !Excepts.Files.Any(p => p.IsMatch(f))
                   select file;
        }

        public IEnumerable<string> Directories(string directory = null)
        {
            directory = directory ?? BaseDirectory;

            IEnumerable<string> dirs = from dir in Directory.GetDirectories(directory)
                                       let d = new DirectoryInfo(dir).Name
                                       where !Excepts.Folders.Any(p => p.IsMatch(d))
                                       select dir;

            foreach (string dir in dirs)
            {
                yield return dir;
                foreach (string subdir in Directories(dir))
                {
                    yield return subdir;
                }
            }
        }

        public string RelativePath(string directory)
        {
            return directory.Replace(BaseDirectory, "").TrimStart('\\');
        }
    }
}