using System;
using System.IO;
using Ionic.Zip;
using RobertLw.Tools.RobertLw.Tools.VsSolutionRename.CommandLine;
using RobertLw.Tools.RobertLw.Tools.VsSolutionRename.PathPaser;


namespace RobertLw.Tools.RobertLw.Tools.VsSolutionRename
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var para = new ParameterWrap(args, Console.Error);
            if (!para.IsGood) Environment.Exit(1);

            var oper = new Operator(para);
            oper.Doit();

            Console.ReadKey();
        }

    }
}
