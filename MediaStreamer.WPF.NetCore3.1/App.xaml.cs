using MediaStreamer.Logging;
using MediaStreamer.RAMControl;
using Microsoft.Extensions.Configuration;
//using Grpc.Net.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
//using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using MediaStreamer.DataAccess.CrossPlatform;
using Microsoft.Extensions.DependencyInjection;

namespace MediaStreamer.WPF.NetCore3_1
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IHost _host;
        private volatile MainWindow _mainWindow;
        private IConfigurationRoot _configuration;
        public App()
        {
            _configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json")
#if DEBUG
                        .AddJsonFile("appsettings.Development.json")
#endif
            .Build();

            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) => {
                    string connectionStringSQLServer =
                        _configuration.GetConnectionString("SQLServer")
                        ?? throw new Exception("Не указан ConnectionString to SQLServer");
                    string serviceAddress = _configuration.GetConnectionString("GRPCService");

                    services.AddSingleton<MainWindow>()
                    .AddSingleton<ApplicationSettingsContext>((sp) =>
                        new ApplicationSettingsContext(_configuration,
                            new DbContextOptionsBuilder<ApplicationSettingsContext>()
                            .UseInMemoryDatabase("ApplicationDataContext")
                            .Options
                        ));
                    //services.AddSingleton(
                    //    (s) => GrpcChannel.ForAddress(serviceAddress)
                    //)
                    //.AddSingleton<ITcpClientService, TcpClientService>();

                    //services.AddSingleton<IDapperQueryRepository, DapperSQLQueryRepository>()
                    //.AddSingleton<IGRPCClientService, GrpcClientService>()
                    //.AddSingleton<IDapperRepository, DapperKernelRepository>()
                    //.AddDbContextFactory<SqlDbContext>(options =>
                    //    options.UseSqlServer(_configuration.GetConnectionString("SqlServer")))

                    //services
                    //    .AddLocalization(options => options.ResourcesPath = "Resources")
                    //    .AddSingleton<IConfigurationRoot>(_configuration)
                    //    .AddMemoryCache()
                })
                .Build();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            _host.Start();

            _mainWindow = _host.Services.GetRequiredService<MainWindow>();
            _mainWindow.Show();

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _host.StopAsync();
            _host.Dispose();
            base.OnExit(e);
        }

        private bool UseMinimalisticUI()
        {
            var sectionValue = _configuration.GetSection("UseMinimalisticUI")?.Value;
            return
                bool.Parse(sectionValue ?? "true")
                ;
        }
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                //var tsk = Task.Factory.StartNew(() => 
                Program._logger = new SimpleLogger()
                //); tsk.Wait()
                ;
                KillExistingAppWindows();

                // Crazy hack
                var args1 = Environment.GetCommandLineArgs()/*.CreateArgs().*/.Skip(1);
                if (args1 != null && args1.Count() > 0)
                {
                    var arguments1 = args1.Aggregate((a, b) => Path.Combine(a + Environment.NewLine + b));
                    Program._logger?.LogTrace($"cmd arguments 1 : {arguments1}");
                    Program._logger?.LogTrace("Arguments.Count() > 0, ok.");
                }
            }
            catch (Exception ex) {
                Program._logger?.LogError(ex.ToString()+ ex.Message);
            }
        }

        private static void KillExistingAppWindows()
        {
            try
            {
                int countOfAppsRunning = Process.GetProcessesByName(
                        Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)
                        ).Count();

                Program._logger?.LogDebug($"Count of already running instances of the app = {countOfAppsRunning}");

                if (countOfAppsRunning > 1) {
                    foreach (
                        var proc in Process.GetProcessesByName(Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location))
                        .Where(p => p.Handle != System.Diagnostics.Process.GetCurrentProcess().Handle
                        && p.Id != System.Diagnostics.Process.GetCurrentProcess().Id
                        ))
                    {
                        Program._logger?.LogDebug($"Killing the process <{proc.Id}>...");
                        proc.CloseMainWindow();
                    }
                    Program._logger?.LogDebug("Waiting for another instance to shut down...");
                    System.Threading.Thread.Sleep(450);
                }
            } catch {
                Program._logger?.LogError("Cannot close already running instances of the app. Check the permissions.");
            }
        }
    }
}
