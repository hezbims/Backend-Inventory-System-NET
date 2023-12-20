import socket
import os
import sys

def getCurrentIpAddress():
    s = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    s.connect(("100.255.255.255", 80))
    currentIpAddress = s.getsockname()[0]
    s.close()
    return currentIpAddress

currentIpAddress = getCurrentIpAddress()

port = 9999

netEnvFilePath = os.path.join("Inv_Backend_NET" , ".env")
with open(netEnvFilePath, "w") as netEnv:
    netEnv.writelines(f"APP_URL=http://{currentIpAddress}:{port}")
    
webEnvFilePath = os.path.join("Inv_Backend_NET" , "web" , "assets" , "env_file")
with open(webEnvFilePath , "w") as webEnv:
    webEnv.write(f"API_URL=http://{currentIpAddress}:{port}/api\n")
    webEnv.write(f"WEBSOCKET_URL=ws://{currentIpAddress}:{port}/ws\n")

os.chdir("Inv_Backend_NET")
os.system("dotnet run")