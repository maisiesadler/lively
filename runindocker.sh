#!/bin/bash

IFS=

result=$(ASSEMBLY_LOCATION=$ASSEMBLY_LOCATION \
  APPLICATION_CONFIG_LOCATION=$APPLICATION_CONFIG_LOCATION \
  ROOT_TYPES=$ROOT_TYPES \
  SKIP_TYPES=$SKIP_TYPES \
  ASSEMBLY_CONFIG_LOCATION=$ASSEMBLY_CONFIG_LOCATION \
  INTERFACE_RESOLVER=$INTERFACE_RESOLVER \
  OUTPUT_FORMAT=$OUTPUT_FORMAT \
  STARTUP_NAME=$STARTUP_NAME \
  dotnet /DepTree.Console.dll)

r=$?
if [ $r -ne 0 ]; then
    echo "Invalid result code"
    echo $result
    exit $r
fi

echo $result

echo "::set-output name=result::$result"
