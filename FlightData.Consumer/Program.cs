using FlightData.DataFlow;
using FlightData.Entities;
using FlightData.Services;
using FlightData.Services.Utility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;


namespace FlightData
{
    class Program
    {
        public static IEventProcessor FlightDataProcessor { get; set; }
        public static BufferBlock<IList<string>> BufferBlock { get; set; }
        public static BlockingCollection<string> Queue = new BlockingCollection<string>();
        public static Task<ActionBlock<IList<RootData>>> DataTransformerBlock { get; set; }

        private static IConfiguration config;

        static void Main(string[] args)
        {
            MainAsync(args);
        }

        static void MainAsync(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            FlightDataProcessor = serviceProvider.GetService<IEventProcessor>();

            InitializeFileSystemWatcher();

            //Initialzie for first round 
            BufferBlock = FlightDataProcessor.GetBufferBlock();

            //Pass the buffer block to Pipeline mesh
            DataTransformerBlock = FlightDataProcessor.Process(BufferBlock);

            
             Timer timer = new Timer(new TimerCallback(Timer_CallBack), null, 2000,5000);
            

            Console.WriteLine(" =========================Consumer has started===============================");
            Console.WriteLine($"========Raw folder path is '{PathUtility.RawFolder}'");
            Console.WriteLine($"========Exception folder path is '{PathUtility.ExceptionFolder}'");
            Console.WriteLine($"========Curated folder path is '{PathUtility.CuratedFolder}'");
            Console.WriteLine($"========Log folder path is '{PathUtility.LogFolder}'");
            Console.WriteLine($"==========Program is observing the input folder {PathUtility.InputFolder}");
            while (true)
            {
                //Thread cancellation token is missing
            }

        }

        private static void ConfigureServices(IServiceCollection services)
        {
            IConfigurationBuilder configBuilder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            config = configBuilder.Build();

            services
            .AddSingleton<BaseValidationRules<Departure>, DepartureValidationRules>()
            .AddSingleton<BaseValidationRules<Arrival>, ArrivalValidationRules>()
            .AddSingleton<IDataFlow, FlightDataFlowBlock>()
            .AddSingleton<IValidationService, ValidationService>()
            .AddSingleton(provider => config)
            .AddSingleton<IEventProcessor, FlightEventProcessor>().BuildServiceProvider();

            services.AddLogging(configure => configure.AddSerilog());
            Log.Logger = new LoggerConfiguration()
              .WriteTo.File($"{PathUtility.LogFolder}\\log.txt", rollingInterval: RollingInterval.Minute, shared: true)
               .CreateLogger();
        }

        private static void Timer_CallBack(object state)
        {
            var files = Queue;
            Queue = new BlockingCollection<string>();
            BufferBlock.Post(new List<string>(files));
            BufferBlock.Complete();
            DataTransformerBlock.Wait();
            BufferBlock = FlightDataProcessor.GetBufferBlock();
            DataTransformerBlock = FlightDataProcessor.Process(BufferBlock);
           
        }

        private static void InitializeFileSystemWatcher()
        {
            FileSystemWatcher fileSystemWatcher = new FileSystemWatcher()
            {
                Path = PathUtility.InputFolder,
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime | NotifyFilters.Size | NotifyFilters.LastAccess | NotifyFilters.FileName | NotifyFilters.Attributes
            };

            fileSystemWatcher.IncludeSubdirectories = true;
            fileSystemWatcher.Filter = "*.*";
            fileSystemWatcher.Created += FileSystemWatcher_Created;
            fileSystemWatcher.EnableRaisingEvents = true;
        }

        private static void FileSystemWatcher_Created(object sender, FileSystemEventArgs e)
        {
            Queue.Add(e.FullPath);
            Console.WriteLine(e.FullPath);
        }

    }
}



