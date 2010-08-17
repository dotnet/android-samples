srcdir_abs=$(shell pwd)
topdir=$(srcdir_abs)/../..
TARGETS_DIR=$(srcdir_abs)/../../tools/msbuild/build
XBUILD=MSBuildExtensionsPath="$(TARGETS_DIR)" xbuild /p:AndroidSdkDirectory="$(ANDROID_SDK_PATH)" /p:MonoDroidInstallDirectory="$(topdir)"

all:
	$(XBUILD) $(PROJ) /t:Build $(EXTRA)

clean:
	$(XBUILD) $(PROJ) /t:Clean $(EXTRA)

install:
	$(XBUILD) $(PROJ) /t:Install $(EXTRA)

uninstall:
	$(XBUILD) $(PROJ) /t:Uninstall $(EXTRA)
