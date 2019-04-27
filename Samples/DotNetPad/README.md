# DotNetPad

This is an application developed by [jbe2277](https://github.com/jbe2277) as a sample to demonstrate the power of Win Application Framework (WAF), a framework to create well-structured WPF applications. The original sample is available at [https://github.com/jbe2277/dotnetpad](https://github.com/jbe2277/dotnetpad).

### Important things to consider during the migration

1. Some libraries included in the solution are already targeting .NET Standard, so you won't have to migrate them.
2. The solution contains multiple applications and libraries which are targeting, instead, the full .NET Framework. They must be migrated to .NET Core 3.0.
3. The WAF framework has been ported to .NET Core, but it's available only [in a separate branch](https://github.com/jbe2277/waf/tree/netcoreapp3.0) of the original repository. It isn't available yet as a standalone NuGet package. However, the existing .NET Framework version will work just fine with the .NET Core 3.0 application in compatibility mode, so you can safely install the package from NuGet and ignore the warnings that will be prompted.


In case you get stuck, you can find the already migrated project at the following link [https://github.com/Microsoft/AppConsult-WinAppsModernizationWorkshop/tree/master/Samples-NetCore/DotNetPad](https://github.com/Microsoft/AppConsult-WinAppsModernizationWorkshop/tree/master/Samples-NetCore/DotNetPad)