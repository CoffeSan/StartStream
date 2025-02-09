using System.Diagnostics;
using System.Text.Json;

namespace StartStream
{
    /// <summary>
    /// Handles the opening and closing of programs specified in the Programs.json file.
    /// </summary>
    public static class ProgramsHandler
    {
        /// <summary>
        /// Opens all programs specified in the Programs.json file.
        /// </summary>
        public static void OpenPrograms()
        {
            string[] programs = LoadProgramList();

            if (programs.Length == 0)
            {
                Console.WriteLine("No programs found in Programas.json.");
                Console.ReadKey();
                return;
            }

            foreach (string program in programs)
            {
                try
                {
                    string obsExecutable = Path.Combine(program, "obs64.exe");
                    if (File.Exists(obsExecutable))
                    {

                        // Start OBS with the correct working directory
                        ProcessStartInfo startInfo = new ProcessStartInfo
                        {
                            FileName = obsExecutable,
                            WorkingDirectory = program
                        };

                        Process.Start(startInfo);
                    }
                    else
                        Process.Start(program);

                    Console.WriteLine($"Started: {program}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to start {program}: {ex.Message}");
                }
            }

            Console.WriteLine("All programs processed.");
            Console.ReadKey();
        }

        /// <summary>
        /// Closes all programs specified in the Programs.json file.
        /// </summary>
        public static void ClosePrograms()
        {
            string[] programs = LoadProgramList();

            if (programs.Length == 0)
            {
                Console.WriteLine("No programs found in Programas.json.");
                Console.ReadKey();
                return;
            }

            foreach (string program in programs)
            {
                string processName = Path.GetFileNameWithoutExtension(program);

                try
                {
                    string obsExecutable = Path.Combine(program, "obs64.exe");
                    if (File.Exists(obsExecutable))
                        processName = "obs64";
                
                    KillProcess(processName);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to close {processName}: {ex.Message}");
                }
            }

            Console.WriteLine("All programs processed.");
            Console.ReadKey();
        }

        /// <summary>
        /// Kills a process with the specified name.
        /// </summary>
        /// <param name="processName">The name of the process to kill.</param>
        public static void KillProcess(string processName)
        {
            Process[] processes = Process.GetProcessesByName(processName);

            if (processes.Length == 0)
            {
                Console.WriteLine($"No running process found for: {processName}");
                return;
            }

            foreach (Process process in processes)
            {
                process.Kill();
                process.WaitForExit();
                Console.WriteLine($"Closed process: {processName}");
            }
        }

        /// <summary>
        /// Loads the list of programs from the Programs.json file.
        /// </summary>
        /// <returns>An array of strings representing the programs to open/close.</returns>
        public static string[] LoadProgramList()
        {
            const string configFilePath = "Programs.json";

            if (!File.Exists(configFilePath))
            {
                Console.WriteLine("Error: Configuration file not found.");
                return Array.Empty<string>();
            }

            try
            {
                string jsonContent = File.ReadAllText(configFilePath);
                List<string>? programs = JsonSerializer.Deserialize<List<string>>(jsonContent);

                return programs?.ToArray() ?? Array.Empty<string>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading configuration: {ex.Message}");
                return Array.Empty<string>();
            }
        }
    }
}
