#!/bin/bash

result=$(ASSEMBLY_LOCATION=$ASSEMBLY_LOCATION \
  APPLICATION_CONFIG_LOCATION=$APPLICATION_CONFIG_LOCATION \
  ROOT_TYPE=$ROOT_TYPE \
  SKIP_TYPE=$SKIP_TYPE \
  ASSEMBLY_CONFIG_LOCATION=$ASSEMBLY_CONFIG_LOCATION \
  INTERFACE_RESOLVER=$INTERFACE_RESOLVER \
  dotnet /DepTree.Console.dll)

r=$?
if [ $r -ne 0 ]; then
    echo "Invalid result code"
    echo $result
    exit $r
fi

echo "::set-output name=result::$result"
