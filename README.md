# P2P auction server
This project is P2P kademlia based auction server where auction ledger is stored in blockchain.

## Instalation
Project is made in .NET technology using C# programming language. 

To run is necessary to install .NET7

sudo apt install dotnet-runtime-7.0

## Run
To run project shell script run.sh can be used. Script run n instantions of application. It runs one bootstrap node / auction server and multiple clients (clients can be miners).

Script kill all instantions after pressing a key.

To see output of each instantion, there are log files in folder terminalInstantions.

Command line arguments [instantion type - BootstrapNode/Client] [folder path for outputs] [Miner](optional)
