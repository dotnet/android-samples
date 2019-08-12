# MonoDroid (Xamarin.Android) samples

This repository contains Mono for Android samples, showing usage of various
Android API wrappers from C#. Visit the [Android Sample Gallery](https://docs.microsoft.com/samples/browse/?term=Xamarin.Android)
to download individual samples.

## License

The Apache License 2.0 applies to all samples in this repository.

   Copyright 2011 Xamarin Inc

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.

## Contributing

Before adding a sample to the repository, please run either install-hook.bat
or install-hook.sh depending on whether you're on Windows or a POSIX system.
This will install a Git hook that runs the Xamarin code sample validator before
a commit, to ensure that all samples are good to go.

## Samples Submission Guidelines

## Galleries

We love samples! Application samples show off our platform and provide a great way for people to learn our stuff. And we even promote them as a first-class feature of the docs site. You can find the sample galleries here:

- [Xamarin.Forms Samples](https://docs.microsoft.com/samples/browse/?term=Xamarin.Forms)

- [iOS Samples](https://docs.microsoft.com/samples/browse/?term=Xamarin.iOS)

- [Mac Samples](https://docs.microsoft.com/samples/browse/?term=Xamarin.Mac)

- [Android Samples](https://docs.microsoft.com/samples/browse/?term=Xamarin.Android)

## Sample GitHub Repositories

These sample galleries are populated by samples in these GitHub repos:

- [https://github.com/xamarin/xamarin-forms-samples](https://github.com/xamarin/xamarin-forms-samples)

- [https://github.com/xamarin/mobile-samples](https://github.com/xamarin/mobile-samples)

- [https://github.com/xamarin/ios-samples](https://github.com/xamarin/ios-samples)

- [https://github.com/xamarin/mac-samples](https://github.com/xamarin/mac-samples)

- [https://github.com/xamarin/monodroid-samples](https://github.com/xamarin/monodroid-samples)

- [https://github.com/xamarin/mac-ios-samples](https://github.com/xamarin/mac-ios-samples)

The [mobile-samples](https://github.com/xamarin/mobile-samples) repository is for samples that are cross-platform.
The [mac-ios-samples](https://github.com/xamarin/mac-ios-samples) repository is for samples that are Mac/iOS only.

## Sample Requirements

We welcome sample submissions, please start by creating an issue with your proposal.

Because the sample galleries are powered by the github sample repos, each sample needs to have the following things:

- **Screenshots** - a folder called Screenshots that has at least one screen shot of the sample on each platform (preferably a screen shot for every page or every major piece of functionality). For an example of this, see [android-p/AndroidPMiniDemo](https://github.com/xamarin/monodroid-samples/tree/master/android-p/AndroidPMiniDemo/Screenshots).

- **Readme** - a `README.md` file that explains the sample, and contains metadata to help customers find it. For an example of this, see [android-p/AndroidPMiniDemo](https://github.com/xamarin/monodroid-samples/blob/master/android-p/AndroidPMiniDemo/README.md). The README file should begin with a YAML header (delimited by `---`) with the following keys/values:

    - **name** - must begin with `Xamarin.Android -`

    - **description** - brief description of the sample (&lt; 150 chars) that appears in the sample code browser search

    - **page_type** - must be the string `sample`.

    - **languages** - coding language/s used in the sample, such as: `csharp`, `fsharp`, `vb`, `java`

    - **products**: should be `xamarin` for every sample in this repo

    - **urlFragment**: although this can be auto-generated, please supply an all-lowercase value that represents the sample's path in this repo, except directory separators are replaced with dashes (`-`) and no other punctuation.

    Here is a working example from [_android-p/AndroidPMiniDemo_ README raw view](https://raw.githubusercontent.com/xamarin/monodroid-samples/master/android-p/AndroidPMiniDemo/README.md).

    ```yaml
    ---
    name: Xamarin.Android - Android P Mini Demo
    description: "Demonstrates new display cutout and image notification features (Android Pie)"
    page_type: sample
    languages:
    - csharp
    products:
    - xamarin
    urlFragment: android-p-androidpminidemo
    ---
    # Heading 1

    rest of README goes here, including screenshot images and requirements/instructions to get it running
    ```

    > NOTE: This must be valid YAML, so some characters in the name or description will require the entire string to be surrounded by " or ' quotes.

- **Buildable solution and .csproj file** - the project _must_ build and have the appropriate project scaffolding (solution + .csproj files).

This approach ensures that all samples integrate with the Microsoft [sample code browser](https://docs.microsoft.com/samples/browse/?term=Xamarin.Android).

A good example of this stuff is here in the [Android Pie sample](https://github.com/xamarin/monodroid-samples/tree/master/android-p/AndroidPMiniDemo)

For a cross-platform sample, please see: https://github.com/xamarin/mobile-samples/tree/master/Tasky

## GitHub Integration

We integrate tightly with Git to make sure we always provide working samples to our customers. This is achieved through a pre-commit hook that runs before your commit goes through, as well as a post-receive hook on GitHub's end that notifies our samples gallery server when changes go through.

To you, as a sample committer, this means that before you push to the repos, you should run the "install-hook.bat" or "install-hook.sh" (depending on whether you're on Windows or macOS/Linux, respectively). These will install the Git pre-commit hook. Now, whenever you try to make a Git commit, all samples in the repo will be validated. If any sample fails to validate, the commit is aborted; otherwise, your commit goes through and you can go ahead and push.

This strict approach is put in place to ensure that the samples we present to our customers are always in a good state, and to ensure that all samples integrate correctly with the sample gallery (README.md, Metadata.xml, etc). Note that the master branch of each sample repo is what we present to our customers for our stable releases, so they must *always* Just Work.

Should you wish to invoke validation of samples manually, simply run "validate.windows" or "validate.posix" (again, Windows vs macOS/Linux, respectively). These must be run from a Bash shell (i.e. a terminal on macOS/Linux or the Git Bash terminal on Windows).

If you have any questions, don't hesitate to ask!
