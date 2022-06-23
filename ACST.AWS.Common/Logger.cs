
namespace ACST.AWS.Common
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;

    [DebuggerStepThrough()]
    public static class Logger
    {
        static readonly string procName = Process.GetCurrentProcess().ProcessName;

        static readonly TraceSwitch traceSwitch = new TraceSwitch("AppTraceLevel", null);

        public static readonly string DefaultLoggingFolder = Path.GetTempPath();

        #region Constructor

        static Logger()
        {
            try
            {
                // Replace TextWriterTraceListener FileName with date values
                if (Trace.Listeners["UtilityTextListener"] is TextWriterTraceListener)
                {
                    TextWriterTraceListener textWriterTL = (TextWriterTraceListener)Trace.Listeners["UtilityTextListener"];

                    // Get file name from TextWriterTraceListener
                    StreamWriter writer = (StreamWriter)textWriterTL.Writer;
                    FileStream stream = (FileStream)writer.BaseStream;
                    string OriginalFileName = stream.Name;

                    string FileName = FormattedPathName.TimeStampFile(OriginalFileName);
                    string FilePath = Path.GetDirectoryName(OriginalFileName);

                    DefaultLoggingFolder = FilePath;

                    // Remove pattern trace file
                    textWriterTL.Close();
                    Trace.Listeners.Remove(textWriterTL);

                    try
                    {
                        // cleanup pattern file
                        if (File.Exists(OriginalFileName))
                            File.Delete(OriginalFileName);
                    }
                    catch { }

                    // Create new TL with modified custom name
                    TextWriterTraceListener myListener = new TextWriterTraceListener(Path.Combine(FilePath, FileName), "UtilityTextListener");

                    Trace.Listeners.Add(myListener);
                }

                TraceInfo(Environment.NewLine + Environment.NewLine);
                TraceInfo("Begin {0} Logging at {1} on {2}", traceSwitch.Level.ToString(), DateTime.Now.ToString(), procName);
                TraceInfo(Environment.NewLine);

            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion Constructor

        #region Debug Methods

        static void DebugLine(string message)
        {
            Debug.WriteLine(message);
        }

        [Conditional("DEBUG")]
        public static void DebugInfo(string format, params object[] args)
        {
            DebugInfo(string.Format(CultureInfo.InvariantCulture, format, args));
        }

        [Conditional("DEBUG")]
        public static void DebugInfo(string message)
        {
            DebugLine(message);
        }
        #endregion Debug Methods

        #region Trace Methods

        [ConditionalAttribute("TRACE")]
        public static void TraceFlush()
        {
            Trace.Flush();
        }

        [ConditionalAttribute("TRACE")]
        public static void TraceIndent()
        {
            Trace.Indent();
        }

        [ConditionalAttribute("TRACE")]
        public static void TraceUnindent()
        {
            Trace.Unindent();
        }

        [ConditionalAttribute("TRACE")]
        public static void TraceInfo(string format, params object[] args)
        {
            TraceInfo(string.Format(CultureInfo.InvariantCulture, format, args));
        }

        [ConditionalAttribute("TRACE")]
        public static void TraceInfo(string message)
        {
            if (traceSwitch.TraceInfo)
                TraceLine(TraceLevel.Info, message);
        }

        [ConditionalAttribute("TRACE")]
        public static void TraceError(string format, params object[] args)
        {
            TraceError(string.Format(CultureInfo.InvariantCulture, format, args));
        }

        [ConditionalAttribute("TRACE")]
        public static void TraceError(Exception ex)
        {
            if (traceSwitch.TraceError | traceSwitch.TraceVerbose)
            {
                TraceIndent();
                TraceLine(TraceLevel.Error, Environment.NewLine);
                TraceLine(TraceLevel.Error, ex.ToString());
                TraceUnindent();
            }
        }

        [ConditionalAttribute("TRACE")]
        public static void TraceError(string message)
        {
            if (traceSwitch.TraceError)
                TraceLine(TraceLevel.Error, message);
        }

        [ConditionalAttribute("TRACE")]
        public static void TraceWarning(string format, params object[] args)
        {
            TraceWarning(string.Format(CultureInfo.InvariantCulture, format, args));
        }

        [ConditionalAttribute("TRACE")]
        public static void TraceWarning(string message)
        {
            if (traceSwitch.TraceWarning)
                TraceLine(TraceLevel.Warning, message);
        }

        [ConditionalAttribute("TRACE")]
        public static void TraceVerbose(string format, params object[] args)
        {
            TraceVerbose(string.Format(CultureInfo.InvariantCulture, format, args));
        }

        [ConditionalAttribute("TRACE")]
        public static void TraceVerbose(string message)
        {
            if (traceSwitch.TraceVerbose)
                TraceLine(TraceLevel.Verbose, message);
        }

        static void TraceLine(TraceLevel level, string message)
        {
            string traceLevel = null;

            switch (level)
            {
                case TraceLevel.Error:
                    traceLevel = "ERROR:  ";
                    break;
                case TraceLevel.Warning:
                    traceLevel = "WARNING:";
                    break;
                case TraceLevel.Info:
                    traceLevel = "INFO:   ";
                    break;
                case TraceLevel.Verbose:
                    traceLevel = "VERBOSE:";
                    break;
                default:
                    traceLevel = "DEFAULT:";
                    break;
            }

            //string finalMessage = string.Format( CultureInfo.InvariantCulture, "{0}, {1}, {2}, {3}, {4}", traceLevel, procName, Thread.CurrentThread.ManagedThreadId, DateTime.Now, message);
            string finalMessage = string.Format("{0}", message);
            Trace.WriteLine(finalMessage);
        }
        #endregion Trace Methods
    }
}

