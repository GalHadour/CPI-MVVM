using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CPI.Models.Entity;
using Renci.SshNet;

namespace CPI.ViewModels
{
    public class Service
    {
        private static SshClient sshClient;
        private static ShellStream shellStream;


        public static bool Reload(Computer cpu, string bcIP, int bcPort, string path)
        {

            if (Initialize(cpu))
            {
                string command = String.Format("./gaf.sh {0} {1} {2} &", bcIP, bcPort, path);
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


        internal static bool DeleteFile(Computer cpu, string fileName, Models.Type fileType)
        {
            if (Initialize(cpu))
            {
                string command = String.Empty;
                switch (fileType)
                {
                    case Models.Type.CALL:
                        command = String.Format("cd calls && rm {0} &", fileName);
                        break;
                    case Models.Type.SMS:
                        command = String.Format("cd sms && rm {0} &", fileName);
                        break; ;
                }
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


        public static bool DownloadFile(Computer cpu, string localDestinationFilename, string remoteDestinationFilename)
        {
            try
            {
                using (var sftp = new SftpClient(cpu.IP, 22, cpu.Username, cpu.Password))
                {
                    sftp.Connect();
                    using (FileStream fileStream = File.OpenWrite(localDestinationFilename))
                    {
                        sftp.DownloadFile(remoteDestinationFilename, fileStream);
                    }
                    sftp.Disconnect();
                }
                return true;
            }
            catch (Exception)
            {
#if(DEBUG)
                throw;
#else
                return false;
#endif



            }
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

                shellStream = sshClient.CreateShellStream("Service", 200, 50, 800, 600, 1024, termkvp);

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
