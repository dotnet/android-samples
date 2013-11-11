Action Bar PullToRefresh
========================

A binding library for https://github.com/chrisbanes/ActionBar-PullToRefresh.

To get javadoc so that the binding dll can get method parameter names:

	git clone above.
	cd ActionBar-PullToRefresh
	mvn clean verify javadoc:aggregate-jar

If you need to update library project zip:

	cd library
	android update project -p .
	ant debug
	zip -r ActionBar-PullToRefresh.zip bin/classes.jar bin/AndroidManifest.xml res
	mv ActionBar-PullToRefresh.zip ActionBarPullToRefresh/Jars

