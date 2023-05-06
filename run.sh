#!/bin/bash

# Define the command to run in each terminal window
command_to_run="dotnet run ./bin/Debug/net7.0/Auction_System"

# Define the number of instances to run
num_instances=4

# linux
# Loop through the number of instances and open a new terminal window for each
#gnome-terminal --tab --title="Instance $i" --command="$command_to_run BootstrapNode"
#for i in $(seq 1 $num_instances)
#do
#  gnome-terminal --tab --title="Instance $i" --command="$command_to_run Client"
#done

# wsl
cmd.exe /c start "Instance 0" wsl $command_to_run BootstrapNode ./terminalInstantions/0/
sleep 3
for i in $(seq 1 $num_instances)
do
 cmd.exe /c start "Instance $i" wsl $command_to_run Client ./terminalInstantions/$i/  
done

#cmd.exe /c start "Instance 5" wsl $command_to_run Client ./terminalInstantions/$i/ miner

# vypis do souboru
# $command_to_run BootstrapNode > ./logs/instance0.txt &
# sleep 2
# for i in $(seq 1 $num_instances)
# do
#  $command_to_run Client  > ./logs/instance$i.txt &
# done

# echo "Press any key to continue"
# read
# pkill -f "sh ./run.sh"

#cmd.exe /c start "Instance $i" wsl $command_to_run Client test1