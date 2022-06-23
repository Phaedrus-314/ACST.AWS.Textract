using System;

using ACST.AWS.Common;

namespace ACST.AWS.TextractClaimMapper
{
    public class Program
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

        public static void Main(string[] args)
        {
            try
            {
                WatchFolder();

                //RunWorkingTests();

                //RunSpeculativeTests();
            }
            catch (Exception ex)
            {
                Logger.TraceInfo(ex.ToString());
            }

            Console.WriteLine(Environment.NewLine + Environment.NewLine + "Press Enter to Exit...");
            Console.ReadLine();
        }


        public static void WatchFolder()
        {
            Watcher.Run(Configuration.Instance.Watch_Folder, Configuration.Instance.Watch_FileFilter);
        }

        static void RunWorkingTests()
        {
            TestWorkingLocal.TestTransmissionInsert();

            //TestWorkingLocal.TestConfigurationGenerator_IntermediateFiles();

            //TestWorkingLocal.BuildSampleADAData();

            //string namedCoordinatesFileName = @".\NamedCoordinatesData\ADANamedCoordinates_2.xml";
            //string imageFile = @"C:\temp\NewDental\Individuals_ClaimsOnly\Dental Claims 060820 5.jpg";

            //TestLocal.SimulateADAtextractClient();

            //TestWorkingLocal.ProcessImageAsyncTest(imageFile, namedCoordinatesFileName).Wait();

            //TestLocal.BuildTextractClaimAsyncTest().Wait();
        }

        static void RunSpeculativeTests()
        {
            //TestSpeculativeLocal.LocalMappingTests();
            TestSpeculativeLocal.LocalTextractLinqTests();
        }
    }
}
