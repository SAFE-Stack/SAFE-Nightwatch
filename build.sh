#!/bin/bash

if [ -z "$ANDROID_HOME" ]; then
  echo "ANDROID_HOME is not currently set, using a default setting of /opt/android-sdk-linux/"
  export ANDROID_HOME=/opt/android-sdk-linux/
fi

if test "$OS" = "Windows_NT"; then
  MONO=""
else
  # Mono fix for https://github.com/fsharp/FAKE/issues/805
  export MONO_MANAGED_WATCHER=false
  MONO="mono"
fi

if [ -e "paket.lock" ]; then
	$MONO .paket/paket.exe restore
else
	$MONO .paket/paket.exe install
fi
exit_code=$?
if [ $exit_code -ne 0 ]; then
	exit $exit_code
fi
$MONO packages/build/FAKE/tools/FAKE.exe $@ --fsiargs build.fsx
