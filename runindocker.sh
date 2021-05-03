#!/bin/bash

result=$(dotnet /DepTree.Console.dll)
echo "::set-output name=result::$result"
