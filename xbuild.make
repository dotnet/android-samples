srcdir_abs=$(shell pwd)
topdir=$(srcdir_abs)/../../..
TARGETS_DIR=$(srcdir_abs)/../../../tools/msbuild/build
XBUILD=MSBuildExtensionsPath="$(TARGETS_DIR)" xbuild /p:AndroidSdkDirectory="$(ANDROID_SDK_PATH)" /p:MonoDroidInstallDirectory="$(topdir)"

all:
	$(XBUILD) $(PROJ) /t:Build $(EXTRA)

clean:
	$(XBUILD) $(PROJ) /t:Clean $(EXTRA)

install:
	$(XBUILD) $(PROJ) /t:Install $(if $(ADB_TARGET),"/p:AdbTarget=$(ADB_TARGET)",) $(EXTRA) $(INSTALL_EXTRA)

uninstall:
	$(XBUILD) $(PROJ) /t:Uninstall $(if $(ADB_TARGET),"/p:AdbTarget=$(ADB_TARGET)",) $(EXTRA) $(UNINSTALL_EXTRA)
