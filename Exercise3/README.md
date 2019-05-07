## Exercise 3 - Integrate a custom UWP XAML component
The company has recently gone after a big hardware refresh and now all the managers are equipped with a Microsoft Surface or other touch equipped devices. Many managers would like to use the Contoso Expenses application on the go, without having to attach the keyboard, but the current version of the application isn't really touch friendly. The development team is looking to make the application easier to use with a touch device, without having to rewrite it from scratch with another technology.
Thanks to XAML Islands, we can start replacing some WPF controls with the UWP counterpart, which are already optimized for multiple input experiences, like touch and pen.

The development team has decide to start modernizing the form to add a new expense, by making easier to choose the expense date with a touch device. The Universal Windows Platform offers a control called **CalendarView**, [which is perfect for our scenario](https://docs.microsoft.com/en-us/windows/uwp/design/controls-and-patterns/calendar-view). It's the same control that it's integrated in Windows 10 when you click on the date and time in the taskbar:

![](../Manual/Images/CalendarViewControl.png)

However, it isn't included as a 1st party control in the Windows Community Toolkit, so we'll have to use the generic XAML Host control.

When we have added the **InkCanvas** control in Exercise 2, we didn't need to interact in any way with the Universal Windows Platform. The Windows Community Toolkit, in fact, wraps for us all the relevant properties and types. However, when we start to use the generic XAML host control, we can't use this advantage anymore. We can use this control to host any kind of UWP control, either included in the platform or built by a 3rd party vendor, so we need access to the Universal Windows Platform to interact with it.

In order to start using Universal Windows Platform APIs in a WPF application we need to add a reference to two files:

- **Windows.md**, which contains the metadata that describes all the APIs of the Universal Windows Platform.
- **System.Runtime.WindowsRuntime** which is a library that contains the infrastructure required to properly support the **IAsyncOperation** type, which is used by the Universal Windows Platform to handle asynchronous operation with the well known async / await pattern. Without this library your options to interact with the Universal Windows Platform would be very limited, since all the APIs which take more than 50 ms to return a result are implemented with this pattern.

In the past this process wasn't really straightforward, because it required to manually dig into the file system and look for the folders where these files are deployed by the Windows 10 SDK. However, the team has recently released a NuGet package called **Microsoft.Windows.SDK.Contracts**  that does this job for us. Since it's directly referenced by the Windows Community Toolkit, it's already installed in our project. This means that we're good to go and we can use any UWP control without doing anything special.
___ 

### Exercise 3 Task 1 - Add the WindowsXamlHost control

1. Regardless of your starting point, the required NuGet package should be already installed. We can verify this by right clicking on the **ContosoExpenses** project in Solution Explorer, choosing **Manage NuGet packages** and moving to the **Installed** tab.

    ![Manage NuGet Packages menu in Visual Studio](../Manual/Images/ManageNuGetPackages.png)

2. We should see a packaged called **Microsoft.Toolkit.Wpf.UI.XamlHost**.

    ![Microsoft.Toolkit.Wpf.UI.XamlHost NuGet Package](../Manual/Images/XamlHostNuGetPackages.png)

    The package is already installed because the one we have installed for exercises 2, **Microsoft.Toolkit.Wpf.UI.Controls**, has a dependency on it. As such, when we have installed it in the previous exercises, NuGet automatically downloaded and installed also the **Microsoft.Toolkit.Wpf.UI.XamlHost** package.
3. Now we can start editing the code to add our control. Locate, in Solution Explorer, the file called **AddNewExpense.xaml** in the **Views** folder and double click on it. This is the form used to add a new expense to the list. Here is how it looks like in the current version of the application:

    ![](../Manual/Images/AddNewExpense.png)
    
    As you can notice, the date picker control included in WPF is meant for traditional computers with mouse and keyboard. Choosing a date with a touch screen isn't really feasible, due to the small size of the control and the limited space between each day in the calendar.
    
4. We can see the current date picker implemented using the standard WPF control towards the end of the XAML file:

    ```xml
    <DatePicker x:Name="txtDate" Grid.Row="6" Grid.Column="1" Margin="5, 0, 0, 0" Width="400" />
    ```

5. We're going to replace this control with the **WindowsXamlHost** one, which allows hosting any UWP control inside our WPF application. However, first, we need to add a new namespace to the page. Scroll to the top of the page, identify the **Window** tag and add the following attribute:

    ```xml
    xmlns:xamlhost="clr-namespace:Microsoft.Toolkit.Wpf.UI.XamlHost;assembly=Microsoft.Toolkit.Wpf.UI.XamlHost"
    ```

    This is how the full definition should look like:
    
    ```xml
    <Window x:Class="ContosoExpenses.Views.AddNewExpense"
            xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
            xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
            xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
            xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
            xmlns:xamlhost="clr-namespace:Microsoft.Toolkit.Wpf.UI.XamlHost;assembly=Microsoft.Toolkit.Wpf.UI.XamlHost"
            DataContext="{Binding Source={StaticResource ViewModelLocator},Path=AddNewExpenseViewModel}"
            xmlns:local="clr-namespace:ContosoExpenses"
            mc:Ignorable="d"
            Title="Add new expense" Height="450" Width="800"
            Background="{StaticResource AddNewExpenseBackground}">
    ```
    
6. Since the new control takes more space than the WPF one, let's also increase the height of the window to 800, by changing the **Height** attribute of the **Window** tag from 450 to 800:

    ```xml
    <Window x:Class="ContosoExpenses.Views.AddNewExpense"
            xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
            xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
            xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
            xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
            xmlns:xamlhost="clr-namespace:Microsoft.Toolkit.Wpf.UI.XamlHost;assembly=Microsoft.Toolkit.Wpf.UI.XamlHost"
            DataContext="{Binding Source={StaticResource ViewModelLocator},Path=AddNewExpenseViewModel}"
            xmlns:local="clr-namespace:ContosoExpenses"
            mc:Ignorable="d"
            Title="Add new expense" Height="800" Width="800"
            Background="{StaticResource AddNewExpenseBackground}">
    ```

7. Now replace the **DatePicker** control you have previously identified in the XAML page with the following one:

    ```xml
    <xamlhost:WindowsXamlHost InitialTypeName="Windows.UI.Xaml.Controls.CalendarView" Grid.Column="1" Grid.Row="6" Margin="5, 0, 0, 0" x:Name="CalendarUwp"  />
    ```

    We have added the **WindowsXamlHost** control, by using the **xamlhost** prefix we have just defined. The most important property to setup the control is **InitialTypeName**: you must specify the full name of the UWP control you want to host. In our case, we specify the full signature of the **CalendarView** control, which is **Windows.UI.Xaml.Controls.CalendarView**.


Now press F5 to build and run the application. Once it starts, choose any employee from the list, then press the **Add new expense** button at the bottom of the list. You will notice how the WPF DatePicker control has been replaced with a full calendar view, which is more touch friendly. 

![](../Manual/Images/CalendarViewWrapper.png)

However, the work isn't completed yet. We need a way to handle the selected date, so that we can display it on the screen and populate the **Expense** object we're going to save in the database.

___ 

### Exercise 3 Task 2 - Interact with the WindowsXamlHost control
Let's take a look [at the documentation](https://docs.microsoft.com/en-us/uwp/api/Windows.UI.Xaml.Controls.CalendarView) of the **CalendarView** control. There are two things which are relevant for our scenario:

- The **SelectedDates** property, which contains the date selected by the user.
- The **SelectedDatesChanged** event, which is raised when the user selects a date.
    
Let's go back to the **AddNewExpense.xaml** page and handle them.

> Can you guess which is the challenge here?

The **WindowsXamlHost** control is a generic host control for any kind of UWP control. As such, it doesn't expose any property called **SelectedDates** or any event called **SelectedDatesChanged**, since they are specific of the **CalendarView** control.
In order to implement our scenario, we need to move to the code behind and cast the **WindowsXamlHost** to the type we expect, in our case the **CalendarView** one. The best place to do is when the **ChildChanged** event is raised, which is triggered when the guest control has been rendered.

1. Double click on the **AddNewExpense.xaml** file under the **Views** folder in Solution Explorer in Visual Studio
2. Identify the **WindowsXamlHost** control you have previously added and subscribe to the **ChildChanged** event:

    ```xml
    <xamlhost:WindowsXamlHost InitialTypeName="Windows.UI.Xaml.Controls.CalendarView" Grid.Column="1" Grid.Row="6" Margin="5, 0, 0, 0" x:Name="CalendarUwp"  ChildChanged="CalendarUwp_ChildChanged" />
    ```
    
3. Before moving on to the code behind, we need to add a **TextBlock** control to display the value selected by the user. First add a new **RowDefinition** with **Height** equal to **Auto** at the end of the **Grid.RowDefinitions** collection of the main **Grid**. This is how the final setup should look like:

    ```xml
    <Grid.RowDefinitions>
        <RowDefinition Height="Auto"/>
        <RowDefinition Height="Auto"/>
        <RowDefinition Height="Auto"/>
        <RowDefinition Height="Auto"/>
        <RowDefinition Height="Auto"/>
        <RowDefinition Height="Auto"/>
        <RowDefinition Height="Auto"/>
        <RowDefinition Height="Auto"/>
        <RowDefinition Height="Auto"/>
    </Grid.RowDefinitions>
    ```

4. Copy and paste the following code after the **WindowsXamlHost** control and before the **Button** one at the end of the XAML page:

    ```xml
    <TextBlock Text="Selected date:" FontSize="16" FontWeight="Bold" Grid.Row="7" Grid.Column="0" />
    <TextBlock Text="{Binding Path=Date}" FontSize="16" Grid.Row="7" Grid.Column="1" />
    ```

5. Locate the **Button** control at the end of the XAML page and change the **Grid.Row** property from **7** to **8**:

    ```xml
    <Button Content="Save" Grid.Row="8" Grid.Column="0" Command="{Binding Path=SaveExpenseCommand}" Margin="5, 12, 0, 0" HorizontalAlignment="Left" Width="180" />
    ```
    
    Since we have added a new row in the **Grid**, we need to shift the button of one row.
    

6. Now let's start to work on the code. Identify in Solution Explorer the **AddNewExpense.xaml.cs** file inside the **Views** folder and double click on it.

7. First, we need to add some using on the top of the file in order to be able to manipulate the WindowsXamlHost control.

    ```csharp
    using Microsoft.Toolkit.Wpf.UI.XamlHost;
    ```

8. Now copy and paste the following event handler inside the class definition:

    ```csharp
    private void CalendarUwp_ChildChanged(object sender, System.EventArgs e)
    {
        WindowsXamlHost windowsXamlHost = (WindowsXamlHost)sender;
    
        Windows.UI.Xaml.Controls.CalendarView calendarView =
            (Windows.UI.Xaml.Controls.CalendarView)windowsXamlHost.Child;
    
        if (calendarView != null)
        {
            calendarView.SelectedDatesChanged += (obj, args) =>
            {
                if (args.AddedDates.Count > 0)
                {
                    Messenger.Default.Send<SelectedDateMessage>(new SelectedDateMessage(args.AddedDates[0].DateTime));
                }
            };
        }
    }
    ```

    We are handling the **ChildChanged** event we have previously subscribed to. As first step, we retrieve a reference to the **WindowsXamlHost** control which triggered it. The control exposes a property called **Child**, which hosts the UWP control we have assigned with the **InitialTypeName** property. We retrieve this property and we cast it to the type of control we're hosting, which in our case is **Windows.UI.Xaml.Controls.CalendarView**. Now we have access to the full UWP control, so we can subscribe to the **SelectedDatesChanged** event, which is triggered when the user selects a date from the calendar. Inside this handler, thanks to the event arguments, we have access to the **AddedDates** collection, which contains the selected dates. In our case we're using the **CalendarView** control in single selection mode, so the collection will contain only one element. 
    We need to pass the selected date to the ViewModel, since this is where the new **Expense** object is created and saved into the database. To achieve this goal we use the messaging infrastructure provided by MVVM Light. We're going to send a message called **SelectedDateMessage** to the ViewModel, which will receive it and set the **Date** property with the selected value.
    
13. The project won't currently compile, because we don't have a class called **SelectedDateMessage**. Let's create it!
14. Right click on the **Messages** folder in Solution Explorer and choose **Add -> Class**.
15. Name it **SelectedDateMessage**.
16. Add the following namespace at the top of the class:

    ```csharp
    using GalaSoft.MvvmLight.Messaging;
    ```
    
17. Let it inherit from the **MessageBase** class, then declare a **DateTime** property and initialize it through the public constructor. This should be the final look & feel of the class:

    ```csharp
    using GalaSoft.MvvmLight.Messaging;
    using System;
    
    namespace ContosoExpenses.Messages
    {
        public class SelectedDateMessage: MessageBase
        {
            public DateTime SelectedDate { get; set; }
    
            public SelectedDateMessage(DateTime selectedDate)
            {
                this.SelectedDate = selectedDate;
            }
        }
    }
    ```

18. Now we need to receive this message in the corresponding ViewModel, so that it can populate the **Date** property. Double click on the **AddNewExpenseViewModel.cs** file inside the **ViewModels** folder in Solution Explorer.
19. Look for the public constructor of the class at the beginning and, inside it, copy and paste the following code:

    ```csharp
    Messenger.Default.Register<SelectedDateMessage>(this, message =>
    {
        Date = message.SelectedDate;
    });
    ```
    
    We register to receive the **SelectedDateMessage**. Whenever we receive it, we extract the selected date from it through the **SelectedDate** property and we use it to set the **Date** property exposed by the ViewModel. Since this property is in binding with the **TextBlock** control we have added in 4, we'll be able to see in real time which is the date selected by the user.
20. We don't need to make any additional change. If we look at the **SaveExpenseCommand** (which takes care of saving the expense inside the database), we can notice how the **Date** property was already used to create the new **Expense** object. Since this property is now being populated by the **CalendarView** control through the message we have just created, we're good to go.

Let's test again the project:

1. Press F5 and launch the application
2. Choose one of the available employees, then click the **Add new expense** button.
3. Fill all the information in the form and choose one date from the new **CalendarView** control. Notice how, below the control, the selected date will be displayed as a string.
4. Press the **Save** button.
5. The form will be closed and, in the list of expenses, you will find the new one you have just created at the end of the list. Take a look at the first column with the expense date: it should be exactly the one you have selected in the **CalendarView** control.

We have replaced an existing WPF control with a newer mordern version, which fully supports mouse, keyboard, touch and digital pens. Despite the fact that it isn't included as 1st party control in the Windows Community Toolkit, we've been able anyway to include a **CalendarView** control in our application and to interact with it.