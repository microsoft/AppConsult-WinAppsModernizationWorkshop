# Book Library

This is an application developed by [jbe2277](https://github.com/jbe2277) as a sample to demonstrate the power of Win Application Framework (WAF), a framework to create well-structured WPF applications. The original sample is part of the following repository [https://github.com/jbe2277/waf](https://github.com/jbe2277/waf).

### Important things to consider during the migration

1. Some libraries included in the solution are already targeting .NET Standard, so you won't have to migrate them.
2. The solution contains multiple applications and libraries which are targeting, instead, the full .NET Framework. They must be migrated to .NET Core 3.0.
3. The WAF framework has been ported to .NET Core, but it's available only [in a separate branch](https://github.com/jbe2277/waf/tree/netcoreapp3.0) of the original repository. It isn't available yet as a standalone NuGet package. However, the existing .NET Framework version will work just fine with the .NET Core 3.0 application in compatibility mode, so you can safely install the package from NuGet and ignore the warnings that will be prompted.
3. Make sure to specify the **AssemblyName** and the **RootNamespace** properties in both .csproj files, otherwise the project won't work as expected.
4. The application is modular, meaning that it's made by multiple applications loaded at runtime which aren't referenced in Visual Studio. As a consequence, the original applications' projects copy the build output to a shared folder placed in the root of the solution called **out**. When you migrate the projects to .NET Core 3.0 make sure to carry on also this configuration using the following attribute:



As a reference, in case you get stuck, you can find below the correct .csproj files.

**BookLibrary.Library.Presentation.csproj**
```xml
<Project Sdk="Microsoft.NET.Sdk.WindowsDesktop">

  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>netcoreapp3.0</TargetFramework>
    <UseWPF>true</UseWPF>
    <GenerateAssemblyInfo>false</GenerateAssemblyInfo>
    <RootNamespace>Waf.BookLibrary.Library.Presentation</RootNamespace>
    <AssemblyName>BookLibrary</AssemblyName>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="2.2.4" />
    <PackageReference Include="NLog" Version="4.6.2" />
    <PackageReference Include="System.ComponentModel.Annotations" Version="4.5.0" />
    <PackageReference Include="System.ComponentModel.Composition" Version="4.6.0-preview.19073.11" />
    <PackageReference Include="System.Waf.Wpf" Version="5.0.0" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\BookLibrary.Library.Applications\BookLibrary.Library.Applications.csproj" />
    <ProjectReference Include="..\BookLibrary.Library.Domain\BookLibrary.Library.Domain.csproj" />
  </ItemGroup>

  <ItemGroup>
    <Resource Include="Resources\Images\**" />
  </ItemGroup>
</Project>
```

**BookLibrary.Reporting.Presentation.csproj**
```xml
<Project Sdk="Microsoft.NET.Sdk.WindowsDesktop">
  <PropertyGroup>
    <TargetFramework>netcoreapp3.0</TargetFramework>
    <RootNamespace>Waf.BookLibrary.Reporting.Presentation</RootNamespace>
    <AssemblyName>Waf.BookLibrary.Reporting.Presentation</AssemblyName>
    <GenerateAssemblyInfo>false</GenerateAssemblyInfo>
    <UseWPF>true</UseWPF>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="NLog" Version="4.6.2" />
    <PackageReference Include="System.ComponentModel.Composition" Version="4.6.0-preview.19073.11" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\BookLibrary.Library.Domain\BookLibrary.Library.Domain.csproj" />
    <ProjectReference Include="..\BookLibrary.Library.Presentation\BookLibrary.Library.Presentation.csproj" />
    <ProjectReference Include="..\BookLibrary.Reporting.Applications\BookLibrary.Reporting.Applications.csproj" />
  </ItemGroup>

  <Target Name="PostBuild" AfterTargets="PostBuildEvent">
    <Exec Command="xcopy /y $(TargetDir)Waf.BookLibrary.Reporting.Applications.dll $(SolutionDir)BookLibrary.Library.Presentation\bin\Debug\netcoreapp3.0&#xD;&#xA;xcopy /y $(TargetDir)Waf.BookLibrary.Reporting.Presentation.dll $(SolutionDir)BookLibrary.Library.Presentation\bin\Debug\netcoreapp3.0" />
  </Target>
</Project>
```
