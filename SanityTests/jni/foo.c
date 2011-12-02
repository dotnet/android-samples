#include <android/log.h>
#include "foo.h"

void
GoMonodroidGo (void)
{
	__android_log_print (ANDROID_LOG_INFO, "*jonp*", "Go MonoDroid, Go!");
}

void
foo_init (void)
{
	__android_log_print (ANDROID_LOG_INFO, "*jonp*", "foo_init");
}

