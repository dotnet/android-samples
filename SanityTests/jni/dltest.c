/* 
 * dlopen test program for libraries 
 *
 * Compile as: gcc -o dltest dltest.c -ldl
 */
#include <stdio.h>
#include <dlfcn.h>

int
main (int argc, char **argv)
{
	int i;
	for (i = 1; i < argc; ++i) {
		void *h;
		h = dlopen (argv [i], RTLD_NOW);
		if (h == NULL)
			printf ("error loading library `%s': %s\n", argv [i], dlerror ());
		if (h != NULL)
			dlclose (h);
	}
	return 0;
}

