#!/bin/sh

mono xpkg.exe create XamarinActionBarSherlock-4.4.0.0.xam \
	--name="ActionBarSherlock for Xamarin" \
	--publisher="Xamarin, Inc." \
	--website="http://www.xamarin.com" \
	--summary="C# binding for ActionBarSherlock." \
	--license="./LICENSE-ABSoriginal.txt" \
	--library=android:../ActionBarSherlock/bin/Debug/ActionBarSherlock.dll \
	--details=Details.md \
	--getting-started=GettingStarted.md \
	--icon=XamarinActionBarSherlock_128x128.png \
	--icon=XamarinActionBarSherlock_512x512.png \
   	--sample="Demo. Port of ActionBarSherlock demo - feature showcase.:../ActionBarSherlock.sln" \
	--screenshot="Select Target Framework":sshot_TargetFramework.png \
	--screenshot="Configure Minimum Target Framework":sshot_AndroidManifest.png \
	--screenshot="Action bar space allocated":sshot_SimpleTheme.png \
	--screenshot="Feature showcase (from \"FeatureToggles\" sample)":sshot_FeatureToggles.png \
	--screenshot="List Navigation":sshot_ListNavigation.png \
	--screenshot="Tab Navigation":sshot_TabNavigation.png \
	--screenshot="Collapsible Action Items - default state":sshot_CollapsibleActionItems1.png \
	--screenshot="Collapsible Action Items - collapsed state":sshot_CollapsibleActionItems2.png
	

