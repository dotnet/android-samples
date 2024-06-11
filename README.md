# .NET for Android samples

This repository contains .NET for Android samples, showing usage of various
Android API wrappers from C#. Visit the [Android Sample Gallery](https://docs.microsoft.com/samples/browse/?term=dotnet-android)
to download individual samples.

See the [.NET MAUI Installation docs](https://docs.microsoft.com/en-us/dotnet/maui/get-started/installation) for setup instructions.

## Galleries

We love samples! Application samples show off our platform and provide a great way for people to learn our stuff. And we even promote them as a first-class feature of the docs site. You can find the sample galleries here:

- [MAUI Samples](https://learn.microsoft.com/samples/browse/?term=maui)

- [Android Samples](https://docs.microsoft.com/samples/browse/?term=dotnet-android)

## Sample Requirements

We welcome sample submissions, please start by creating an issue with your proposal.

Because the sample galleries are powered by the github sample repos, each sample needs to have the following things:

- **Screenshots** - a folder called Screenshots that has at least one screen shot of the sample on each platform (preferably a screen shot for every page or every major piece of functionality). For an example of this, see [android-p/AndroidPMiniDemo](https://github.com/xamarin/monodroid-samples/tree/master/android-p/AndroidPMiniDemo/Screenshots).

- **Readme** - a `README.md` file that explains the sample, and contains metadata to help customers find it. For an example of this, see [android-p/AndroidPMiniDemo](https://github.com/xamarin/monodroid-samples/blob/master/android-p/AndroidPMiniDemo/README.md). The README file should begin with a YAML header (delimited by `---`) with the following keys/values:

    - **name** - must begin with `.NET for Android -`

    - **description** - brief description of the sample (&lt; 150 chars) that appears in the sample code browser search

    - **page_type** - must be the string `sample`.

    - **languages** - coding language/s used in the sample, such as: `csharp`, `fsharp`, `vb`, `java`

    - **products**: should be `dotnet-android` for every sample in this repo

    - **urlFragment**: although this can be auto-generated, please supply an all-lowercase value that represents the sample's path in this repo, except directory separators are replaced with dashes (`-`) and no other punctuation.

    Here is a working example from [_android-p/AndroidPMiniDemo_ README raw view](https://raw.githubusercontent.com/xamarin/monodroid-samples/master/android-p/AndroidPMiniDemo/README.md).

    ```yaml
    ---
    name: .NET for Android - Android P Mini Demo
    description: "Demonstrates new display cutout and image notification features (Android Pie)"
    page_type: sample
    languages:
    - csharp
    products:
    - dotnet-android
    urlFragment: android-p-androidpminidemo
    ---
    # Heading 1

    rest of README goes here, including screenshot images and requirements/instructions to get it running
    ```

    > NOTE: This must be valid YAML, so some characters in the name or description will require the entire string to be surrounded by " or ' quotes.

- **Buildable solution and .csproj file** - the project _must_ build and have the appropriate project scaffolding (solution + .csproj files).

This approach ensures that all samples integrate with the Microsoft [sample code browser](https://learn.microsoft.com/samples/browse/?term=dotnet-android).


## Tips for .NET Migration

The goal here is to fully "modernize" the template for .NET and C# 11.

Compare a `dotnet new android` template named the same as the existing project.

1. If the root namespace doesn't match the project name, to get the
   existing code to compile, you may need:

```xml
<RootNamespace>Xamarin.AidlDemo</RootNamespace>
```

2. Update any dependencies, NuGet packages, etc.

3. Remove `android:versionCode`, `android:versionName`, `package`,
   `<uses-sdk/>`, and `<application label=""`. These are defined in
   the `.csproj` file.

4. Remove all unused using statements, since we now have
   `ImplicitUsings=enable`.

5. Fix all namespace declarations to use C# 10 file-scoped namespaces.

6. Build. Fix any warnings related to nullable reference types (`Nullable=enable`).

7. Run the app and ensure the sample still works.


## Porting to .NET

When porting a legacy sample to .NET, please make sure to preserve as
much history of the original sample as possible.  Some samples have
their project, source and resource files in the same directory where
the readme file, screenshot folder and other files not directly
related to the sample code reside.  Since .NET defaults to importing
all the files in the project directory as if they were part of the
project, the application code must first be moved to a subdirectory
(with the exception of the .sln file).

New subdirectory should use the same name as the solution file,
without the .sln extension.  After creating it **first** move all the
relevant files and directories (source code, project file(s), the
`Properties` and `Resources` directories etc), using the `git mv`
command to the newly created directory, modify the .sln file to update
project file path(s) and **commit** these changes.  This ensures that
further changes will preserve commit history.

Now the sample is ready for porting.  After creating new project file
(using `dotnet new android -n SampleName`) in a separate directory,
copy any necessary package and project references from the old
project, updating them as needed and after that replace the old
project file with the new one.  

A handful of useful tips (copied from the `dotnet` branch's README in
this repository):

  1. If the root namespace doesn't match the project name, to get the existing code to compile, you may need:

``` xml
<RootNamespace>Xamarin.AidlDemo</RootNamespace>

```
  2. Update any dependencies, NuGet packages, etc.
  3. Remove android:versionCode, android:versionName, package,
    <uses-sdk/>, and <application label="". These are defined in the
    .csproj file. 
  4. Remove all unused using statements, since we now have ImplicitUsings=enable.
  5. Fix all namespace declarations to use C# 10 file-scoped namespaces.
  6. Build. Fix any warnings related to nullable reference types (Nullable=enable).
  7. Run the app and ensure the sample still works.

Another collection of tips can be found [here](https://github.com/xamarin/xamarin-android/wiki/Migrating-Xamarin.Android-Applications-to-.NET-6)

## License

.NET (including the android-samples repo) is licensed under the [MIT license](./LICENSE).


## Code of conduct

This project has adopted the code of conduct defined by the [Contributor Covenant](https://contributor-covenant.org) to clarify expected behavior in our community. For more information, see the [.NET Foundation Code of Conduct](https://www.dotnetfoundation.org/code-of-conduct).
