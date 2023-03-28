#!/bin/bash

# Define the command to run in each terminal window
command_to_run="dotnet run ./bin/Debug/net7.0/Auction_System"

# Define the number of instances to run
num_instances=10

# linux
# Loop through the number of instances and open a new terminal window for each
#gnome-terminal --tab --title="Instance $i" --command="$command_to_run BootstrapNode"
#for i in $(seq 1 $num_instances)
#do
#  gnome-terminal --tab --title="Instance $i" --command="$command_to_run Client"
#done

# wsl
cmd.exe /c start "Instance 0" wsl $command_to_run BootstrapNode
sleep 2
for i in $(seq 1 $num_instances)
do
 cmd.exe /c start "Instance $i" wsl $command_to_run Client
done