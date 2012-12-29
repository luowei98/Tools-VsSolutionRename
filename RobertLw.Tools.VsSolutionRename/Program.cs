using System;
using System.IO;
using RobertLw.Tools.VsSolutionRename.CommandLine;


namespace RobertLw.Tools.VsSolutionRename
{
    internal class Program
    {
        private static MessageWriter message;

        private static void Main(string[] args)
        {
            message = new MessageWriter();
            message.Flushed += MessageFlushed;

            var para = new ParameterWrap(args, message);
            if (!para.IsGood) Environment.Exit(1);

            var oper = new Operator(para);
            oper.Doit();

            Console.ReadKey();
        }

        private static void MessageFlushed(object sender, EventArgs args)
        {
            Console.WriteLine(sender.ToString());
        }
    }
}