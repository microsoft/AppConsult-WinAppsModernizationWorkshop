## Exercise 2 - Use a 1st party UWP control with XAML Islands

We start with the simpliest modernization path possible: We would like to use a rich UWP control that is "*available for use in WPF*". Crazy idea? No! Indeed, the most requested controls are already wrapped for you! The current XAML Islands iteration brings you the InkCanvas, the InkToolbar, the MapControl and the MediaPlayerElement.
So in our Contoso Expenses application we will bring a modern touch by using InkCanvas and MapControl. This is possible thanks to the Microsoft.Toolkit.Wpf.UI.Controls NuGet package.

___ 

### Exercise 2 Task 1 - Setup the Contoso Expenses solution
Let's first be sure we can run and debug the Contoso Expenses solution locally.

1.  The source code of the Contoso Expenses solution is in the **Releases** tab of <a href="https://github.com/Microsoft/Windows-AppConsult-XAMLIslandsLab/tree/master/" target="_blank">Windows AppConsult XAMLIslandsLab repository</a>. A direct link for the download is `https://aka.ms/XAMLIslandsLab-Content`. Please use this url to donwload the zip file containing the lab content. 

2.  When ready, click on the downloaded file in your browser to open it.

    ![Downloaded file in Chrome](https://github.com/Microsoft/Windows-AppConsult-XAMLIslandsLab/raw/master/Manual/Images/SourceCodeDownloaded.png)

3.  Open the zip file and extract all the content to `C:\`. It will create a folder named `C:\XAMLI

4.  Launch Visual Studio 2019, and double click on the `C:\XAMLIslandsLab\Lab\Exercise1\01-Start\ContosoExpenses\ContosoExpenses.sln` file to open the solution.

    ![ContosoExpenses solution in Windows Explorer](https://github.com/Microsoft/Windows-AppConsult-XAMLIslandsLab/raw/master/Manual/Images/ContosoExpensesSolution.png)

5.  Verify that you can debug the Contoso Expenses WPF project by pressing the **Start** button or CTRL+F5.

___ 

### Exercise 2 Task 2 - Reference the "Microsoft.Toolkit.Wpf.UI.Controls" NuGet package
We need this WPF package because it takes care for us about all the necessary piping for XAML Islands. It provides wrapper classes for 1st party controls, such as the InkCanvas, InkToolbar, MapControl, and MediaPlayerElement, all for WPF.

Please note that the same package exists for Windows Forms. Its name is <a href="https://www.nuget.org/packages/Microsoft.Toolkit.Forms.UI.Controls/" target="_blank">Microsoft.Toolkit.Forms.UI.Controls</a>.

1.  If the Contoso Expenses solution is not opened in Visual Studio, double click on `C:\XAMLIslandsLab\Lab\Exercise2\01-Start\ContosoExpenses\ContosoExpenses.sln` (the folder where you have extracted the zipped file).
2.  Right click on the **ContosoExpenses** project in the Solution Explorer window on the left and choose **Manage NuGet Packages...**.

    ![Manage NuGet Packages menu in Visual Studio](Images/ManageNuGetPackages.png)

3. Search for `Microsoft.Toolkit.Wpf.UI.Controls`. The NuGet package from Microsoft.Toolkit will be displayed. Make sure to check the **Include prerelease** option. The current stable release (5.x), in fact, supports only the full .NET Framework, while the upcoming 6.x release (now in preview) includes support for .NET Core 3.0 as well.

    ![Microsoft.Toolkit.Wpf.UI.Controls NuGet package](Images/Microsoft.Toolkit.Wpf.UI.Controls.png)

4.  Click on the **Install** button on the right.

    ![Install Controls NuGet package](Images/InstallControlsNuGetPackage.png)

___ 

### Exercise 2 Task 3 - Use the InkCanvas control in the application
One of the features that the development team is looking to integrate inside the application is support to digital signature. Managers wants to be able to easily sign the expenses reports, without having to print them and digitalize them back.
'XAML Islands' is the perfect candidate for this scenario, since the Universal Windows Platform includes a control called **InkCanvas**, which offers advanced support to digital ink. Additionally, it includes many AI powered features, like the capability to recognize text, shapes, etc.

Adding it to a WPF application is easy, since it's one of the 1st party controls included in the Windows Community Toolkit we have just installed. Let's do it!

1. Go back to Visual Studio and double click on the **ExpenseDetail.xaml** file inside the **Views** folder in Solution Explorer.
2. As first step, we need to add a new XAML namespace, since the control we need is part of a 3rd party library. Locate the **Window** tag at the top of the XAML file.
3. Copy and paste the following definition as attribute of the **Window** element:

    ```xml
    xmlns:toolkit="clr-namespace:Microsoft.Toolkit.Wpf.UI.Controls;assembly=Microsoft.Toolkit.Wpf.UI.Controls"
    ```
    This is how the complete definition should look like:
    
    ```xml
    <Window x:Class="ContosoExpenses.ExpenseDetail"
            xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
            xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
            xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
            xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
            xmlns:toolkit="clr-namespace:Microsoft.Toolkit.Wpf.UI.Controls;assembly=Microsoft.Toolkit.Wpf.UI.Controls"
            DataContext="{Binding Source={StaticResource ViewModelLocator}, Path=ExpensesDetailViewModel}"
            xmlns:local="clr-namespace:ContosoExpenses"
            mc:Ignorable="d"
            Title="Expense Detail" Height="500" Width="800"
            Background="{StaticResource HorizontalBackground}">
    ```
4. Now we can add the **InkCanvas** control to the page. Move to the bottom of the XAML file and, inside the **Grid** control before the `</Grid>` and `<-- Chart -->` lines, add the following code:

    ```xml
    <TextBlock Text="Signature:" FontSize="16" FontWeight="Bold" Grid.Row="5" />
                
    <toolkit:InkCanvas x:Name="Signature" Grid.Row="6" />
    ```

    The first control is a simple **TextBlock**, used as a header. The second one is real **InkCanvas** control, which is prefixed by the **toolkit** keyword we have defined as namespace, being a 3rd party control.
    
5. That's it! Now we can test the application. Press F5 to launch the debugging experience.
6. Choose an employee from the list, then one of the available expenses.
7. Ops, this wasn't expected. The application will crash with the following exception:

    ```text
    WindowsXamlManager and DesktopWindowXamlSource are supported for apps targeting Windows version 10.0.18226.0 and later.  Please check either the application manifest or package manifest and ensure the MaxTestedVersion property is updated.
    ```

    XAML Islands with .NET Core 3.0 is supported only starting from Windows 10 1903, so we need to declare this requirement. We can do it using an application manifest.
8. Right click on the project in Solution Explorer and choose **Add -> New item**.
9. Look for the template called **Application Manifest File**. Name it **app.manifest** and press **Add**.
10. The file will be automatically opened inside Visual Studio. Look for the **compatibility** section and identify the commented entry prefixed by **Windows 10**:

    ```xml
    <!-- Windows 10 -->
    <!--<supportedOS Id="{8e0f7a12-bfb3-4fe8-b9a5-48fd50a15a9a}" />-->
    ```
11. Add the following entry below this item:

    ```xml
    <maxversiontested Id="10.0.18362.0"/>
    ```
    
12. Uncomment the **supportedOS** entry for Windows 10. This is how the section should look like:

    ```xml
    <!-- Windows 10 -->
    <supportedOS Id="{8e0f7a12-bfb3-4fe8-b9a5-48fd50a15a9a}" />
    <maxversiontested Id="10.0.18362.0"/>
    ```

13. Now right click on the **ContosoExpenses** project and choose **Properties**.
14. Make sure that, in the **Resources** section, the **Manifest** dropdown is set to **app.manifest**:

    ![](Images/NetCoreAppManifest.png)
    
15. Now press F5 and try again to click on an employee, then one of the expenses. Now the exception should be gone.
16. Notice that, in the expense detail page, there's a new space for the **InkCanvas** control. 

    ![](https://github.com/Microsoft/Windows-AppConsult-XAMLIslandsLab/raw/master/Manual/Images/InkCanvasPenOnly.png)

    If you have a device which supports a digital pen, like a Surface, and you're running this lab on a physical machine, go on and try to use it. You will see the digital ink appearing on the screen. However, if you don't have a pen capable device and you try to sign with your mouse, nothing will happen. This is happening because, by default, the **InkCanvas** control is enabled only for digital pens. However, we can change this behavior.
17. Stop the debugger and double click on the **ExpenseDetail.xaml.cs** file inside the **Views** folder in Solution Explorer.
18. Add the following namespace declaration at the top of the class:

    ```csharp
    using Microsoft.Toolkit.Win32.UI.Controls.Interop.WinRT;
    ```
    
12. Now locate the **ExpenseDetail()** method, which is the public constructor of the class.
13. Add the following line of code right after the **InitializeComponent()** method:

    ```csharp
    Signature.InkPresenter.InputDeviceTypes = CoreInputDeviceTypes.Mouse | CoreInputDeviceTypes.Pen;
    ```
    
    **InkPresenter** is an object exposed by the **InkCanvas** control which we can use to customize the default inking experience. Thanks to the **InputDeviceTypes** we can change which inking devices are supported. By using the values offered by the **CoreInputDeviceTypes** enumerator, we enable pen and mouse.
    
14. Now let's test the application again. Press F5 to start the debugging, then choose one of the employees followed by one of the expenses.
15. Try now to draw something in the signature space with the mouse. This time, you'll see the ink appearing on the screen.

    ![](https://github.com/Microsoft/Windows-AppConsult-XAMLIslandsLab/raw/master/Manual/Images/Signature.png)
    
Congratulations! You have added your first UWP control to a WPF application thanks to XAML Islands!