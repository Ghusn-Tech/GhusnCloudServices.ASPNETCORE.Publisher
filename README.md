# GhusnCloudServices.ASPNETCORE.Publisher

GhusnCloudServices.ASPNETCORE.Publisher is a .NET tool designed to automate the publishing of ASP.NET Core projects to a Linux-x64 target.

## Installation

You can install GhusnCloudServices.ASPNETCORE.Publisher globally using the following command:

```cmd
dotnet tool install --global GhusnCloudServices.ASPNETCORE.Publisher
```

## Usage

Once installed, you can use GhusnCloudServices.ASPNETCORE.Publisher in your ASP.NET Core project directory to publish the project. Run the following command:

```cmd
publish-project
```

This command will publish the project with the following setup:
- Destination: bin/Release/[TargetFramework]/publish/
- Target framework: Current project .NET version
- Deployment mode: Framework dependent
- Target runtime: linux-x64

## Requirements

- .NET Core 2.1 or later

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.