# Log Studio

Log Studio is a .NET Windows Forms application that takes as input a text log file with log row items that can be filtered and whose values can be plotted in charts.
The current version categorizes the log rows in a tree view.

## Getting started

These instructions will guide you in how to install the software and set up the project in your development environment for development and testing purposes.
See deployment for notes on how to deploy the project on a live system.

### Prerequisites

The software runs on Windows 7 and 10 operating system. Make sure that .NET Framework 4.6.1 is installed prior to installing Log Studio.

### Installing the software

Double-click the msi file located in the folder LogStudio.Setup\bin\Release and follow the instructions in the installer wizard.

### Usage

User's guide not provided yet, will be published later.

### Getting started with development

The project is organized in a Visual Studio solution file located in root folder/LogStudio/LogStudio.sln and can be opened in Visual Studio Community edition 2019 or one of the licensed editions, so you need to install it to be able to develop the software.

We cannot with certainty say how old versions of Visual Studio that can be used, so our recommendation is that you use Visual Studio 2019 for development.

We have used ReSharper during development, but it is not mandatory to use ReSharper.

The solution consists of these C# projects (add more descriptive text):

| C# project                    | Description                      |
|-------------------------------|----------------------------------------------------------------------|
| LogStudio                     | .NET Windows Forms project, the startup project                      |
| LogStudio.Reader              | Module that shows the data in rows in the Tab reader tab             |
| LogStudio.Reader.Tests        | Unit tests for LogStudio.Reader                                      |
| LogStudio.Data                | Parses the data in the file into objects                             |
| LogStudio.DataTest            | Unit tests for LogStudio.Data                                        |
| LogStudio.Graph               | Module that shows the data in graphs in the Graph tab                |
| LogStudio.Framework           | Framework for modules in LogStudio                                   |
| LogStudio.LogFileRepair       | CLI for repairing corrupted log files                                |
| LogStudio.LogFileGenerator    | CLI tool for outputting the log file events in console as live data  |
| LogStudio.Setup               | WiX setup project                                                    |
| LogStudio.Test                | Unit tests for LogStudio                                                    |

#### Log file format

The input to the application, an event log text file with file ending .plg, has the following format:

The log file is divided into two sections:

1. The items definition list
2. The actual log, where the defined items value changes are logged with a time stamp

##### The items definition list

This is an example of a row that defines an item:

```
#-IOItem: Name=Tank.Sensor.Temperature|DName=Sensor temperature|Type=Int32|Unit=°C|Format=|Axis=Linear|HiHi=0|Hi=0|Lo=0|LoLo=0
```

In this case the item name is delimited with punctuations, in three levels: "Top category"."Category"."Name.
After loading the log file in Log studio, the items are shown in a tree view control to the left, where the top categories are nodes that can be expanded, and so on.

| Id        | Description                                                                                                                                       |
|-----------|---------------------------------------------------------------------------------------------------------------------------------------------------|
| Name      | string with the name                                                                                                                              |
| DName     | string that contains the displayname                                                                                                              |
| Type      | the C# data type of the value being logged                                                                                                        |
| Unit      | string that describes the unit of the value, for example "mm"                                                                                     |
| Format    | format of the value (format string in C#)                                                                                                         |
| Axis      | Linear or Log10. If Linear, then values should be plotted in a linear coordinate system, if Log10 values should be plotted in a logarithmic scale |
| HiHi      | Upper limit for value (not used in Log Studio, default value 0)                                                                                   |
| Hi        | Upper limit for value (not used in Log Studio, default value 0)                                                                                   |
| Lo        | Lower limit for value (not used in Log Studio, default value 0)                                                                                   |
| LoLo      | Lower limit for value (not used in Log Studio, default value 0)                                                                                   |

##### The event log

```bash
Timestamp|Itemname|UserId|CycleId|Value
```

- Timestamp: the event timestamp in the format YYYY-MM-dd HH:mm:ss:fff
- Itemname: the name of an item taken from the items definition list
- UserId: the windows user
- CycleId: the values can be written during the same cycle id. The cycle ids are incremented throughout the event log, from the start of the logging to the end.
- Value: a float, double, integer, boolean, or string value. The value of the specific item in item name.  

Example:

```bash
2020-04-02 18:37:26.950|Tank.Sensor.Temperature|john.doe|12345|125
```

#### NuGet packages

These NuGet packages need to be installed in order to run the application from the IDE:

- Castle.Core
- CommandLineParser
- CSharp.ExpressionParser
- JetBrains.Annotations
- log4net
- Microsoft.Msagl
- Microsoft.Msagl.Drawing
- Microsoft.Msagl.GraphViewerGDI
- NSubstitute
- NUnit
- System.Threading.Tasks.Extensions
- System.ValueTuple
- WiX

The easiest way is to right-click the solution and choose 'Restore NuGet packages'.

Here is a document describing how to install nuget packages: <https://docs.microsoft.com/en-us/nuget/quickstart/install-and-use-a-package-in-visual-studio>

#### Tests

The unit tests in the project do not have full coverage. To run the tests in the IDE, please install the NUnit 3 Test Adapter extension in Visual Studio.

#### Deployment

You can create an installer by running the LogStudio.Setup project. It produces an .msi and a cabinet file for installation of the software.
We refer to the [WiX Toolset documentation](https://wixtoolset.org/) on how to make changes in the Setup project.

To facilitate changes in the setup project, you can install the Wix Toolset Visual Studio 2019 extension in Visual Studio and wix311.exe from [WiX Toolset downloads](https://wixtoolset.org/).

We do not yet have support for automatic updates.

## Support

Read the [contributing guidelines](CONTRIBUTING.md) for a description of how to report bugs.
If you have other questions, you can e-mail us, e-mail address below.

## Versioning

We use [SemVer](http://semver.org/) for versioning. For the versions available, see the [Log Studio repository](https://github.com/your/project/tags).

## Contributing

We welcome bug reports and suggestions for new features. However, a feature contribution needs to be approved in advance by our team.
Please read more in our [contributing guidelines](CONTRIBUTING.md).

## License

This project is licensed under the LGPL 3 License - see the [LICENSE.md](LICENSE.md) file for details.

## Contact

If you would like to get in touch with us you can send us an e-mail at github.arcamsoftware@ge.com. Please start the subject line with Log Studio.
You can also post a comment in the forum.
We usually respond within a week.
