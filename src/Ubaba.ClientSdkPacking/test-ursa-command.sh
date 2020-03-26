#!/bin/bash

# removing last generated directory
rm -d -f -r ./gen-package

dotnet run -- /home/mortzi/src/ursa/src/Ursa.ClientSdkPacking/ursa.sample.yaml \
--api-key 0bc6fdc4-b3d1-3377-b3eb-728b3ee5d2ea
