using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace GhusnCloudServices.ASPNETCORE.Publisher
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                // Step 1: Get the current project's target framework
                string projectFile = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.csproj").FirstOrDefault();
                if (projectFile == null)
                {
                    Console.WriteLine("No project file found in the current directory.");
                    return;
                }

                string targetFramework = await GetTargetFrameworkAsync(projectFile);
                if (string.IsNullOrEmpty(targetFramework))
                {
                    Console.WriteLine("Could not determine the target framework.");
                    return;
                }

                // Step 2: Publish the project
                string publishDirectory = Path.Combine("bin", "Release", "GhusnCloudServices", targetFramework, "Publish");
                string publishCommand = $"dotnet publish -c Release -r linux-x64 --self-contained false -o {publishDirectory}";

                Console.WriteLine("Publishing the project...");
                var publishResult = await ExecuteCommandAsync(publishCommand);
                if (publishResult.ExitCode != 0)
                {
                    Console.WriteLine($"Publish failed: {publishResult.Output}");
                    return;
                }

                // Verify publish directory
                if (!Directory.Exists(publishDirectory))
                {
                    Console.WriteLine("Publish directory does not exist.");
                    return;
                }

                // Step 3: Zip the published files
                string projectName = Path.GetFileNameWithoutExtension(projectFile);
                string zipFileName = $"{projectName}_linux-x64.zip";
                string zipFilePath = Path.Combine(Directory.GetCurrentDirectory(), "bin", "Release", "GhusnCloudServices", targetFramework, zipFileName);

                Console.WriteLine("Zipping the published files...");
                if (File.Exists(zipFilePath))
                {
                    Console.WriteLine($"Existing zip file found at: {zipFilePath}. Deleting...");
                    File.Delete(zipFilePath);
                }

                ZipFile.CreateFromDirectory(publishDirectory, zipFilePath);
                Console.WriteLine($"Zip file created at: {zipFilePath}");

                // Step 4: Delete the published files
                Console.WriteLine("Cleaning up published files...");
                Directory.Delete(publishDirectory, true);

                Console.WriteLine($"Success! The zip file is located at: {zipFilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }

        private static async Task<(int ExitCode, string Output)> ExecuteCommandAsync(string command)
        {
            var processInfo = new ProcessStartInfo("cmd", $"/c {command}")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            var process = new Process { StartInfo = processInfo };
            process.Start();

            string output = await process.StandardOutput.ReadToEndAsync();
            string error = await process.StandardError.ReadToEndAsync();
            process.WaitForExit();

            int exitCode = process.ExitCode;
            if (exitCode != 0)
            {
                output += error;
            }

            return (exitCode, output);
        }

        private static async Task<string> GetTargetFrameworkAsync(string projectFile)
        {
            string targetFramework = null;

            using (var reader = new StreamReader(projectFile))
            {
                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    if (line.Contains("<TargetFramework>"))
                    {
                        targetFramework = line.Split(new[] { "<TargetFramework>", "</TargetFramework>" }, StringSplitOptions.RemoveEmptyEntries)[1];
                        break;
                    }
                }
            }

            return targetFramework;
        }
    }
}