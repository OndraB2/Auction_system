#!/bin/bash

# Define the command to run in each terminal window
command_to_run="dotnet run ./bin/Debug/net7.0/Auction_System"

# Define the number of instances to run
num_instances=4

# Array to store PIDs
pids=()

$command_to_run BootstrapNode ./terminalInstantions/0/ & pids+=($!)
sleep 3
for i in $(seq 1 $num_instances)
do
    $command_to_run Client ./terminalInstantions/$i/ & pids+=($!)
done

$command_to_run Client ./terminalInstantions/5/ miner & pids+=($!)

#sleep 20
read -p "Press any key to stop"

for pid in "${pids[@]}"
do
    echo "kill $pid"
    kill "$pid"
done

# linux
# Loop through the number of instances and open a new terminal window for each
#gnome-terminal --tab --title="Instance $i" --command="$command_to_run BootstrapNode"
#for i in $(seq 1 $num_instances)
#do
#  gnome-terminal --tab --title="Instance $i" --command="$command_to_run Client"
#done

# wsl
<<<<<<< HEAD
#cmd.exe /c start "Instance 0" wsl $command_to_run BootstrapNode ./terminalInstantions/0/
sleep 3
for i in $(seq 1 $num_instances)
do
 cmd.exe /c start "Instance $i" wsl $command_to_run Client ./terminalInstantions/$i/  
done
=======
# cmd.exe /c start "Instance 0" wsl $command_to_run BootstrapNode ./terminalInstantions/0/
# sleep 3
# for i in $(seq 1 $num_instances)
# do
#  cmd.exe /c start "Instance $i" wsl $command_to_run Client ./terminalInstantions/$i/  
# done

# cmd.exe /c start "Instance 5" wsl $command_to_run Client ./terminalInstantions/5/ miner

#cmd.exe /c start "Instance 6" wsl $command_to_run Client ./terminalInstantions/6/ miner
>>>>>>> Adam

cmd.exe /c start "Instance 5" wsl $command_to_run Client ./terminalInstantions/5/ miner

#cmd.exe /c start "Instance 6" wsl $command_to_run Client ./terminalInstantions/6/ miner

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