using CPI.Models.Entity;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CPI.ViewModels
{
    public class Scanner
    {
        static SshClient sshClient;
        static ShellStream shellStream;


        /// <summary>
        /// Start The Scanner
        /// </summary>
        /// <param name="cpu">CPU</param>
        /// <param name="indx">Receiver index</param>
        /// <param name="ip">Broadcast IP</param>
        /// <param name="port">Broadcast port</param>
        /// <param name="band">Band</param>
        /// <param name="gain">Gain</param>
        /// <param name="speed">Speed</param>
        /// <param name="sample_rate">Sample Rate</param>
        /// <param name="ppm">PPM</param>
        /// <returns></returns>
        public static bool Start(Computer cpu, int indx, string ip, int port, string band, int gain, int speed, int sample_rate, int ppm)
        {

            if (Initialize(cpu))
            {
                string command = String.Format("cd dev_utilities && ./scanner.sh -r {0} {1} {2} {3} {4} {5} {6} {7} &",
                    indx, band, sample_rate, ppm, gain, speed, ip, port);
                shellStream.WriteLine(command);
#if (DEBUG)
                string feedback = shellStream.Expect("root", new TimeSpan(0, 10, 0));
#endif
                sshClient.Disconnect();

                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Cancel the Scanner
        /// </summary>
        /// <param name="cpu">CPU</param>
        /// <returns></returns>
        public static bool Cancel(Computer cpu)
        {
            if (Initialize(cpu))
            {
                string command = String.Format("cd dev_utilities && ./scanner.sh -s");
                shellStream.WriteLine(command);
#if (DEBUG)
                string feedback = shellStream.Expect("root", new TimeSpan(0, 10, 0));
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

                shellStream = sshClient.CreateShellStream("Scanner", 200, 50, 800, 600, 1024, termkvp);

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
