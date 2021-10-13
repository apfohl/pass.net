using Avalonia;
using Pass;

AppBuilder
    .Configure<App>()
    .UsePlatformDetect()
    .LogToTrace()
    .StartWithClassicDesktopLifetime(args);