## Exercise 1 - Migrate to .NET Core 3
Migrating the application to .NET Core 3 is the best and recomanded path for modernizing a .NET application (WPF or Windows Forms). As previously mentioned, the first really nice improvment is about the startup and execution time! This is only the tip of the iceberg. The best advantage is that, the app will be able to use all the upcoming new features both from .NET Core and UWP! 

___ 

### Exercise 1 Task 1 - Setup for using .NET Core 3
At the moment of writing .NET Core is still in Preview and it is highly experimental technologies. Nevertheless, it is enough stable to play with it. The minimum required is made of two pieces:
- The .NET Core 3 runtime - [https://github.com/dotnet/core-setup](https://github.com/dotnet/core-setup)
- The .NET Core 3 SDK - [https://github.com/dotnet/core-sdk](https://github.com/dotnet/core-sdk)

Do not worry, using the VM provided, all is already setup for you: You do not have to download and install anything. On the other hand, if you are using you own computer, just navigate to the two links above and take the correct installer for your platform.

![Download .NET Core](https://github.com/Microsoft/AppConsult-WinAppsModernizationWorkshop/raw/master/Manual/Images/DownloadNETCore.png)

___ 

### Exercise 1 Task 2 - Perform the migration - The csproj for the WPF App
As mentioned, .NET Core is in the Preview state. Visual Studio 2019 has been released. If you need to install it on your own box, here is the link: [https://visualstudio.microsoft.com/downloads/](https://visualstudio.microsoft.com/downloads/).

Let's open the solution using Visual Studio 2019:
1.  In Windows Explorer, navigate to `C:\WinAppsModernizationWorkshop\Lab\Exercise1\01-Start\ContosoExpenses` and double click on the `ContosoExpenses.sln` solution.
    
    The project ContosoExpenses is now open in Visual Studio but nothing changed: The appllication still uses the Full .NET 4.7.2. To verify this, just right click on the project in the Solution Explorer Windows and **Properties**.
    
    ![Project properties in the Solution Explorer](https://github.com/Microsoft/AppConsult-WinAppsModernizationWorkshop/raw/master/Manual/Images/PropertiesContosoExpenses.png)

    The *Target framework* of the project is displayed in the **Application** tab.
    
    ![.NET Framework version 4.7.2 for the project](https://github.com/Microsoft/AppConsult-WinAppsModernizationWorkshop/raw/master/Manual/Images/NETFramework472.png)

2.  Right click on the project in the solution explorer and choose **Unload Project**.

    ![Unload project](https://github.com/Microsoft/AppConsult-WinAppsModernizationWorkshop/raw/master/Manual/Images/UnloadProject.png)

3.  Right click again on the project in the solution explorer ; click **Edit ContosoExpenses.csproj**.

    ![Edit ContosoExpenses csproj](https://github.com/Microsoft/AppConsult-WinAppsModernizationWorkshop/raw/master/Manual/Images/EditContosoExpensesCSPROJ.png)

4.  The content of the .csproj file looks like

    ![csproj file content](https://github.com/Microsoft/AppConsult-WinAppsModernizationWorkshop/raw/master/Manual/Images/CSPROJFile.png)

    Do not be afraid, it is not the time to understand the whole csproj structure. You will see that the migration will be done easily: Just remove all the content of the file by doing **CTRL+A** and than  **SUPPR**!
    
    ![Empty csproj file](https://github.com/Microsoft/AppConsult-WinAppsModernizationWorkshop/raw/master/Manual/Images/EmptyCSPROJ.png)
    
5.  Start writing the new csproj file content by typing `<Project Sdk="Microsoft.NET.Sdk.WindowsDesktop"> </Project>` in the ContosoExpense.csproj. Microsoft.NET.Sdk.WindowsDesktop is the .NET Core 3 SDK for applications on Windows Desktop. It includes WPF and Windows Forms.

    ![Windows Desktop in csproj](https://github.com/Microsoft/AppConsult-WinAppsModernizationWorkshop/raw/master/Manual/Images/WindowsDesktopInCSPROJ.png)

7.  Let's specify now a few details. To do this, insert a `<PropertyGroup></PropertyGroup>` element in inside the `<Project></Project>` element. 

    ![PropertyGroup inside Project in csproj](https://github.com/Microsoft/AppConsult-WinAppsModernizationWorkshop/raw/master/Manual/Images/PropertyGroup.png)

8.  First, we indicate that the project output is a **executable** and not a dll. This is acheived by adding `<OutputType>WinExe</OutputType>` inside `<PropertyGroup></PropertyGroup>`.

> Note that, if the project output was a dll, this line has to be omitted.

9.  Secondly, we specify that the project is using .NET Core 3: Just below the <OutputType> line, add ` <TargetFramework>netcoreapp3.0</TargetFramework>`

10. Lastly, we point out that this is a WPF application in adding a third line: `<UseWPF>true</UseWPF>`.

> If the application is Windows Forms, we do not need this third line.

#### Summary, verification and last step

- The project using .NET Core 3 and the **Microsoft.NET.Sdk.WindowsDesktop** SDK
- Output is an **application** so we need the `<OutputType>` element
- `<UseWPF>` is self-describing

Here is the full content of the new csproj. Please double check that you have everything:

```xml
<Project Sdk="Microsoft.NET.Sdk.WindowsDesktop">

  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>netcoreapp3.0</TargetFramework>
    <UseWPF>true</UseWPF>
  </PropertyGroup>

</Project>
```

By default, with the new project format, all the files in the folder are considered part of the solution. As such, we don't have to specify anymore each single file included in the project, like we had to do the old .csproj file. We need to specify only the ones for which we need to define a custom build action or that we want to exclude. 
It is now safe to save file by pressing **CTRL+S**.

___ 

### Exercise 1 Task 3 - Perform the migration - The csproj for the class library
The Contoso Expenses solution is using a class library for some models and interfaces for Services. The class library project is also a .NET 4.7.2 project. Let's see how to migrate the project to .NET Core 3:

1.  Right click on the **ContosoExpenses.Data** project in the solution explorer and choose **Unload Project**.

2.  Right click again on the project in the solution explorer ; click **Edit ContosoExpenses.Data.csproj**.

3. Remove all the content of the file by doing **CTRL+A** and than **SUPPR**.
    
4. Because we explained all part of the new XML tags for the .NET Core 3 project, just use the following content and save the file:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>netstandard2.0</TargetFramework>
  </PropertyGroup>

</Project>
```

> **Note:** This time, we use the .NET Standard 2.0 wich is highly portable/compatible with cross-platform apps. This way, you can see there that this class library could be shared with others apps on others platforms (MacOS, Linux) and still with existing full .NET apps on Windows.

___

### Exercise 1 Task 4 - Perform the migration - NuGet packages of the project

1.  The csproj is saved. Let's reopen the project: Go to the **Solution Explorer**, right click on the project and choose **Reload project**.

    ![Reload project in the Solution Explorer](https://github.com/Microsoft/AppConsult-WinAppsModernizationWorkshop/raw/master/Manual/Images/ReloadProject.png)
    
2.  Visual Studio just asks for a confirmation ; click **yes**.

    ![Confirmation for closing the csproj](https://github.com/Microsoft/AppConsult-WinAppsModernizationWorkshop/raw/master/Manual/Images/CloseCSPROJ.png)
    
3.  The project should load correctly. But remember: The NuGet packages used by the project were gone by removing all the content of the csproj! 

4.  You have a confirmation by expending the **Dependencies/NuGet** node in which you have only the .NET Code 3 package.

    ![NuGet packages](https://github.com/Microsoft/AppConsult-WinAppsModernizationWorkshop/raw/master/Manual/Images/NuGetPackages.png)
    
    Also, if you click on the **Packages.config** in the **Solution Explorer**. You will find the 'old' references of the NuGet packages used the project when it was using the full .NET Framework.
    
    ![Dependencies and packages](https://github.com/Microsoft/AppConsult-WinAppsModernizationWorkshop/raw/master/Manual/Images/Packages.png)
    
    Here is the content of the **Packages.config** file. You notice that all NuGet Packages target the Full .NET Framework 4.7.2:
    
    ```xml
    <?xml version="1.0" encoding="utf-8"?>
    <packages>
      <package id="Bogus" version="25.0.3" targetFramework="net472" />
      <package id="LiteDB" version="4.1.4" targetFramework="net472" />
      <package id="Microsoft.Toolkit.Wpf.UI.Controls" version="5.0.1" targetFramework="net472" />
      <package id="Microsoft.Toolkit.Wpf.UI.XamlHost" version="5.0.1" targetFramework="net472" />
    </packages>
    ```

5. Delete the file **Packages.config** by right clicking on it and **Delete** in the **Solution Explorer**.

6. Right click on the **Dependencies** node in the **Solution Explorer** and **Manage NuGet Packages...**

  ![Manage NuGet Packages...](https://github.com/Microsoft/AppConsult-WinAppsModernizationWorkshop/raw/master/Manual/Images/ManageNugetNETCORE3.png)

7. Click on **Browse** at the top left of the opened window and search for `Bogus`. The package by Brian Chavez should be listed. Install it.

    ![Bogus NuGet package](https://github.com/Microsoft/AppConsult-WinAppsModernizationWorkshop/raw/master/Manual/Images/Bogus.png)

8. Do the same for `LiteDB`. This package is provided by Mauricio David.

    ![LiteDB NuGet package](https://github.com/Microsoft/AppConsult-WinAppsModernizationWorkshop/raw/master/Manual/Images/LiteDB.png)

> Isn't it strange that we add the same packages as the ones used by the .NET Framework 4.7.2?

NuGet packages supports multi-targeting. You can include, in the same package, different versions of the library, compiled for different architectures. If you give a closer look at the packages' details, you will see that, other than supporting the full .NET Framework, it includes also a .NET Standard 2.0 version, which is perfect for .NET Core 3 (Further details on .NET Framework, .NET Core and .NET Standard at [https://docs.microsoft.com/en-us/dotnet/standard/net-standard](https://docs.microsoft.com/en-us/dotnet/standard/net-standard))

![Dot Net standard](https://github.com/Microsoft/AppConsult-WinAppsModernizationWorkshop/raw/master/Manual/Images/DotNetStandard.png)

> Since we don't have anymore a packages.config file, can you guess where the list of NuGet packages gets stored?

With the new project format, the referenced NuGet packages are stored directly in the .csproj file. You can check this by right clicking on the **ContosoExpenses** project in Solution Explorer and choosing **Edit ContosoExpenses.csproj**. You will find the following lines added at the end of the file:

```xml
  <ItemGroup>
    <PackageReference Include="Bogus" Version="25.0.4" />
    <PackageReference Include="LiteDB" Version="4.1.4" />
  </ItemGroup>
```

___ 

### Exercise 1 Task 4 - Perform the migration - A Preview NuGet package for Microsoft.Toolkit.Wpf.UI.Controls

1. Let's try to build it in order to 'discover' what we have to do to complete the migration. Use the **Build** menu and **Build solution**.

![](https://github.com/Microsoft/AppConsult-WinAppsModernizationWorkshop/raw/master/Manual/Images/BuildErrorsNETCore3.png)

> All these errors are caused by the same issue. What is it?

Again remember that we deleted all the content of the initial csproj file. We just had the **Bogus** and **LiteDB** NuGet Packages but not the **Microsoft.Toolkit.Wpf.UI.Controls**. There is a reason: go back to the **NuGet: ContosoExpenses** tab and search for `Microsoft.Toolkit.Wpf.UI.Controls`. You will see that this package supports the .NET Framework starting at the version 4.6.2. It does not support yet the .NET Core 3 version.

![](https://github.com/Microsoft/AppConsult-WinAppsModernizationWorkshop/raw/master/Manual/Images/WPFUICONTROLSNuGetPackage.png)

Because we are working with Preview versions in this lab, let's continue and add a custom source for NuGet Packages. 

2.  In the **NuGet: ContosoExpenses** tab, click on the  **Settings** icon for NuGet.

![](https://github.com/Microsoft/AppConsult-WinAppsModernizationWorkshop/raw/master/Manual/Images/SettingsForNuGet.png)

3. Click on the green "PLUS" sign to add a new NuGet Package source.

![](https://github.com/Microsoft/AppConsult-WinAppsModernizationWorkshop/raw/master/Manual/Images/AddNewNuGetSource.png)

4.  Name it `Custom` and give the url `https://dotnet.myget.org/F/uwpcommunitytoolkit/api/v3/index.json` ; Click **Ok**.

![](https://github.com/Microsoft/AppConsult-WinAppsModernizationWorkshop/raw/master/Manual/Images/CustomNuGetSource.png)

5. Still in the **NuGet: ContosoExpenses** tab, you can now change the Packages source with the dropdown listbox; Select **Custom**.

![](https://github.com/Microsoft/AppConsult-WinAppsModernizationWorkshop/raw/master/Manual/Images/ChangeSource.png)

6. Check also the **Include prerelease** checkbox and some NuGet packages will magically be displayed.

![](https://github.com/Microsoft/AppConsult-WinAppsModernizationWorkshop/raw/master/Manual/Images/PrereleaseNuGetPackages.png)

7. Select **Microsoft.Toolkit.Wpf.UI.Controls**. Please be sure to choose the version **6.0.0-build.15.ge5444fb4a5** before clicking **Install**. This version supports the .NET Core 3.0 runtime installed on the VM.

> For the users not using the VM, if you have downloaded the recently released Preview 2 of .NET Core 3.0, you can use the latest version of **Microsoft.Toolkit.Wpf.UI.Controls** provided by this custom source.

8.  Build the project (CTRL+SHIFT+B). We get still some errors that we will fix in the next tasks.

![](https://github.com/Microsoft/AppConsult-WinAppsModernizationWorkshop/raw/master/Manual/Images/NETCore3BuilldErrors.png)

___ 

### Exercise 1 Task 5 - Perform the migration - Fixing AssemblyInfo.cs

The Preview version of .NET Core 3 and Visual Studio 2019 causes the last 6 errors. It is not interesting to give explanations here: It is only 'piping' we have to resolve by either removing the mentioned lines in the `AssemblyInfo.cs` file or just delete the file. We go for the simpliest. 

1.  In the **Solution Explorer** window / Under the **ContosoExpenses** project, expand the **Properties** node and right click on the **AssemblyInfo.cs** file ; Click on **Delete**.
    
    ![AssemblyInfo cs file](https://github.com/Microsoft/AppConsult-WinAppsModernizationWorkshop/raw/master/Manual/Images/AssemblyInfoFile.png)

2.  Just rebuild the project (for example using CTRL+SHIFT+B): Only the last three previous errors should remain listed (if nothing is displayed in the **Error List** window, look at the **Output** window).

    ```dos
    1>------ Build started: Project: ContosoExpenses, Configuration: Debug Any CPU ------
    ...
    1>ExpenseDetail.xaml.cs(20,15,20,23): error CS0234: The type or namespace name 'Services' does not exist in the namespace 'Windows' (are you missing an assembly reference?)
    1>CalendarViewWrapper.cs(26,81,26,93): error CS0234: The type or namespace name 'CalendarView' does not exist in the namespace 'Windows.UI.Xaml.Controls' (are you missing an assembly reference?)
    1>CalendarViewWrapper.cs(26,127,26,168): error CS0234: The type or namespace name 'CalendarViewSelectedDatesChangedEventArgs' does not exist in the namespace 'Windows.UI.Xaml.Controls' (are you missing an assembly reference?)
    1>Done building project "ContosoExpenses_y1viyncj_wpftmp.csproj" -- FAILED.
    =======___ Build: 0 succeeded, 1 failed, 0 up-to-date, 0 skipped ==========
    ``` 

___ 

### Exercise 1 Task 6 - Perform the migration - Adding a reference to the Universal Windows Platform

This error is our fault because we removed everything in the csproj at the beginning of the exercise. 

> This method for migrating the project to .NET Core 3 is manual because Visual Studio 2019 Preview does not handle yet the migration work for us. The Visual Studio team is working to make the migration path easier and smoother in the future.

So to fix this error, we have to reference again the Universal Windows Platform again. This was done in the Exercise 2 Task 3. Here are the same steps:

In order to be able to use the Universal Windows Platform APIs in a WPF application we need to add a reference to two files:

- **Windows.md**, which contains the metadata that describes all the APIs of the Universal Windows Platform.
- **System.Runtime.WindowsRuntime** which is a library that contains the infrastructure required to properly support the **IAsyncOperation** type, which is used by the Universal Windows Platform to handle asynchronous operation with the well known async / await pattern. Without this library your options to interact with the Universal Windows Platform would be very limited, since all the APIs which take more than 50 ms to return a result are implemented with this pattern.

1. Go back to Visual Studio and right click on the **ContosoExpenses** project.
2. Choose **Add reference**.
3. Press the **Browse** button.
4. Look for the following folder on the system: `C:\Program Files (x86)\Windows Kits\10\UnionMetadata\10.0.17763.0\`
5. Change the dropdown to filter the file types from **Component files** to **All files**. This way, the `Windows.md` file will become visible.

    ![](https://github.com/Microsoft/AppConsult-WinAppsModernizationWorkshop/raw/master/Manual/Images/WindowsMd.png)
    
6. Select it and press **Add**.
7. Now press again the **Browse** button.
8. This time look for the following folder on the system: `C:\Windows\Microsoft.NET\Framework\v4.0.30319`
9. Look for a file called `System.Runtime.WindowsRuntime.dll`, select it and press Ok.
10. Now expand the **References** section of the **ContosoExpenses** project in Solution Explorer and look for the **Windows** reference.

    ![](https://github.com/Microsoft/AppConsult-WinAppsModernizationWorkshop/raw/master/Manual/Images/CopyLocalNETCore3.png)
   
11. Select it, right it click on it and choose **Properties**.
12. Change the value of the **Copy Local** property to **No**.
13. Rebuild the project (CTRL+SHIFT+B) and... you succeed!

```dos
=======___ Build: 1 succeeded, 0 failed, 0 up-to-date, 0 skipped ==========
```

___ 

### Exercise 1 Task 7 - Perform the migration - Debug

We are ok to finally, launch the app.

1.  Use the **Debug** menu / **Start Debugging F5**

> You had an exception. What is it that? Don't we finished the migration? Can you find the root cause of the issue reading the Exception Debug popup displayed by Visual Studio?

![Exception displayed in Visual Studio](https://github.com/Microsoft/AppConsult-WinAppsModernizationWorkshop/raw/master/Manual/Images/ExceptionNETCore3.png)

Strange because the images files are in the solution and the path seems correct.

![Images in the Solution Explorer](https://github.com/Microsoft/AppConsult-WinAppsModernizationWorkshop/raw/master/Manual/Images/ImagesInTheSolutionExplorer.png)

> Why do we get this file not found exception?

In fact, it is simple. Again, as we hardly deleted all the content of the csproj file at the beginning of the migration, we removed the information about the **Build action** for the images' files. Let fix it.

2.  In the **Solution Explorer**, select all the images files except the contoso.ico ; In the properties windows choose **Build action** = `Content` and **Copy to Output Directory** = `Copy if Newer`

    ![Build Action Content and Copy if newer](https://github.com/Microsoft/AppConsult-WinAppsModernizationWorkshop/raw/master/Manual/Images/ContentCopyIfNewer.png)

3.  To assign the Contoso.ico to the app, we have to right click on the project in the **Solution Explorer** / **Properties**. In the opened page, click on the dropdown listbox for Icon and select `Images\contoso.ico`

    ![Contoso ico in the Project's Properties](https://github.com/Microsoft/AppConsult-WinAppsModernizationWorkshop/raw/master/Manual/Images/ContosoIco.png)


We are done! Test the app in debug with F5 and it should work... Everything running using .NET Core 3!

We are now ready to go further and use all the power of the full UWP ecosystem controls, packages, dlls.

___ 

### Exercise 1 Task 8 - Supporting the Desktop Bridge
Before wrapping up the exercise, let's make sure that also the Desktop Bridge version of our WPF application based on .NET Core works fine, so that we can leverage all the UWP APIs and the deep Windows 10 integration also with our migrated WPF project.

1. Right click on the **ContosoExpenses.Package** project and choose **Set as StartUp Project**.
2. Right click on the **ContosoExpenses.Package** project and choose **Rebuild**.
3. The build operation will fail with the following error:

    ![](https://github.com/Microsoft/AppConsult-WinAppsModernizationWorkshop/raw/master/Manual/Images/DesktopBridgeNetCoreError.png)
    
    The error is happening because, when a .NET Core application is running packaged with the Desktop Bridge, it's included as self-contained, which means that the whole .NET Core runtime is embedded with the application. Thanks to this configuration, we can deploy the package on any Windows 10 machine and run it, even if it doesn't have the .NET Core runtime installed. However, when we package the application with the Desktop Bridge, we can't use the **Any CPU** compilation architecture, but we need to specify which runtimes we support. As such, we need to add this information in the **.csproj** file of our WPF project.
4. Right click on the **ContosoExpenses** project in Solution Explorer and choose **Edit ContosoExpenses.csproj**.
5. Add the following entry inside the **PropertyGroup** section:

    ```xml
    <RuntimeIdentifiers>win-x86;win-x64</RuntimeIdentifiers>
    ```
    
    This is how the full **PropertyGroup** should look like:
    
    ```xml
    <PropertyGroup>
      <OutputType>WinExe</OutputType>
      <TargetFramework>netcoreapp3.0</TargetFramework>
      <UseWPF>true</UseWPF>
      <ApplicationIcon />
      <RuntimeIdentifiers>win-x86;win-x64</RuntimeIdentifiers>
    </PropertyGroup>
    ```
    
    We are explictly saying that our WPF application can be compiled both for the x86 and x64 architectures for the Windows platform.
    
6. Now press CTRL+S, then right click again on the **ContosoExpenses.Package** and choose **Rebuild**.
7. This time the compilation should complete without errors. If you still see an error related to the **project.assets.json** file, right click on the **ContosoExpenses** project in Solution Explorer and choose **Open Folder in File Explorer**. Delete the **bin** and **obj** folders and rebuild the **ContosoExpenses.Package** project.
8. Now press F5 to launch the application.

Congratulations! You're running a .NET Core 3.0 WPF application!