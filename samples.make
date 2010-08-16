include $(samples_dir)/../build/rules.make

REFS = $(topdir)/lib/mono/2.1/Mono.Android.dll

ifdef R_CS
RES_DIR=-S res
else
RES_DIR=
endif

monodroid_exe = $(topdir)/bin/monodroid.exe
monodroid     = $(RUNTIME) $(monodroid_exe) --sdk-dir=$(ANDROID_SDK_PATH)

aresgen_exe = $(topdir)/bin/aresgen.exe
aresgen     = $(RUNTIME) $(aresgen_exe) --sdk-dir=$(ANDROID_SDK_PATH)

all: $(APK_FILE)

$(APK_FILE): $(OUTPUT_ASSEMBLY) $(monodroid_exe)
	-rm -rf `dirname $<`/build
	$(monodroid) -v=2 --builddir=build $< $(RES_DIR)

$(OUTPUT_ASSEMBLY): $(SOURCES) Makefile $(REFS) $(aresgen_exe)
	test -z "$(R_CS)" || $(aresgen) -o $(R_CS) -S res
	$(SMCS) -target:library -debug -out:$@ $(REFS:%=-r:%) $(SOURCES) $(R_CS)

clean:
	-rm -rf build $(OUTPUT_ASSEMBLY)* $(R_CS)

install: $(APK_FILE)
	adb install $<

uninstall:
	-adb uninstall $(APP_NAME)
