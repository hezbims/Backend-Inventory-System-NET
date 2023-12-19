import socket
import os

def getCurrentIpAddress():
    s = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    s.connect(("100.255.255.255", 80))
    currentIpAddress = s.getsockname()[0]
    s.close()
    return currentIpAddress

currentIpAddress = getCurrentIpAddress()

netEnvFilePath = os.path.join("Inv_Backend_NET" , ".env")
with open(netEnvFilePath, "w") as netEnv:
    netEnv.writelines(f"APP_URL=http://{currentIpAddress}:9999")
    
webEnvFilePath = os.path.join("Inv_Backend_NET" , "web" , "assets" , "env_file")
print(webEnvFilePath)
with open(webEnvFilePath , "w") as webEnv:
    webEnv.write(f"API_URL=http://{currentIpAddress}:5154/api\n")
    webEnv.write(f"WEBSOCKET_URL=ws://{currentIpAddress}:5154/ws\n")
