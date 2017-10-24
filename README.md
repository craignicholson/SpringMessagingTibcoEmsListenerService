# SpringMessagingTiboEmsListnerServer

This is a windows service which implments a Listener/Consumer for Tibco EMS
using the Spring.Messaging.Ems project.

## Quick Setup

- Create new Windows Service
- Rename Service1 to EmsListenerService
- Rename EmsListenerService.Designer.cs 

```C#

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            this.ServiceName = "Service1";
        }

```

to

```C#

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            this.ServiceName = "EmsListnerService";
        }

```

Click on EmsListenerService.cs [Design]
Toolbox Add Components > EventLog

### Add the Installer
https://docs.microsoft.com/en-us/dotnet/framework/windows-services/walkthrough-creating-a-windows-service-application-in-the-component-designer

To create the installers for your service
In Solution Explorer, open the context menu for MyNewService.cs or MyNewService.vb, and then choose View Designer.
Click the background of the designer to select the service itself, instead of any of its contents.
Open the context menu for the designer window (if you’re using a pointing device, right-click inside the window), and then choose Add Installer.
By default, a component class that contains two installers is added to your project. The component is named ProjectInstaller, and the installers it contains are the installer for your service and the installer for the service's associated process.
In Design view for ProjectInstaller, choose serviceInstaller1 for a Visual C# project, or ServiceInstaller1 for a Visual Basic project.
In the Properties window, make sure the ServiceName property is set to MyNewService.
Set the Description property to some text, such as "A sample service". This text appears in the Services window and helps the user identify the service and understand what it’s used for.
Set the DisplayName property to the text that you want to appear in the Services window in the Name column. For example, you can enter "MyNewService Display Name". This name can be different from the ServiceName property, which is the name used by the system (for example, when you use the net start command to start your service).
Set the StartType property to Automatic.
Installer Properties for a Windows Service
In the designer, choose serviceProcessInstaller1 for a Visual C# project, or ServiceProcessInstaller1 for a Visual Basic project. Set the Account property to LocalSystem. This will cause the service to be installed and to run on a local service account.

### Create EventLog Specific for this Application.


## Quick Install

Using Visual Studio Developer Command Prompt for VS 2017, run as Administrator

> installutil.exe SpringMessagingTibcoEmsListererService.exe  

If you see this error you need to run the installutil.exe as Administrator.

```bash

An exception occurred during the Install phase.
System.Security.SecurityException: The source was not found, but some or all event logs could not be searched.  Inaccessible logs: Security.

The Rollback phase of the installation is beginning.
See the contents of the log file for the C:\CSharp\source\SpringMessagingTiboEmsListererService\SpringMessagingTiboEmsListererService\bin\Debug\SpringMessagingTiboEmsListererService.exe assembly's progress.
The file is located at C:\CSharp\source\SpringMessagingTiboEmsListererService\SpringMessagingTiboEmsListererService\bin\Debug\SpringMessagingTiboEmsListererService.InstallLog.
Rolling back assembly 'C:\CSharp\source\SpringMessagingTiboEmsListererService\SpringMessagingTiboEmsListererService\bin\Debug\SpringMessagingTiboEmsListererService.exe'.
Affected parameters are:
   logtoconsole =
   logfile = C:\CSharp\source\SpringMessagingTiboEmsListererService\SpringMessagingTiboEmsListererService\bin\Debug\SpringMessagingTiboEmsListererService.InstallLog
   assemblypath = C:\CSharp\source\SpringMessagingTiboEmsListererService\SpringMessagingTiboEmsListererService\bin\Debug\SpringMessagingTiboEmsListererService.exe
Restoring event log to previous state for source EmsListenerService.
An exception occurred during the Rollback phase of the System.Diagnostics.EventLogInstaller installer.
System.Security.SecurityException: The source was not found, but some or all event logs could not be searched.  Inaccessible logs: Security.
An exception occurred during the Rollback phase of the installation. This exception will be ignored and the rollback will continue. However, the machine might not fully revert to its initial state after the rollback is complete.

The Rollback phase completed successfully.

The transacted install has completed.
The installation failed, and the rollback has been performed.


```

## Quick UnInstall

> installutil /u SpringMessagingTibcoEmsListererService.exe  



## Adding in Spring.Messaging NuGet