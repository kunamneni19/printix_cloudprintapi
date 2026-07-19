using Newtonsoft.Json;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net;

namespace CommonUtils
{
    public class CommonUtil
    {
        private static readonly string[] ValidEnvironments = {
            "dev01", "dev02", "dev03", "dev04", "devenv",
            "testenv", "devenv.us", "testenv.us", "prodenv", "prodenv.us"
        };

        public static void ValidateEnvironmentVariable(string? environment)
        {
            //expect the 'environment' parameter to be non-null here

            string? normalizedEnvironment = environment?.ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(normalizedEnvironment) || !ValidEnvironments.Contains(normalizedEnvironment))
            {
                string validOptions = string.Join(", ", ValidEnvironments);
                throw new InvalidOperationException(
                    $"Invalid or missing 'environment' variable. " +
                    $"Received value: '{environment ?? "null/empty"}'. " + // Use original 'environment' for error message
                    $"Please set the 'environment' variable to one of the following valid options: {validOptions}."
                );
            }
        }
        public string GetWorkFlowName(string name)
        {

            try
            {
                Random rnd = new Random();
                int number = rnd.Next(1, 10000);
                name = "WorkFlow_" + name + number;
                return name;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"failed to execute GetWorkFlowName(): {ex.Message}");
                throw;
            }

        }
        public string GetResourceFilePath(string filename)
        {
            // filename mst end with format. eg: black.pdf
            string FilePath;
            try
            {
                string binPath = AppDomain.CurrentDomain.BaseDirectory;
                string repoRoot = Path.GetFullPath(Path.Combine(binPath, @"..\..\..\"));
                string resourcePath = Path.Combine(repoRoot, "APITest.Utils", "Resources");
                
                if (resourcePath.Contains("ExternalAPITEST"))
                {
                    FilePath = resourcePath.Split("\\ExternalAPITEST")[0] + "\\APITest.Utils\\Resources\\" + filename;
                }
                else
                {
                    FilePath = resourcePath.Split("\\CloudPrintAPI")[0] + "\\APITest.Utils\\Resources\\" + filename; 
                }

               // string GetPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
               // FilePath = GetPath.Split("\\net.printix.test.cloudprintapi")[0] + "\\net.printix.test.cloudprintapi\\APITest.Utils\\Resources\\" + filename;
                return FilePath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"failed to execute GetUploadFilePath(): {ex.Message}");
                throw;
            }

        }

        /**
         * 
          [ Raw Json File on Disk ] 
          │
          ▼ (File.ReadAllText)
            [ Massive String Block of Text ] 
          │
          ▼ (JsonConvert.DeserializeObject<T>)
            [ Strongly Typed C# object (TestData) ]

         */
       
        public static T ReadAndValidateJsonData<T>(string filePath)
        {
            try
            {
                string jsonData = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<T>(jsonData);
            }
            catch (Exception ex)
            {
                Assert.Fail($"fail to read input data : {ex.Message}");
            }
            return default(T);

        }

        public static string env_setup(string env, string path)
        {
            try
            {
                if (path.Contains("ExternalAPITEST"))
                {
                    path = path.Split("\\ExternalAPITEST")[0] + "\\APITest.Utils\\Data\\";
                }
                else
                {
                    path = path.Split("\\CloudPrintAPI")[0] + "\\APITest.Utils\\Data\\";
                }

                switch (env)
                {
                    case "dev01":
                    case "dev02":
                    case "dev03":
                    case "dev04":
                        path = path + env + ".json";
                        break;
                    case "devenv":
                    case "testenv":
                    case "prodenv":
                        path = path + env + "_EU.json";
                        break;
                    case "devenv.us":
                    case "testenv.us":
                    case "prodenv.us":
                        path = path +"//"+ env + "_US.json";
                        break;
                    default:
                        Assert.Fail($"Invalid input for environment {env}");
                        break;
                }

            }
            catch (Exception ex)
            {
                Assert.Fail($"failed to set up browser environment,{ex.Message}");
            }
            return path;
        }

        public static string GetLocalIPAddress()
        {
            IPAddress ipv4Address = null;
            try
            {
                // Get the host name of the local machine
                string hostname = Dns.GetHostName();

                // Get a list of IP addresses associated with the host
                IPAddress[] ipAddresses = Dns.GetHostAddresses(hostname);
                // Find the first IPv4 address in the list
                foreach (IPAddress address in ipAddresses)
                {
                    if (address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        ipv4Address = address;
                        break;
                    }
                }
                if (ipv4Address == null)
                {
                    Console.WriteLine("IPv4 Address not found.");
                    Assert.Fail($" IP4 Address: {ipv4Address} is not found on local machine");
                }

            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            return ipv4Address.ToString();


        }

        public static string GetLocalNetworkMAC()
        {
            PhysicalAddress macAddress = null;
            try
            {
                // Get the first network interface with an operational status.
                NetworkInterface nic = NetworkInterface.GetAllNetworkInterfaces().FirstOrDefault(x => x.OperationalStatus == OperationalStatus.Up);
                // Retrieve the MAC address of the selected network interface.
                if (nic != null)
                {
                    macAddress = nic.GetPhysicalAddress();
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            return macAddress.ToString();
        }

        public static string GetRandomName(string name)
        {
            try
            {
                name = name + GenerateRandomNumber().ToString();

            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            return name;
        }
        public static int GenerateRandomNumber(int min = 5, int max = 10)
        {
            int randomNumber = 0;
            try
            {
                Random rand = new Random();
                randomNumber = rand.Next(min, max);
                if (randomNumber == 0)
                {
                    Assert.Fail($"random number generation is failed and value :{randomNumber}");
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            return randomNumber;
        }

        public static long randomNumber()
        {
            Random random = new Random();
            long randomNumber = random.NextInt64(1000000000L, 9999999999L); // Generates a random 10-digit number
            return randomNumber;

        }

        public static string Get_DummyPrinter(string printername)
        {
            try
            {
                printername = $"\\\\{Environment.MachineName}\\{printername}";
            }
            catch (IOException ex)
            {
                Console.WriteLine("failed to get printer name for dummy");
                Assert.Fail(ex.Message);
            }
            return printername;
        }

        /* 
            new dummy printer based on vendor name 

        */
        public static string GeneratePrinterName(string vendorName)
        {
            // Generate a random number
            Random random = new Random();
            int randomNumber = random.Next(10, 999999);

            // Combine vendor name with the random number
            return $"{vendorName}_{randomNumber}";
        }

    }
}
