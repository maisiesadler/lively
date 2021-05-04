#!/bin/bash

result=$(dotnet /DepTree.Console.dll \
  -a $ASSEMBLY_LOCATION \
  --config $APPLICATION_CONFIG_LOCATION \
  -t $ROOT_TYPE \
  -s $SKIP_TYPE \
  -n $ASSEMBLY_CONFIG_LOCATION \
  -i $INTERFACE_RESOLVER)

r=$?
if [ $r -ne 0 ]; then
    echo "Invalid result code"
    echo $result
    exit $r
fi

echo "::set-output name=result::$result"
