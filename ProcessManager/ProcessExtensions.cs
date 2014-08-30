using System.Diagnostics;
using System.Management;

namespace ProcessManager {
    public static class ProcessExtensions {
        public static void KillAllProcessesSpawnedBy(this Process process) {
            KillAllProcessesSpawnedBy((uint)process.Id);
        }

        public static void KillAllProcessesSpawnedBy(uint parentProcessId) {
            Debug.WriteLine("Finding processes spawned by process with Id [" + parentProcessId + "]");

            // NOTE: Process Ids are reused!
            var searcher = new ManagementObjectSearcher(
                    "SELECT * " +
                    "FROM Win32_Process " +
                    "WHERE ParentProcessId=" + parentProcessId);
            var collection = searcher.Get();
            if (collection.Count <= 0) {
                return;
            }
            Debug.WriteLine("Killing [" + collection.Count
                            + "] processes spawned by process with Id [" + parentProcessId + "]");
            foreach (var item in collection) {
                var childProcessId = (uint)item["ProcessId"];
                if ((int)childProcessId == Process.GetCurrentProcess().Id) {
                    continue;
                }
                KillAllProcessesSpawnedBy(childProcessId);

                var childProcess = Process.GetProcessById((int)childProcessId);
                Debug.WriteLine("Killing child process [" + childProcess.ProcessName + "] with Id ["
                                + childProcessId + "]");
                childProcess.Kill();
            }
        }
    }
}