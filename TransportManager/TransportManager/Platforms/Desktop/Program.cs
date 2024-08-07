using Uno.UI.Runtime.Skia;

namespace TransportManager;

public class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        App.InitializeLogging();

        var services = new ServiceCollection();

        // Configure o DbContext
        services.AddDbContext<TransportDbContext>(options =>
            options.UseSqlite("Data Source=transport.db"));

        var serviceProvider = services.BuildServiceProvider();

        var host = SkiaHostBuilder.Create()
            .App(() => new App(serviceProvider))
            .UseX11()
            .UseLinuxFrameBuffer()
            .UseMacOS()
            .UseWindows()
            .Build();

        host.Run();
    }
}
