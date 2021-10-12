using Avalonia;
using Avalonia.ReactiveUI;
using Pass;

AppBuilder
    .Configure<App>()
    .UsePlatformDetect()
    .LogToTrace()
    .UseReactiveUI()
    .StartWithClassicDesktopLifetime(args);