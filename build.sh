#!/bin/sh

for dir in `find . -type f -name *.csproj`; do
	xbuild $dir $@;

	if [ $? != 0 ]; then
		exit 1;
	fi;
done;
