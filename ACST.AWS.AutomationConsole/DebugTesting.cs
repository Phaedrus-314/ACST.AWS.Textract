
namespace ACST.AWS.AutomationConsole
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using ACST.AWS.TransferUtility;
    using ACST.AWS.Common;
    using System.Reflection;
    using ACST.AWS.Common.OCR;
    using ACST.AWS.Textract.Model;
    using ACST.AWS.TextractClaimMapper;
    using System.Diagnostics;
    
    public static class DebugTesting
    {
        [Conditional("DEBUG")]
        public static void TestSelector()
        {
            Logger.TraceInfo($"Running: ACST.AWS.AutomationConsole.TestSelector{Environment.NewLine}");

            //TestWorkingLocal.SimulateADAtextractClient();
            TestSpeculativeLocal.LocalTest();
        }
    }
}
