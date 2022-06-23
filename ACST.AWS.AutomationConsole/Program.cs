
namespace ACST.AWS.AutomationConsole
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using ACST.AWS.Common;

    public class Program
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

        #region Fields & Properties

        ///// <summary>
        ///// True indicates production, non-debug execution.
        ///// </summary>
        static bool productionRun = true;

        /// <summary>
        /// String array of command line arguments supplied to debug run
        /// </summary>
        static string[] debugArgs = null;

        /// <summary>
        /// Named value collection of argument modifiers
        /// </summary>
        static IDictionary<string, object> validModifiers = null;

        /// <summary>
        /// Parsed and validated application comandline parameters
        /// </summary>
        static IEnumerable<string> validParams = null;

        /// <summary>
        /// Id
        /// </summary>
        static string[] IDs
        {
            get
            {
                return validModifiers.Where(m => m.Key.Equals("ID", StringComparison.OrdinalIgnoreCase)).Select(v => ((string)v.Value).Split(',')).SingleOrDefault();
            }
        }

        #endregion

        #region Methods
        
        public static void Main(string[] args)
        {
            try
            {
                Console.WindowHeight = 35;

                // Parse command line arguments
                validParams = ParseCommandLine(args);
                validModifiers = ParseModifiers(validParams);

                // Display console app usage
                if (validParams.Contains("Help"))
                    ShowHelp();

                // For the moment, Textract is the only AWS we are automating
                if (validParams.Contains("Textract"))
                {
                    // This will only run in Debug build
                    RunTextractTests();

                    // This will only run in Release build
                    if (productionRun)
                        StartTextractWatcher();
                }

                //Logger.TraceInfo($"Running: ACST.AWS.TextractClaimMapper{Environment.NewLine}");

                //Watcher.Run(Configuration.Instance.Watch_Folder, Configuration.Instance.Watch_FileFilter);

                // Pause console
                if (validParams.Contains("Pause"))
                {
                    Console.WriteLine("Press Enter to Exit...");
                    Console.ReadLine();
                }

            }
            catch (Exception ex)
            {
                Logger.TraceInfo(ex.ToString());
                Console.WriteLine("Press Enter to Exit...");
                Console.ReadLine();
            }
        }

        /// <summary>
        /// Returns enumerable collection of valid application arguments
        /// </summary>
        /// <param name="args">Command line arguments</param>
        static IEnumerable<string> ParseCommandLine(string[] args)
        {

            List<string> validParms = new List<string>();
            string modifiers = null;

            try
            {
                // Decide between dash or space as commandline argument delimiter
                string commandLine = Environment.CommandLine;

                // Remove executable from commandline
                int secondQuotePosition = commandLine.IndexOf("\"", 2);
                commandLine = commandLine.Substring(secondQuotePosition).TrimStart(new char[] { ' ', '\"' });

                if (commandLine.Contains('-'))
                    args = commandLine.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
            }
            catch { }

            // Local debug override
            ArgsOverride();
            args = debugArgs ?? args;

            for (int i = 0; i < args.Count(); i++)
            {
                args[i] = args[i].Replace(" ", string.Empty);
            }

            // Display Help if 'H|Help' argument found
            if (args.Any(a => a.TrimStart('-').StartsWith("h", StringComparison.OrdinalIgnoreCase))
                | args.Any(a => a.TrimStart('-').Equals("?"))
               )
                validParms.Add("Help");

            // Pause console if 'Pa|Pause' argument found
            if (args.Any(a => a.TrimStart('-').StartsWith("pa", StringComparison.OrdinalIgnoreCase)))
                validParms.Add("Pause");

            // Load new EDI Claims if 'C|Claims' argument found
            if (args.Any(a => a.TrimStart('-').StartsWith("t", StringComparison.OrdinalIgnoreCase)))
                validParms.Add("Textract");

            return validParms;
        }

        /// <summary>
        /// Returns dictionary of name / value pairs which supply additional detail to application arguments
        /// </summary>
        /// <param name="validParams">Valid commad line arguments</param>
        static IDictionary<string, object> ParseModifiers(IEnumerable<string> validParams)
        {
            Dictionary<string, object> ret = null;

            try
            {
                ret = validParams.Where(v => v.Contains(':'))
                                .Select(v => v.Split(':'))
                                .ToDictionary(v => v[0], v => (object)v[1].Trim());
            }
            catch { }

            return ret;
        }

        /// <summary>
        /// Display application options and help
        /// </summary>
        static void ShowHelp()
        {
            var ParmDescriptions = new[]
            {
                new { parm = "H  | Help | ?", description = "show this message and pause." },
                new { parm = "T  | Textract", description = "Textract OCR files from configured watched folder." },
                new { parm = "Pa | Pause", description = "pause before closing console." }
            };

            Console.WriteLine("Usage: ACST.AWS.Automation Console [OPTIONS]");
            Console.WriteLine("Execute AWS Automation process specificed by Option.");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine();
            ParmDescriptions.ToList().ForEach(a => Console.WriteLine("{0,-20} {1}", a.parm, a.description));
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Modifiers:");
            Console.WriteLine("\t\t[...]\tRequired");
            Console.WriteLine("\t\t{...}\tOptional");
            Console.WriteLine();
            Console.WriteLine("Seperators:");
            Console.WriteLine("\t\t-\tArgument Names");
            Console.WriteLine("\t\t:\tArgument Values");
            Console.WriteLine("\t\t,\tRepeating Values");
            Console.WriteLine();
            Console.WriteLine("Examples:");
            Console.WriteLine("\t\tEdiAutomationConsole.exe -Claims -PPO: Cigna");
            Console.WriteLine("\t\tEdiAutomationConsole.exe -Claims -Id: 123456,2222222");
            Console.WriteLine("\t\tEdiAutomationConsole.exe -Enrollment -Pause");
            Console.WriteLine("\t\tEdiAutomationConsole.exe -Remitance -Pause");
            Console.WriteLine("\t\tEdiAutomationConsole.exe -h");
            Console.WriteLine();
            Console.WriteLine("Press Enter to Exit...");
            Console.ReadLine();
        }

        /// <summary>
        /// Start Textract Folder Watcher
        /// </summary>
        static void StartTextractWatcher()
        {

            Logger.TraceInfo($"Running: ACST.AWS.TextractClaimMapper{Environment.NewLine}");

            Watcher.Run(Configuration.Instance.Watch_Folder, Configuration.Instance.Watch_FileFilter);

        }
        #endregion

        #region Test & Development
        static bool DEBUG_Simulate_Prod = false;

        /// <summary>
        /// Create or override command line arguments 
        /// </summary>
        [Conditional("DEBUG")]
        static void ArgsOverride()
        {
            productionRun = false;

            DEBUG_Simulate_Prod = true;

            debugArgs = new[] { "Textract", "Pause" };

            //debugArgs = new[] { "?" };
            Logger.TraceInfo($"ArgsOverride: productionRun: {productionRun}, DEBUG_Simulate_Prod: {DEBUG_Simulate_Prod}");
            debugArgs.ToList().ForEach(d => Logger.TraceInfo($"DebugArgs: {d}"));
        }

        [Conditional("DEBUG")]
        static void RunTextractTests()
        {
            Logger.TraceInfo("Debug Main: RunTextractTests");
            
            if (DEBUG_Simulate_Prod) 
                StartTextractWatcher();

            if (!DEBUG_Simulate_Prod)
                DebugTesting.TestSelector();
        }
        #endregion
    }
}
