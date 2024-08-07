using Serilog;
using Uno.Resizetizer;

namespace TransportManager;

public partial class App : Application {
  public static IServiceProvider? Services { get; private set; }

  public App(IServiceProvider serviceProvider) {
    // Initialize logging before any other operation
    InitializeLogging();
    this.InitializeComponent();
    Services = serviceProvider ?? throw new ArgumentNullException(
        nameof(serviceProvider)); // Ensure it's not null
  }

  protected Window? MainWindow { get; private set; }
  protected IHost? Host { get; private set; }

  internal static void InitializeLogging() {
    Log.Logger = new LoggerConfiguration()
                     .MinimumLevel.Debug()
                     .WriteTo.Console()
                     .WriteTo
                     .File("logs/transportmanager.txt",
                           rollingInterval: RollingInterval.Day)
                     .CreateLogger();
  }

  protected override void OnLaunched(LaunchActivatedEventArgs args) {
    var builder = this.CreateBuilder(args).Configure(
        host => host
#if DEBUG
                    // Switch to Development environment when running in DEBUG
                    .UseEnvironment(Environments.Development)
#endif
                    .ConfigureServices((context, services) => {
                      // Register your services
                      services.AddDbContext<TransportDbContext>(
                          options =>
                              options.UseSqlite("Data Source=transport.db"));

                      // Register other services, e.g.,
                      // services.AddSingleton<IMyService, MyService>();

                      // Register Serilog logger
                      services.AddSingleton(Log.Logger);
                    }));

    MainWindow = builder.Window;

#if DEBUG
    MainWindow.EnableHotReload();
#endif
    MainWindow.SetWindowIcon();

    Host = builder.Build();

        ApplyMigrations(Host);

    // Do not repeat app initialization when the Window already has content,
    // just ensure that the window is active
    if (MainWindow.Content is not Frame rootFrame) {
      // Create a Frame to act as the navigation context and navigate to the
      // first page
      rootFrame = new Frame();

      // Place the frame in the current Window
      MainWindow.Content = rootFrame;
    }

    if (rootFrame.Content == null) {
      // When the navigation stack isn't restored navigate to the first page,
      // configuring the new page by passing required information as a
      // navigation parameter
      rootFrame.Navigate(typeof(MainPage), args.Arguments);
    }
    // Ensure the current window is active
    MainWindow.Activate();
  }

  private static void ApplyMigrations(IHost host) {
        using var scope = host.Services.CreateScope();
        var dbContext =
            scope.ServiceProvider.GetRequiredService<TransportDbContext>();
        dbContext.Database.Migrate();
    }
}
