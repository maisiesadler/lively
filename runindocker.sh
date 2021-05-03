#!/bin/bash

echo $ASSEMBLY_LOCATION

result=$(dotnet /DepTree.Console.dll)

r=$?
if [ $r -ne 0 ]; then
    echo "Invalid result code"
    echo $result
    exit $r
fi

echo "::set-output name=result::$result"
