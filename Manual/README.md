# Modernize your .NET Framework application with .NET Core, XAML Islands, UWP and MSIX

## Introduction
Windows has introduced many great features to build modern applications: the Universal Windows Platform, which allows to leverage all the latest and greatest features like Windows Hello, Timeline, notifications, etc.; Fluent Design, which allows to create rich and beautiful user interfaces, with a special focus on accessibility and new interaction paradigms, like touch, inking or gaze; app packaging, which greatly simplifies the deployment of applications, allowing developers to focus on writing great code and leaving Windows to take care of all the rest: installation, update, uninstallation, etc.

In this lab we'll explore many of the technologies that will allow you to leverage all these enhancements in your existing WPF and Windows Forms application. You'll be able to enhance your .NET application with lot of new features without rewriting it from scratch, thanks to .NET Core 3.0, XAML Islands and MSIX.


### Objectives
- Learn why .NET Core 3.0 is important also for Windows desktop developers and how you can migrate your applications
- Learn how to modernize the user experience and the features of a desktop WPF application
- Learn how to leverage the Universal Windows Platform without having to rewrite the app from scratch
- Use a built-in XAML Islands control in an existing WPF application
- Be able to 'integrate' any custom UWP XAML component in the WPF application
- Understand how MSIX can improve your developer experience, by simplifying and enhancing the developer experience
 

### Prerequisites

- Experience in developing Windows Desktop applications with WPF
- Basic knowledge of C# and XAML
- Basic knowledge of UWP 

### Overview of the lab
We're going to start from an existing LOB application and we're going to enhance it by supporting modern features with the help of XAML Islands. We'll learn how to integrate Fluent controls from the Universal Windows Platform in the existing codebase.

The lab consists of five exercises:
1. In the first exercise you're going to migrate the WPF application to .NET Core 3.0, which will open up new and important scenarios in the future.
2. In the second exercise you're going to start modernizing the application by adding a UWP control which enables to digitally sign a document.
3. In the third exercise you're going to learn how you can add any native UWP control and interact with it.
4. In the fourth exercise you're going to leverage some APIs from the Universal Windows Platform in your application.
5. In the last exercise you're going to package your application with MSIX and to setup a CI/CD pipeline on Azure DevOps so that you can automatically deliver new versions of your app to your testers and users as soon as they comes out.

### Technical requirements
- Windows 10 1903 (build 18362)
- [Visual Studio 2019](https://www.visualstudio.com)
- [.NET Core 3 Preview SDK](https://dotnet.microsoft.com/download/dotnet-core/3.0)) 

Be aware that the following Visual Studio workloads have to be installed: 

- .NET desktop development
- Universal Windows Platform development

Make sure to check the option to install the 18362 SDK, as part of the Universal Windows Platform development workload:

![](Images/Windows10SDK.png)


### Scenario
Contoso Expenses is an internal application used by managers of Contoso Corporation to keep track of the expenses submitted by their reports. Modernizing this application is necessary in order to enhance employee efficiency when creating expenses reports. Many of the requested features could be easily implemented with the Universal Windows Platform. However, the application is complex and it's the outcome of many years of development by different teams. As such, rewriting it from scratch with a new technology isn't an option on the table. The team is looking for the best approach to add these features but, at the same time, reusing the existing codebase.

### The project
Contoso Expenses is a desktop application, built with WPF and .NET Framework 4.7.2. It's leveraging the following 3rd party libraries:

- MVVM Light, as a basic implementation for the MVVM pattern
- Unity, as a dependency injection container
- LiteDb, which is an embedded NoSQL solution to store the data
- Bogus, which is a tool to generate fake data


___ 

### Key concepts that will be used during the lab

**Please note**. The following information is provided in case you're planning to follow this lab on your own or from home. If you are following this lab as part of a live training class, feel free to skip it and jump directly to the beginning of the first exercise. These concepts, in fact, should have already be explained by the trainers of the lab before starting the practical exercises.

#### Universal Windows Platform
Starting from Windows 8, Microsoft has introduced a new kind of applications: Windows Store apps, based on a new framework called Windows Runtime. Unlike the .NET Framework, the Windows Runtime is a native layer of APIs which are exposed directly by the operating system to applications which want to consume them. With the goal to make the platform viable for every developer and to not force them to learn C\+\+, the Windows Runtime has introduced language projections, which are layers added on top of the runtime to allow developers to interact with it using well-known and familiar languages. Thanks to projections, developers can build applications on top of the Windows Runtime leveraging the same C# and XAML knowledge they have acquired in building apps with the .NET Framework. The Windows Runtime libraries (called Windows Runtime Components) are described using special metadata files, which make it possible for developers to access the APIs using the specific syntax of the language they’re using. This way, projections can also respect the language conventions and types, like uppercase if you use C# or camel case if you use JavaScript. Additionally, Windows Runtime components can be used across multiple languages: for example, a Windows Runtime component written in C++ can be used by an application developed in C# and XAML.
With the release of Windows 10, Microsoft has introduced the Universal Windows Platform, which can be considered the successor of the Windows Runtime since it’s built on top of the same technology. The most important feature of the Universal Windows Platform is that it offers a common set of APIs across every platform: no matter if the app is running on a desktop, on a Xbox One or on a HoloLens, you’re able to use the same APIs to reach the same goals. This is a major step forward compared to the Windows Runtime, which didn’t provide this kind of cross-device support. You were able to share code and UI between a PC project and a mobile project, but, in the end, developers needed anyway to create, maintain and deploy two different solutions.

The Universal Windows Platform has been built with security and privacy in mind. As such, Universal Windows Platform applications run inside a sandbox; they don’t have access to the registry; they can freely read and write data only in a specific local folder; etc. Any operation which is potentially dangerous requires the declaration of a capability and the consent of the user: some examples are accessing to the files in the Pictures library; using the microphone or the webcam; retrieving the location of the user; etc. Everything is controlled by a manifest file, which is an XML file that describes the identity of the application: its unique identifier, its capabilities, its visual aspect, its integration with the Windows 10 ecosystem, etc.

Last but not the least, all the investments of the Windows team for developers are focused on the Universal Windows Platform. All the latest features added in Windows 10, like Timeline, Project Rome, Windows Hello, etc. are exposed by the Universal Windows Platform, so that developers can integrate them in their applications.

#### MSIX packaging
With the introduction of Windows Store apps first and Universal Windows Platform apps later, Microsoft has also introduced a new packaging model called [MSIX](http://aka.ms/msix) (formerly known as AppX), which is very different from the existing deployment models (like MSI). It's completely controlled by the operating system; it can be used to deploy applications not only using traditional approaches, like the web, SSCM, Intune, but it opens up now opportunities like the Microsoft Store / Store for Business / Store for Education; it helps developers to focus on building great application, leaving all the installation, update and uninstallation tasks to the operating system; it helps IT Pros to be more agile and to modernize the deployment of enterprise applications.

With the release of Windows 10 Anniversary Update, this new format has been expanded to support not only modern applications, but also the existing ones built with traditional Win32 technologies, like WPF, Windows Forms, Java, Electron, etc. [Desktop Bridge](https://developer.microsoft.com/en-us/windows/bridges/desktop) is the name of the technology that has enabled this feature, allowing developers to release their Win32 applications also on the Microsoft Store. Another key feature of Desktop Bridge is that it enables Win32 applications to have an identity, which opens up the opportunity to consume a broader set of APIs from the Universal Windows Platform. 

When a Win32 application runs packaged as MSIX, it's executed inside a lightweight container which helps to improve the security and the reliability of the application. The container enables three features:

- A **Virtual File System**. Each package can contain a folder, called **VFS**, which maps all the main system folders, like *C:\Windows*, *C:\Program Files*, etc. When the application looks for a file in one of these folders, Windows will look first inside the Virtual File System and, only if it can't find it, will redirect the call to the real file system. Thanks to this feature you can create self-deployable packages, which don't need the user to manually install 3rd party dependencies like frameworks, libraries, etc. Additionally, you can solve the problem known as **DLL hell**, which can happen when you have on a system multiple applications which depend by different versions of the same system framework or library. Since you can bundle the most appropriate version of the framework in each package, it won't interfere with any other application or framework already installed on the machine.
- A **virtualized registry**. All the writing operation to the HKEY_CURRENT_USER hive of the registry are stored in a binary file which is unique for each application. This way the application doesn't have the opportunity to interfere with the real registry. And when the user uninstalls the application, the binary file is simply deleted, making sure that no orphan registry keys are left in the system.
- A **virtualized file system**. As a best practice in Windows development, all the data generated by an application which is tightly coupled to it (a database, log files, temporary files, etc.) should be saved in the AppData folder, which lives under the user's space. In a packaged application, all the reading and writing operations against the AppData folder are automatically redirected to the local folder, which is unique for an application. This approach helps to keep the system efficient and reliable since, when the user uninstalls the application, the local folder is simply deleted, making sure there are no orphan files left in the system.

MSIX packaging plays an important role with XAML Island because, by combining these two technologies, you'll be able to leverage at the same time features and user controls of the Universal Windows Platform without rewriting your application from scratch.

#### XAML Islands architecture
The Windows 10 October 2018 Update with the SDK 17763 has enabled a first preview of XAML Islands for Desktop applications, followed by an official release included in Windows 10 19H1. This means that Windows 10 now supports hosting UWP controls inside the context of a Win32 Process. The 'magic' is powered by two new system APIs called <a href="https://docs.microsoft.com/en-us/uwp/api/windows.ui.xaml.hosting.windowsxamlmanager" target="_blank">WindowsXamlManager</a> and <a href="https://docs.microsoft.com/en-us/uwp/api/windows.ui.xaml.hosting.desktopwindowxamlsource" target="_blank">DesktopWindowXamlSource</a>.

- The **WindowsXamlManager** handles the UWP XAML Framework. As such, the only exposed method is called **InitializeForCurrentThread()**, which takes care of initializing the UWP XAML Framework inside the current thread of a non-Win32 Desktop app, so that you can start adding UWP controls to it.
- The **DesktopWindowXamlSource** is the actual instance of your Islands content. It has a **Content** property which you can instantiate and set with the control you want to render. 

With an instance of the **DesktopWindowXamlSource** class you can attach its HWND to any parent HWND you want from your native Win32 App. This enables any framework that exposes HWND to host a XAML Island, including 3rd party technologies like Java or Delphi.
However, when it comes to WPF and Windows Forms applications, you don’t have to manually do that thanks to the Windows Community Toolkit, since it already wraps these classes into ready-to-be-used controls.

[The Windows Community Toolkit](https://docs.microsoft.com/en-us/windows/communitytoolkit/) is an open-source project, maintained by Microsoft and driven by the community, which includes many custom controls, helpers and service to speed up the development of Windows applications. Starting from version 5.0, the toolkit includes 4 packages to enable XAML Island: 

- One called **XamlHost**. It's a generic control that can host any UWP control, either custom or native. It comes in two variants: [Microsoft.Toolkit.Wpf.UI.XamlHost](https://www.nuget.org/packages/Microsoft.Toolkit.Wpf.UI.XamlHost/) for WPF and [Microsoft.Toolkit.Forms.UI.XamlHost](https://www.nuget.org/packages/Microsoft.Toolkit.Forms.UI.XamlHost/) for Windows Forms.
- One called **Controls**, which includes wrappers for 1st party controls like Map or InkCanvas. Thanks to these controls, you'll be able to leverage them like if they're native WPF or Windows Forms control, including direct access to the exposed properties and binding support. Also in this case, it comes into two variants: [Microsoft.Toolkit.Wpf.UI.Controls](https://www.nuget.org/packages/Microsoft.Toolkit.Wpf.UI.Controls/) for WPF and [Microsoft.Toolkit.Forms.UI.Controls](https://www.nuget.org/packages/Microsoft.Toolkit.Forms.UI.Controls/) for Windows Forms.

#### Backward compatibility
XAML Islands is supported starting from Windows 10 1809. Trying to run an application which uses XAML Island on a previous version of Windows will cause an exception.
If you need to handle backward compatibility, right now the only option is to instantiate the control in code and not in XAML, using the approach described [in the following document](https://docs.microsoft.com/en-us/windows/communitytoolkit/controls/wpf-winforms/windowsxamlhost).
Since the XAML control is initialized in code, you have the opportunity to detect the version of the OS where the application is running and choose if you want to continue or, for example, replace it with a standard WPF control.

```csharp
if (//it's Windows 10 1809 or higher)
{
    WindowsXamlHost myHostControl = new WindowsXamlHost();

    Windows.UI.Xaml.Controls.Button myButton =
        UWPTypeFactory.CreateXamlContentByType("Windows.UI.Xaml.Controls.Button")
        as Windows.UI.Xaml.Controls.Button;
        
}
else 
{
    //do something else    
}
```

However, the XAML Islands team is planning to enhance the backward compatibility story, by allowing the various controls included in the toolkit to handle this scenario for you and be automatically instantiated only if the app is running on a supported operating system.

The only exception to this rule is the **WebView** control. The Windows Community Toolkit, in fact, includes a control called [WebViewCompatible](https://docs.microsoft.com/en-us/windows/communitytoolkit/controls/wpf-winforms/webviewcompatible), which offers built-in support for backward compatibility. If the application is running on Windows 10 1803 or later, it will render the web view using the new UWP control and the Edge engine. Otherwise, it will fallback to the traditional **WebBrowser** control, which uses the Internet Explorer engine.

#### .NET Core 3
.NET Core is a open-source framework built from scratch which brings all the goodies of the .NET Framework into the new modern world. Unlike the full .NET Framework, which has its roots deeply integrated into Windows, .NET Core is cross-platform, lightweight and easily extensible.

Until today, .NET Core has always been focused on supporting these new requirements. As such, its primary workload has always been web or back-end applications. Thanks to .NET Core, you can easily build scalable web applications or APIs that can be hosted on Windows, Linux, or in micro services architectures like Docker containers.

At BUILD 2018 we have announced the next major release of .NET Core, 3.0, which is, without any doubts, the biggest and most ambitious release since the first version. On top of .NET Core 3.0, in fact, you'll be able to build new workloads.

![.NET Core workloads](https://github.com/Microsoft/AppConsult-WinAppsModernizationWorkshop/raw/master/Manual/Images/NETCoreWorkloads.png)

As you can see from the image, for the first time .NET Core will support not just web and back-end applications, but also desktop ones which, until today, have always been part only of the full traditional .NET Framework.

> **Disclaimer:** This doesn't mean that WPF and Windows Forms will become cross-platform and you'll be able to run a Windows desktop application, as it is, also on Linux and MacOS. The UI piece of the two frameworks still has a dependency on the Windows rendering system and, as such, it can't run on platforms which use instead a different visual rendering system.
> 

Let's take a look at the most important benefits of running a desktop application on top of .NET Core.

##### Performance improvements.
Key investments in .NET Core were made around performances. Startup time is much faster and most of the APIs have been rewritten to be fully optimized. This is true for server side and client side workloads.

##### Side-by-side support
One of the biggest blockers for enterprises to adopt newer versions of the .NET Framework is that it can be installed only at system level and it automatically comes with newer version of Windows. This means that if you have an application which targets .NET Framework 4.5 and you want to update it to take advantage of some of the improvements delivered by .NET Framework 4.7, you are forced to update all the applications (or, at least, to make sure they still run well) at the same time. The reason is that you can't install the .NET Framework 4.7 side-by-side with .NET Framework 4.5, but you have to update the existing 4.5 installation. This isn't a nightmare only for enterprises, but a big blocker also for Microsoft. If you look at the recent history of .NET Framework, you will notice how every upgrade brings mainly fixes and minor improvements. The reason is that, since we need to make sure to maintain backward compatibility, the team can't be agile and evolve the framework with changes that, potentially, can break older apps. Checking new code into the .NET Framework requires a long validation and testing period. You can read some thoughts from the team on this [in the following article](https://blogs.msdn.microsoft.com/dotnet/2018/10/04/update-on-net-core-3-0-and-net-framework-4-8/).

.NET Core, instead, can run truly side-by-side, with two different approaches:

- You can embed the runtime inside the application. This way you'll be able to deploy the app on any machine, even without the runtime installed, and make sure it will target the specific .NET Core version you have used to build it.
- You can install multiple .NET Core runtimes on the same machine. Unlike with the .NET Framework, you can have on the same machine .NET Core 1.0, .NET Core 2.0, .NET Core 3.0 and any .NET Core version will ship in the future. This means that if you deploy an application which runs on .NET Core 2.0, it will effectively leverage the .NET Core 2.0 runtime and not another runtime in backward compatibility mode.

Additionally, you will be able to leverage many of the benefits of the .NET Core ecosystem, like the opportunity to use the command line tools to create and build your projects or to use the improved .csproj format. In the end, .NET Core 3.0 will bring some specific benefits for desktop development, like a better support to high DPI screens or the opportunity to leverage all the UWP APIs.

##### Why .NET Core 3 for XAML Islands
You may be wonder which role .NET Core plays here. All the long-term investments in .NET will be delivered to .NET Core, while the full .NET Framework will focus mainly on security updates and in supporting the latest networking protocols, security standards, and Windows features. You can learn more about the roadmap [here](https://blogs.msdn.microsoft.com/dotnet/2018/10/04/update-on-net-core-3-0-and-net-framework-4-8/).

As such, XAML Island is supported also on the .NET Framework, but all the long-terms investement in this technology will be focused on .NET Core.


Let's start to work!

___ 

### Exercise 0 - The source code
First, please download the source code and set up the active folder for all the upcoming exercises.

___ 

### Exercise 0 - Task 1 - Setup the Contoso Expenses solution
Let's first be sure we can run and debug the Contoso Expenses solution locally.

1.  The source code of the Contoso Expenses solution is in the **Releases** tab of <a href="https://github.com/Microsoft/AppConsult-WinAppsModernizationWorkshop" target="_blank">AppConsult WinAppsModernization workshop repository</a>. A direct link for the download is `https://aka.ms/WinAppsModernizationSourceCode`. Please use this url to donwload the zip file containing the lab content. 

2.  When ready, click on the downloaded file in your browser to open it.

3.  Open the zip file and extract all the content to `C:\`. It will create a folder named `C:\WinAppsModernizationWorkshop`

4.  Launch Visual Studio 2019, and double click on the `C:\WinAppsModernizationWorkshop\Lab\Exercise1\01-Start\ContosoExpenses\ContosoExpenses.sln` file to open the solution.

    ![ContosoExpenses solution in Windows Explorer](https://github.com/Microsoft/AppConsult-WinAppsModernizationWorkshop/raw/master/Manual/Images/ContosoExpensesSolution.png)

5.  Verify that you can debug the Contoso Expenses WPF project by pressing the **Start** button or CTRL+F5.

___

Here are all exercises illustrating the modernization journey for an application:

* <a href="https://github.com/Microsoft/AppConsult-WinAppsModernizationWorkshop/tree/master/Exercise1" target="_blank">Exercise 1 - Start with a full .NET WPF application and migrate it to .NET Core 3</a>
* <a href="https://github.com/Microsoft/AppConsult-WinAppsModernizationWorkshop/tree/master/Exercise2" target="_blank">Exercise 2 - Use a 1st party UWP control with XAML Islands</a>
* <a href="https://github.com/Microsoft/AppConsult-WinAppsModernizationWorkshop/tree/master/Exercise3" target="_blank">Exercise 3 - Integrate a custom UWP XAML component</a>
* <a href="https://github.com/Microsoft/AppConsult-WinAppsModernizationWorkshop/tree/master/Exercise4" target="_blank">Exercise 4 - Adding Windows 10 features to the application</a> 
* <a href="https://github.com/Microsoft/AppConsult-WinAppsModernizationWorkshop/tree/master/Exercise5" target="_blank">Exercise 5 - Package and deploy your application with MSIX</a> 