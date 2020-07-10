# GitHub Packages Test

This is a dummy solution to test GitHub Packages functionality.

- [home](https://github.com/features/packages)
- [documentation](https://help.github.com/en/github/managing-packages-with-github-packages)

These procedures can be followed to push NuGet packages to the Packages section of a GitHub repository (GitHub Package Repository, GPR in short).

See [this SO post](https://stackoverflow.com/questions/59445987/publishing-and-consuming-github-package-repository-with-nuget-unable-to-load-th/62809861#62809861).

## Setup

Before using GPR, you must [create a token in your GitHub account](https://help.github.com/en/github/authenticating-to-github/creating-a-personal-access-token-for-the-command-line):

1. go to GitHub;
2. click your avatar (top-right);
3. `settings`;
4. `developer settings`;
5. `personal access tokens`;
6. generate: check `write:packages`, `read:packages`, `delete:packages`. This will automatically check the repo permissions for your OAuth token;
7. Click `generate token`.

## Creating and Publishing Packages

1. set the `RepositoryUrl` and `RepositoryType` properties in your `.csproj` file to your target repository URL and `git` respectively, e.g.:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <!-- omissis ... -->
    <RepositoryUrl>https://github.com/vedph/cadmus_packages</RepositoryUrl>
    <RepositoryType>git</RepositoryType>
  </PropertyGroup>
</Project>

```

2. THIS IS DONE ONLY ONCE: you first need to register (once) the GPR feed with (from the package's project folder):

```bash
dotnet nuget add source --username YOURGITHUBUSERNAME --password YOURPERSONALACCESSTOKEN --name github https://nuget.pkg.github.com/YOURGITHUBREPO/index.json
```

For instance:

```
dotnet nuget add source --username Myrmex --password ...tokenhere... --name github https://nuget.pkg.github.com/vedph/index.json
```

If you need to install `nuget.exe`, download it from <https://www.nuget.org/downloads>. If you place it e.g. in `C:\Exe`, you can invoke it from the Windows Git Bash with `/c/Exe/nuget.exe`.

Also, you need to set the nuget API key:

```bash
nuget setapikey YOURGITHUBTOKEN -Source https://nuget.pkg.github.com/YOURGITHUBUSERNAME/index.json
```

This encrypts the key and saves it in a config file under your `%APPDATA%` folder. e.g. mine ends up in `C:\Users\dfusi\AppData\Roaming\NuGet\NuGet.Config`.

3. open the GitHub bash in your project folder, and **create the package** like this:

```bash
dotnet pack NAME.csproj -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
```

where `-c` selects the configuration. See [creating snupkg](https://github.com/NuGet/docs.microsoft.com-nuget/blob/master/docs/create-packages/Symbol-Packages-snupkg.md) for more, and the [dotnet pack reference](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-pack).

4. open the GitHub bash in your project folder, and **publish** like this:

```bash
dotnet nuget push --source github .\bin\Release\NAME.1.0.0.nupkg"
dotnet nuget push --source github .\bin\Release\NAME.1.0.0.snupkg"
```

## Consuming Packages

- [documentation](https://help.github.com/en/github/managing-packages-with-github-packages/configuring-dotnet-cli-for-use-with-github-packages#publishing-multiple-packages-to-the-same-repository)

Using packages from GitHub in your project is similar to using packages from nuget.org. Add your package dependencies to your .csproj file, specifying the package name and version.

1. for each client project, [add nuget.config to your project](https://help.github.com/en/github/managing-packages-with-github-packages/configuring-dotnet-cli-for-use-with-github-packages): add a `nuget.config` file in your project folder with a content like this:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
    <packageSources>
        <clear />
        <add key="github" value="https://nuget.pkg.github.com/OWNER/index.json" />
    </packageSources>
    <packageSourceCredentials>
        <github>
            <add key="Username" value="YOURUSERNAME" />
            <add key="ClearTextPassword" value="YOURTOKEN" />
        </github>
    </packageSourceCredentials>
</configuration>
```

To avoid storing token in this file, which would be saved in the repo, use a per-user or per-machine NuGet setting ([reference](https://docs.microsoft.com/en-us/nuget/consume-packages/configuring-nuget-behavior#environment-variables-in-configuration)): e.g. per-user:

```bash
nuget config -set GITHUB_PACKAGES_TOKEN=YOURTOKEN
```

This saves the setting per-user (the default option). Now, consume it like this (see <https://stackoverflow.com/questions/34318069/setting-an-environment-variable-in-a-nuget-config-file>):

```xml
<add key="Password" value="%GITHUB_PACKAGES_TOKEN%" />
```

2. to use a package:

```bash
dotnet add YOURPROJECT.csproj package YOURPACKAGE --version YOURPACKAGEVERSION
```
