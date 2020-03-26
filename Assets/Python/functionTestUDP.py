import socket
import sys

#README
#To run this file you just need to first determine what host and port the C# script will be run on 
#After that ensure you have registered the function you would like to run to the C# script and remember the command 
#Enter the command to get the behavior 
#The program will run until a q command is received (thus q is reserved and can't be used)

# Create a UDP socket
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

#Note: If you change one of these be sure you also update them on the DebugManager object - script function manager
ip = "localhost" #IP of the wireless interface to send messages too
port = 10101   #Port to send messages too


server_address = (ip, port)

try:
    while(1):
        message = input("Please Enter your command...")

        if(message == "Q" or message == "q"):
            break
        else:
            # Send data
            print("Sending data")
            sent = sock.sendto(message.encode(), server_address)
            print("Data sent")
finally:
    print ( 'closing socket')
    sock.close()