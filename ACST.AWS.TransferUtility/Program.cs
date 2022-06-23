using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACST.AWS.TransferUtility
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //Todo: get rid of test console make this a dll

                //FileTransfer.StartPolling(); // Upload entire directory or internal file

                //Task t = FileTransfer.UploadFileAsync(@"C:\temp\HCFA_111219_5_PollTest\HCFA_111219_5.v1.jpg","testName");
                Task t = FileTransfer.UploadFileAsync(@"C:\temp\HCFA_111219_5_PollTest\HCFA_111219_5.v1.jpg");
                t.Wait();
                
                if(t.IsCompleted)
                {
                    Console.WriteLine("Verified completion.");
                }

            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(String.Format(Environment.NewLine + "{0}", ex.ToString()));
                Console.WriteLine(ex.ToString());
            }

            Console.WriteLine(Environment.NewLine + Environment.NewLine + "Press Enter to Exit...");
            Console.ReadLine();
        }
    }
}
