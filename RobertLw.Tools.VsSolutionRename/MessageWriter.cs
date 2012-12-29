#region File Descrption

// /////////////////////////////////////////////////////////////////////////////
// 
// Project: RobertLw.Tools.VsSolutionRename.RobertLw.Tools.VsSolutionRename
// File:    MessageWriter.cs
// 
// Create by Robert.L at 2012/12/28 17:35
// 
// /////////////////////////////////////////////////////////////////////////////

#endregion

using System;
using System.ComponentModel;
using System.IO;


namespace RobertLw.Tools.VsSolutionRename
{
    public sealed class MessageWriter : StringWriter
    {
        #region Delegates

        [EditorBrowsable(EditorBrowsableState.Never)]
        public delegate void FlushedEventHandler(object sender, EventArgs args);

        #endregion

        public MessageWriter(bool autoFlush = true)
        {
            AutoFlush = autoFlush;
        }

        public bool AutoFlush { get; set; }
        public event FlushedEventHandler Flushed;

        private void OnFlush()
        {
            if (Flushed != null)
            {
                Flushed(this, EventArgs.Empty);
            }
        }

        public override void Flush()
        {
            base.Flush();
            OnFlush();
        }

        public override void Write(char value)
        {
            base.Write(value);
            if (AutoFlush) Flush();
        }

        public override void Write(string value)
        {
            base.Write(value);
            if (AutoFlush) Flush();
        }

        public override void Write(char[] buffer, int index, int count)
        {
            base.Write(buffer, index, count);
            if (AutoFlush) Flush();
        }
    }
}