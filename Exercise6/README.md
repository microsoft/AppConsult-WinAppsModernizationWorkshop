### Exercise 6 - Package and deploy your application with MSIX
In the previous exercise we have seen how the usage of some APIs from the Universal Windows Platform require the application to have an identity. This goal can be achieved by packaging it using MSIX, the new format introduced in Windows 10 to package and deploy Windows applications. It's the successor of the AppX format, which was launched in Windows 8 to deploy Store apps; now it has been enhanced to deploy any kind of application and to support any kind of distribution: Store, web, enterprise tools like SSCM and Intune, etc.
MSIX brings many advantages to the table both for IT Pros and developers, like:

- Optimized network usage and storage space
- Complete clean uninstall, thanks to a lightweight container where the app is executed. No registry keys and temporary files are left on the system.
- It empowers a modern IT environment, by decoupling OS updates, application updates and customization
- Simplifies the install, update and uninstall process. All these tasks are performed by Windows, so as a developer you can focus on building your application rather than maintaining the installer technology.

Additionally, thanks to a feature called AppInstaller it's easy to deploy applications on a website or a file share and offer automatic updates.

In this exercise we're going to learn how to package our WPF application as MSIX and how we can improve the deployment story thanks to Azure DevOps.

### Exercise 6 Task 1 - Package the application
Visual Studio offers an easy way to package an existing application thanks to a new template called Windows Application Packaging Project. Let's add it!

1. Go back to Visual Studio. 
2. Right click on the **ContosoExpenses** solution in **Solution Explorer** and choose **Add -> New project**.

    ![](../Manual/Images/AddNewProject.png)

3. Search for `packaging` and look for a template called **Windows Application Packaging Project** in the C# category:

    ![](../Manual/Images/WAP.png)
    
4. Name it `ContosoExpenses.Package` and press OK.
5. You will be asked which target SDK and minimum SDK you want to use:

    - **Target SDK** defines which APIs of the Universal Windows Platform you'll be able to use in your application. Choosing the most recent version will allow you to leverage all the latest and greates features offered by the platform.
    - **Minimum SDK** defines which is the minimum Windows 10 version you support. Lower versions won't be able to install this package. In case of a packaged Win32 application, the minimum supported version is Windows 10 Anniversary Update, since it was the first release to support the Desktop Bridge.
    
    For the purpose of our lab make sure to choose the most recent version for both options, as in the following picture:
    
    ![](https://github.com/Microsoft/Windows-AppConsult-XAMLIslandsLab/raw/master/Manual/Images/TargetSdk.png)
    
    Then press OK.
6. You will see a new project inside your Visual Studio solution, which structure will resemble the one of a Universal Windows Platform project:

    ![](../Manual/Images/WAPdetails.png)
    
    It has a manifest, which describes the application; it has some default assets, which are used for the icon in the Programs menu, the tile in the Start screen, the Store, etc. However, unlike a Universal Windows Platform project, it doesn't contain code. Its purpose is to package an existing Win32 application.
7. The project includes a section called **Applications**, which you can use to choose which applications included in your Visual Studio solution you want to include inside the package. Right click on it and choose **Add reference**.
8. You will see a list of all the other projects included in the solution which, currently, is only the **ContosoExpenses** application:

    ![](../Manual/Images/ReferenceManager.png)
    
    Make sure to select it and press Ok.
9. Now expand the **Applications** section. You will notice that the **ContosoExpense** project is referenced and highlighted in bold, which means that it will be used as a starting point for the package. In case of a project with multiple executables, you can set the starting point by clicking on the correct one in the **Applications** list and choosing **Set as entry point**.  However, this isn't our case, so we are ready to go on.
10. That's it! We can now test the packaged version of the application.
11. Right click on the **ContosoExpenses.Package** project in Solution Explore and choose **Set As Startup Project**.
12. Press F5 to launch the debugging. 
13. The packaging operation will fail with the following error:

    ![](../Manual/Images/DesktopBridgeNetCoreError.png)
    
    The error is happening because, when a .NET Core application is packaged as MSIX, it uses the self-contained deployment approach, which means that the whole .NET Core runtime is embedded with the application. Thanks to this configuration we can deploy the package on any Windows 10 machine and run it, even if it doesn't have the .NET Core runtime installed. However, when we do this we need to specify which runtimes we support, so that Visual Studio knows which version of .NET Core to include inside the package. We need to add this information in the **.csproj** file of our WPF project.
14. Right click on the **ContosoExpenses** project in Solution Explorer and choose **Edit ContosoExpenses.csproj**.
15. Add the following entry inside the **PropertyGroup** section:

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
    
16. Now press CTRL+S, then right click again on the **ContosoExpenses.Package** and choose **Rebuild**. Now the operation will complete without errors.
17. Press F5 to launch the packaged application.

Out of the box, you won't notice any meaningful difference. We have simply packaged our WPF application, so it's behaving like the traditional one. However, we can notice some small changes that can help us to understand the application is running as packaged:

- The icon in the taskbar or in the Start screen isn't anymore the icon of our application, but it's the default asset which is included in every UWP project.
- If we right click on the **ContosoExpense.Package** application listed in the Start menu, we will notice that we many options which are typically reserved to applications downloaded from the Microsoft Store, like **App settings**, **Rate and review** or **Share**.
    
    ![](../Manual/Images/StartMenu.png)

- If we want to remove the application from the system, we can just right click again on his icon in the Start menu and choose **Uninstall**. After pressing Ok, the application will be immediately removed, without leaving any leftover on the system.

### Exercise 6 Task 2 - Test the notification scenario
Now that we have packaged the application with MSIX, we can test the notification scenario which wasn't working at the end of Exercise 5.

1. Make sure that the **ContosoExpenses.Package** is still set as startup project.
2. Press F5 to launch the application.
3. Choose one employee from the list.
4. Press the **Add new expense** button.
5. Fill all the information about the expense and press **Save**.
6. This time you will see a notification appearing in the lower right corner.

![](../Manual/Images/ToastNotification.png)

### Exercise 6 Task 3 - Sign in to the Azure DevOps portal
In this task, you will learn how to start Azure DevOps for free, how to define builds to automatically run whenever a team member checks in code changes and to build pipelines to package the application to MSIX after the build runs.

![](../Manual/Images/Exercise6Task3Objectives.jpg)

Azure DevOps Services is a cloud service for collaborating on code development. It provides an integrated set of features that you access through your web browser or IDE client. The following features are included:

* Git repositories for source control of your code.
* Build and release services to support continuous integration and delivery of your apps.
* Agile tools to support planning and tracking your work, code defects, and issues using Kanban and Scrum methods.
* Many tools to test your apps, including manual/exploratory testing and continuous testing.
* Highly customizable dashboards for sharing progress and trends.
* Built-in wiki for sharing information with your team.

Azure DevOps Projects presents a simplified experience where you bring your existing code and Git repository or choose from one of the sample applications to create a continuous integration (CI) and continuous delivery (CD) pipeline to Azure. 

DevOps Projects sets up everything you need for developing, deploying, and monitoring your application. You can use the DevOps Projects dashboard to monitor code commits, builds, and deployments, all from a single view in the Azure portal.

1. Open the <a href="https://dev.azure.com/" target="_blank">Azure DevOps portal</a>.

2. Click on the **Start free** button to create a free account.

![](../Manual/Images/AzureDevOpsPortal.jpg)

3. Enter your email address, phone number, or Skype ID for your Microsoft account.


![](../Manual/Images/MicrosoftAccountSignIn.jpg)


4. After signing in, click on Continue to get started with Azure DevOps:


![](../Manual/Images/GetStartedAzureDevOps.jpg)

5. Enter the **Contoso Expenses** name for your project and select the visibility. The name can't contain special characters (such as / : \ ~ & % ; @ ' " ? < > | # $ * } { , + = [  ]), can't begin with an underscore, can't begin or end with a period, and must be 64 characters or less. Visibility can be either public or private. With public visibility, anyone on the internet can view your project. With private visibility, only people who you give access to can view your project. Select **Create project**.

![](../Manual/Images/AzureDevOpsCreateProject.jpg)

When your project has been created, the welcome page will appear. Feel free to explore and customize the new project.

![](../Manual/Images/AzureDevOpsWelcome.png)


### Exercise 6 Task 4 - Integrate Contoso Expenses with Azure DevOps Repository
In this session, we will learn how to integrate the ContosoExpenses solution to the project in Azure DevOps.

Note that so far the **Contoso Expenses** Repository is empty:

![](../Manual/Images/AzureDevOpsRepositoryEmpty.png)

We need to clone it to our computer. Cloning a repo creates a complete local copy of the repo for you to work with. Cloning also downloads all commits and branches in the repo and sets up a named relationship with the repo on the server. Use this relationship to interact with the existing repo, pushing and pulling changes to share code with your team.

Clone the repo to your computer:

1. Click on **Clone in Visual Studio** to clone this repository:

![](../Manual/Images/AzureDevOpsRepositoryCloneInVS.png)


2. Provide the **local path** for the repository and click on **Clone**:

![](../Manual/Images/AzureDevOpsRepositoryVS1.png)

After finishing to clone the Repository, the Team Explorer tab will be displayed, as follows:

![](../Manual/Images/AzureDevOpsRepositoryCloned.png)


The next step will be to add the Contoso Expenses solution to this repository.

3. Click on the **Show Folder view** option:

![](../Manual/Images/AzureDevOpsShowFolderView.png)

4. Right click in the **Contoso Expenses** folder or in a empty area and select **Open Folder in File Explorer**:

![](../Manual/Images/AzureDevOpsOpenFolderinFileExplorer.png)


Note that the local repository folder is empty and that exists only the local .vs folder used by Git tracking:

![](../Manual/Images/AzureDevOpsRepositoryLocalEmpty.png)

5. Copy the **Contoso Expenses** solution created in the previous exercise to the local repository.

![](../Manual/Images/AzureDevOpsRepositoryAddedSolution.png)

6. Switch back to Visual Studio and double-click on the **ContosoExpenses.sln**:

![](../Manual/Images/AzureDevOpsOpenSolution.png)

Observe that the solution was added to source-control:

![](../Manual/Images/AzureDevOpsSolutionAddedToSourceControl.png)

Before commit the changes, it is important to notice that not every file created or updated in your code should be committed to Git. Temporary files from development environment, test outputs and logs are all examples of files that are created but are not part of the codebase. Throught the gitignore feature it is possible to customize which files Git tracks.

7. Click on the **Team Explorer** tab and click on **Settings**:

![](../Manual/Images/AzureDevOpsRepositorySettings.png)

8. Click on **Repository Settings**:

![](../Manual/Images/AzureDevOpsRepositorySettings2.png)

9. Click on **Add** to create a default **.gitignore** file:

![](../Manual/Images/AzureDevOpsRepositoryGitIgnore.png)

More information about .gitignore file available at:
<a href="https://docs.microsoft.com/en-us/azure/devops/repos/git/ignore-files?view=azure-devops&tabs=command-line" target="_blank">Ignore file changes with Git</a>


10. Click on **Changes** to commit the changes, in this case, to commit all the solution files.


![](../Manual/Images/AzureDevOpsCommitChanges.png)

The commit changes UI will be displayed, asking to enter the commit message.


11. Enter the commit message and click on **Commit Staged** button:

![](../Manual/Images/AzureDevOpsCommitChanges.png)

12. Click on **Sync** option to share with the remotely server that there are changes in the local repository.

![](../Manual/Images/AzureDevOpsSync.png)

It is important to observe that so far, the commit and the changes exist only locally.

13. Click on **Push** to upload the changes to the server:

![](../Manual/Images/AzureDevOpsPush0.png)

Visual Studio will push your changes to the Azure DevOps repository:

![](../Manual/Images/AzureDevOpsPush.png)

At the end, the following message will be displayed to inform that the changes were successfully pushed to the server:

![](../Manual/Images/AzureDevOpsPush2.png)

14. Switch back to the Azure DevOps portal and click on Repos/Files to double-check that the files were upload to the server:

![](../Manual/Images/AzureDevOpsAddRepoFiles.png)

### Exercise 6 Task 5 - Create your first pipeline
Azure Pipelines helps you to implement a build, test, and deployment pipeline for any app. 

In this session, you will learn how to use Azure Pipelines to automatically build the **Contoso Expense** application every time that the changes are pushed to the repository.

1. – In the Azure DevOps portal, navigate to the **Pipelines** page. Then choose **New**, **New build pipeline**.

![](../Manual/Images/AzureDevOpsNewPipeline.png)

Alternatively, you can navigate to **Pipelines** page, click on **+** button besides the Contoso Expense project name and select **New build pipeline**:

![](../Manual/Images/AzureDevOpsNewPipeline2.png)

In the **Where is your code?** page, you can select your source code from different repository sources:

![](../Manual/Images/AzureDevOpsWhereIsYourpage.png)

2. To keep it simple, click on **Azure Repos Git** as we made the source code available in the Azure DevOps portal:

![](../Manual/Images/AzureDevOpsWhereIsYourpage2.png)

3. The following page will be displayed asking to select the **Contoso Expense** repository:

![](../Manual/Images/AzureDevOpsPipelineSelectRepository.png)


4. In the Configure your pipeline page, select the **Universal Windows Platform** pipeline option, as we want to generate the MSIX through the ContosoExpenses.Package project:

![](../Manual/Images/AzureDevOpsPipelineConfigure.png)

Azure Pipelines will analyze your repository. As it is the first time that we are configuring the pipeline to this project, the repository doesn't have the **azure-pipelines.yml** yet. Azure Pipelines recommends a starter template based on the code in your repository.

5. Click on **Save and run**.

![](../Manual/Images/AzureDevOpsPipelineReview.png)

6. Select **Commit directly to the master branch** and click on **Save and Run**:

![](../Manual/Images/AzureDevOpsPipelineSaveAndRun.png)

Wait for the pipeline finished to be configured:

![](../Manual/Images/AzureDevOpsPipelineConfiguring.png)

More information about the YAML file available at <a href="https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema?view=azure-devops&tabs=schema" target="_blank">YAML schema reference</a>.

The build will be started automatically:

![](../Manual/Images/AzureDevOpsPipelineBuild.png)

Wait for the build to finish. Observe that the build finished with errors.


![](../Manual/Images/AzureDevOpsPipelineBuildWithErrors.png)


Error message:
```text
C:\Program Files\dotnet\sdk\2.2.105\Sdks\Microsoft.NET.Sdk.WindowsDesktop\Sdk not found. Check that a recent enough .NET Core SDK is installed and/or increase the version specified in global.json.
```


This is happening as the **.NET Core 3.0 SDK** is in **preview** and doesn't exist yet in the Azure DevOps environment.



7. Click on **Pipelines** item and click on the **Edit** button to edit the **azure-pipelines.yml** file:

![](../Manual/Images/AzureDevOpsPipelineEdit.png)

The following page will be displayed with the content of the azure-pipelines.yml. Observe that there are a lot of extensions available to help to define the tasks:

![](../Manual/Images/AzureDevOpsAzurePipelineYmlFile.png)

8. Scroll down in the task list and click on **Use dotnet** task. That option will acquire a specific version of the .NET Core SDK from the Internet or the local cache and adds it to the PATH.

![](../Manual/Images/AzureDevOpsPipelineTaskUseDotnet.png)

9. Enter the version **3.0.100-preview3-010431** and click on **Add**:

![](../Manual/Images/AzureDevOpsPipelineTaskUseDotnetVersion.png)


Observe that the following code will be added to the yaml file:

```yaml
- task: DotNetCoreInstaller@0
  inputs:
    version: '3.0.100-preview3-010431'
```

10. You can include the **displayname** attribute to make the task more user friendly:

```yaml
- task: DotNetCoreInstaller@0
  displayName: 'Use .NET Core sdk 3.0.100-preview3-010431'
  inputs:
    version: '3.0.100-preview3-010431'
```

11. Click on the **Save** button:

![](../Manual/Images/AzureDevOpsPipelineSave1.png)

12. Click on **Save** again:

![](../Manual/Images/AzureDevOpsPipelineSave2.png)

13. Click on **Pipeline**, click on **Builds** and select the latest build:

![](../Manual/Images/AzureDevOpsPipelineBuildList.png)


Note that the build finished with the same error. Through the message error, it is possible to see that the compiler still using the **.NET Core SDK 2.2.105** instead of **.NET Core SDK 3.0**.

```text
C:\Program Files\dotnet\sdk\2.2.105\Sdks\Microsoft.NET.Sdk.WindowsDesktop\Sdk not found. Check that a recent enough .NET Core SDK is installed and/or increase the version specified in global.json.
```

To fix this error, it will be necessary to add the **global.json** file to the repository folder. If the **global.json** exists, the .NET Core SDK version, specified in the file, will be used for all SDK commands like **dotnet build**.

For more information about the global.json file check the <a href="https://docs.microsoft.com/en-us/dotnet/core/tools/global-json" target="_blank">global.json overview</a>.

14. Open the **local repository folder** of the **Contoso Expenses** project and add a **global.json** file with the following content:

```json
{

 "sdk": {

 "version": "3.0.100-preview3-010431"

 }

}
```

The local repository folder should looks like:

![](../Manual/Images/AzureDevOpsPipelineGlobalJson.png)


15. In the Visual Studio, open the **Contoso Expenses** solution and click on **Changes** available in the **Team Explorer** tab.

![](../Manual/Images/AzureDevOpsCommitChanges.png)


16. Make sure that the **global.json** is listed in the changes folder, fill in the **commit message** and click on **Commit All** button:

![](../Manual/Images/AzureDevOpsPipelineGlobalJsonCommit.png)

Now, it will be necessary to create a task to install .NET Core 3.0 in the Azure DevOps build environment.

17. Switch back to the **Azure DevOps portal**, click on **Pipeline** and click on the last build that was automatically started after committing the changes in Visual Studio.

![](../Manual/Images/AzureDevOpsPipelineBuildAfterGlobal.png)


Observe that this time, the build finishes with a different error telling us that the **MSBuild version**, of the build environment, is less than the required by the **Contoso Expenses** project:

![](../Manual/Images/AzureDevOpsPipelineError1.png)

Error message:

```text
Version 3.0.100-preview3-010431 of the .NET Core SDK requires at least version 16.0.0 of MSBuild. The current available version of MSBuild is 15.9.21.66.
```

That is happening because the **Contoso Expenses** is using **Visual Studio 2019 Preview**. Therefore, we need to change the pipeline image to **Windows Server 2019**, as it comes with the Visual Studio 2019 Preview installed.

18. Click on **Pipeline** and click on the **Edit** button to modify the yaml file:

![](../Manual/Images/AzureDevOpsPipelineEdit3.png)

19. Change the vmImage value from **VS2017-Win2016** to **windows-2019** and click on **Save**:

![](../Manual/Images/AzureDevOpsPipelineEdit4.png)

20. Commit the yaml file changes by clicking on **Save**:

![](../Manual/Images/AzureDevOpsPipelineSave2.png)

21. Click on **Pipeline**, click on **Builds** and select the latest build:

![](../Manual/Images/AzureDevOpsPipelineBuildList.png)

Note that this time, it was generated an error during the NuGetCommand task:

![](../Manual/Images/AzureDevOpsPipelineError2.png)

Error message:

```text
NU1605: Detected package downgrade: System.Collections from 4.3.0 to 4.0.11. Reference the package directly from the project to select a different version.
```

To fix that error, it is necessary to edit the yaml file and specify the NuGet version that we want to use.

22. Click on **Pipeline** and click on the **Edit** button to edit the yaml file:

![](../Manual/Images/AzureDevOpsPipelineEdit3.png)

23. Add the NuGet veersion, as follows:

```yaml
- task: NuGetToolInstaller@0
  displayName: 'Use NuGet 4.4.1'
  inputs:
    versionSpec: 4.4.1
```

Be aware with the indentation.


24. Click on **Save**.

![](../Manual/Images/AzureDevOpsPipelineEdit5.png)


25. Commit the yaml file changes by clicking on **Save**:

![](../Manual/Images/AzureDevOpsPipelineSave2.png)

26. Click on **Pipeline**, click on **Builds** and select the latest build:

![](../Manual/Images/AzureDevOpsPipelineBuildList.png)

The build will fail again, but this will be the last error. :)

![](../Manual/Images/AzureDevOpsPipelineError3.png)

Message error:

```text
Error APPX0104: Certificate file 'ContosoExpenses.Package_TemporaryKey.pfx' not found.
```


This error happened, because the compiler not found the certificate to sign the package.

To fix this error, it will be necessary to disable the AppxPackageSigningEnabled in the yaml file.

27. Click on **Pipeline** and click on **Edit** button to edit the yaml file:

![](../Manual/Images/AzureDevOpsPipelineEdit3.png)


28. In the last task, include the **/p:AppxPackageSigningEnabled=false** parameter, as follows:

```yaml
- task: VSBuild@1
  inputs:
    platform: 'x86'
    solution: '$(solution)'
    configuration: '$(buildConfiguration)'
    msbuildArgs: '/p:AppxBundlePlatforms="$(buildPlatform)" /p:AppxPackageDir="$(appxPackageDir)" /p:AppxBundle=Always /p:UapAppxPackageBuildMode=StoreUpload /p:AppxPackageSigningEnabled=false'
```

The yaml file should looks like:

![](../Manual/Images/AzureDevOpsPipelineFinalYaml.png)


29. Click on **Run** to queue a new build and click on the **build number** to go to the build details. Note that the build succeeded this time.

![](../Manual/Images/AzureDevOpsPipelineBuildSuccess.png)


Congrats! The build succeeded, but it is missing one thing here. Where are the build output? To access the output files, it will be necessary to add one more task.

30.  Click on **Pipeline** and click on the **Edit** button to edit the yaml file:

![](../Manual/Images/AzureDevOpsPipelineEdit3.png)


31.  Click on the last line of the yaml file, type **artifacts** in the **filter text** and click on **Publish Build Artifacts**:

![](../Manual/Images/AzureDevOpsPipelinePublishArtifacts.png)

32. Add the **\AppxPackages** at the end of the **Path to publish**, name the **Artifact name** field to **drop** and click on **Add**.

![](../Manual/Images/AzureDevOpsPipelinePublishArtifacts2.png)

Note that the task was included at the end of yaml file:

```yaml
- task: PublishBuildArtifacts@1
  inputs:
    PathtoPublish: '$(Build.ArtifactStagingDirectory)\AppxPackages'
    ArtifactName: 'drop'
```

Azure-pipelines.yml:

![](../Manual/Images/AzureDevOpsPipelinePublishArtifacts3.png)


33. Commit the yaml file changes by clicking on **Save**:

![](../Manual/Images/AzureDevOpsPipelineSave2.png)

34. Click on **Pipeline**, click on **Builds** and select the latest build:

![](../Manual/Images/AzureDevOpsPipelineBuildList.png)


After build succeeds, observe that the **Artifacts** button is available.  

![](../Manual/Images/AzureDevOpsPipelinePublishArtifacts4.png)

35. Click on the **Artifacts** button and select the **drop** item that contains the build output.

![](../Manual/Images/AzureDevOpsPipelinePublishArtifacts3.png)


In the **Artifacts explorer** UI it is possible to see all the output files generated:

![](../Manual/Images/AzureDevOpsPipelinePublishArtifacts6.png)

36. Click on the **...** button, beside the **drop** folder, to download the output files:

![](../Manual/Images/AzureDevOpsPipelinePublishArtifacts7.png)