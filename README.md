[# Kafka_Stream_Demo](https://jsfiddle.net/#&togetherjs=7GCk87e4of)


using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

class Program
{
    // P/Invoke declarations
    [DllImport("Netapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern int NetUseAdd(
        string UncServerName,
        int Level,
        ref USE_INFO_2 Buf,
        out int ParmError
    );

    [DllImport("Netapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern int NetUseDel(
        string UncServerName,
        string UseName,
        int ForceCond
    );

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct USE_INFO_2
    {
        public string ui2_local;
        public string ui2_remote;
        public string ui2_password;
        public uint ui2_status;
        public uint ui2_asg_type;
        public uint ui2_refcount;
        public uint ui2_usecount;
        public string ui2_username;
        public string ui2_domainname;
    }

    static async Task Main()
    {
        string sourceDrive = @"\\sourceServer\sourceShare";
        string destinationDrive = @"\\destinationServer\destinationShare";
        string username = "serviceAccountUsername";
        string password = "serviceAccountPassword";

        // Get the current date in the required format
        string currentDate = DateTime.Now.ToString("dd_MMM_yy");

        // Construct the file name based on the date
        string sourceFileName = $"{currentDate}.txt";

        // Construct the full paths of the source and destination files
        string sourceFilePath = Path.Combine(sourceDrive, sourceFileName);
        string destinationFilePath = Path.Combine(destinationDrive, sourceFileName);

        // Establish network connections to the source and destination drives asynchronously
        bool isConnectedToSource = await ConnectToNetworkDriveAsync(sourceDrive, username, password);
        bool isConnectedToDestination = await ConnectToNetworkDriveAsync(destinationDrive, username, password);

        if (isConnectedToSource && isConnectedToDestination)
        {
            try
            {
                // Check if the source file exists asynchronously
                bool isSourceFileExists = await FileExistsAsync(sourceFilePath);

                if (isSourceFileExists)
                {
                    // Copy the file to the destination asynchronously
                    await CopyFileAsync(sourceFilePath, destinationFilePath);
                    Console.WriteLine("File copied successfully.");
                }
                else
                {
                    Console.WriteLine("Source file does not exist.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to copy the file: " + ex.Message);
            }
            finally
            {
                // Disconnect from the network drives asynchronously
                await DisconnectFromNetworkDriveAsync(sourceDrive);
                await DisconnectFromNetworkDriveAsync(destinationDrive);
            }
        }
        else
        {
            Console.WriteLine("Failed to establish network connections.");
        }
    }

    static Task<bool> ConnectToNetworkDriveAsync(string drivePath, string username, string password)
    {
        return Task.Run(() =>
        {
            // Create a USE_INFO_2 structure for the network drive
            USE_INFO_2 driveInfo = new USE_INFO_2
            {
                ui2_remote = drivePath,
                ui2_username = username,
                ui2_password = password,
                ui2_domainname = null,
                ui2_asg_type = 0
            };

            int result = NetUseAdd(null, 2, ref driveInfo, out _);
            return result == 0;
        });
    }

    static Task DisconnectFromNetworkDriveAsync(string drivePath)
    {
        return Task.Run(() =>
        {
            int result = NetUseDel(null, drivePath, 2);
            if (result != 0)
            {
                Console.WriteLine("Failed to disconnect from the network drive: " + drivePath);
            }
        });
    }

    static Task<bool> FileExistsAsync(string filePath)
    {
        return Task.Run(() => File.Exists(filePath));
    }

    static Task CopyFileAsync(string sourceFilePath, string destinationFilePath)
    {
        return Task.Run(() => File.Copy(sourceFilePath, destinationFilePath, true));
    }
}

