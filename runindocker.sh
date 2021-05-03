#!/bin/bash

result=$(dotnet /DepTree.Console.dll $APPLICATION_CONFIG_LOCATION $ASSEMBLY_LOCATION)

r=$?
if [ $r -ne 0 ]; then
    echo "Invalid result code"
    echo $result
    exit $r
fi

echo "::set-output name=result::$result"
