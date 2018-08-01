using CPI.Models;
using CPI.Models.Entity;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CPI.ViewModels
{
    public class Interceptor
    {
        private static SshClient sshClient;
        private static ShellStream shellStream;


        public static bool Start(Computer cpu, RecordFile cfile, InterceptorType type, bool rmCfile, string bcIP, int bcPort, string path)
        {

            if (Initialize(cpu))
            {
                string command = String.Format("cd dev_utilities && ./interceptor.sh -r {0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10} {11} &",
                    type.ToString(), cfile.ID, cfile.Count, cfile.Band, cfile.ARFCN, cfile.Frequency, cfile.SampleRate, cfile.Rx, rmCfile ? 1 : 0, bcIP, bcPort, path);
                shellStream.WriteLine(command);
#if (DEBUG)
                string feedback = shellStream.Expect("root", new TimeSpan(0, 0, 10));
#endif
                sshClient.Disconnect();

                return true;
            }
            else
                return false;
        }


        public static bool Stop(Computer cpu, int indx)
        {
            if (Initialize(cpu))
            {
                string command = String.Format("cd dev_utilities && ./interceptor.sh -s");
                shellStream.WriteLine(command);
#if (DEBUG)
                string feedback = shellStream.Expect("root", new TimeSpan(0, 0, 10));
#endif
                sshClient.Disconnect();

                return true;
            }
            else
                return false;
        }

        private static bool Initialize(Computer cpu)
        {
            try
            {
                sshClient = new SshClient(cpu.IP, 22, cpu.Username, cpu.Password);
                sshClient.Connect();

                IDictionary<Renci.SshNet.Common.TerminalModes, uint> termkvp = new Dictionary<Renci.SshNet.Common.TerminalModes, uint>
                {
                    { Renci.SshNet.Common.TerminalModes.ECHO, 53 }
                };

                shellStream = sshClient.CreateShellStream("Interceptor", 200, 50, 800, 600, 1024, termkvp);

                // Login As Root
                string feedback = shellStream.Expect(new Regex(@"[$>]"));
                shellStream.WriteLine("sudo su");
                //expect password or user prompt
                feedback = shellStream.Expect(new Regex(@"([$#>:])"));
                if (feedback.Contains(":"))
                {
                    shellStream.WriteLine(cpu.Password);
                    //expect user or root prompt
                    feedback = shellStream.Expect(new Regex(@"[$#>]"));
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }


    }
}
