#!/bin/bash

result="$(ASSEMBLY_LOCATION=$ASSEMBLY_LOCATION \
  ASSEMBLY_PATTERN_MATCH=$ASSEMBLY_PATTERN_MATCH \
  APPLICATION_CONFIG_LOCATION=$APPLICATION_CONFIG_LOCATION \
  ROOT_TYPES=$ROOT_TYPES \
  SKIP_TYPES=$SKIP_TYPES \
  ASSEMBLY_CONFIG_LOCATION=$ASSEMBLY_CONFIG_LOCATION \
  INTERFACE_RESOLVER=$INTERFACE_RESOLVER \
  OUTPUT_FORMAT=$OUTPUT_FORMAT \
  STARTUP_NAME=$STARTUP_NAME \
  dotnet /Lively.Console.dll)"

r=$?
if [ $r -ne 0 ]; then
    echo "Invalid result code"
    echo $result
    exit $r
fi

result="${result//'%'/'%25'}"
result="${result//$'\n'/'%0A'}"
result="${result//$'\r'/'%0D'}"

echo "printing result"
echo $result

echo "::set-output name=result::$result"
