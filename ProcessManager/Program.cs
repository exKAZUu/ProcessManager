using System;
using System.Diagnostics;
using Mono.Options;

namespace ProcessManager {
    internal class Program {
        private static void Main(string[] args) {
            var help = false;
            var execFilePath = "";
            var killPid = 0U;
            var p = new OptionSet {
                { "s|start=", v => execFilePath = v },
                { "h|?|help", v => help = v != null }, {
                    "k|kill=", v => {
                        if (!uint.TryParse(v, out killPid)) {
                            Console.Error.WriteLine("The given PID is an invalid value.");
                            Environment.Exit(-1);
                        }
                    }
                }
            };
            var arguments = p.Parse(args);

            if (help) {
                p.WriteOptionDescriptions(Console.Out);
            } else if (!string.IsNullOrEmpty(execFilePath)) {
                using (var proc = new Process {
                    StartInfo = {
                        FileName = execFilePath,
                        Arguments = string.Join(" ", arguments),
                        UseShellExecute = false,
                        RedirectStandardOutput = false,
                        RedirectStandardInput = false,
                        CreateNoWindow = false,
                    },
                }) {
                    proc.Start();
                    Console.WriteLine("PID: " + proc.Id);
                }
            } else if (killPid != 0) {
                ProcessExtensions.KillAllProcessesSpawnedBy(killPid);
            }
        }
    }
}