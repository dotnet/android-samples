LOCAL_PATH := $(call my-dir)

include $(CLEAR_VARS)

LOCAL_LDLIBS    := -llog
LOCAL_MODULE    := foo
LOCAL_SRC_FILES := foo.c

include $(BUILD_SHARED_LIBRARY)

include $(CLEAR_VARS)

LOCAL_SHARED_LIBRARIES = foo
LOCAL_LDLIBS    := -llog
LOCAL_MODULE    := bar
LOCAL_SRC_FILES := bar.c

include $(BUILD_SHARED_LIBRARY)

include $(CLEAR_VARS)

LOCAL_MODULE    := dltest
LOCAL_SRC_FILES := dltest.c

include $(BUILD_EXECUTABLE)

