import socket
from pathlib import Path
import os, random
from _thread import *


# YouOnlyLabelOnce server
# by tom o'donnell (tkodonne@purdue.edu)
#
# allows for easy yolov5 image labeling collaboration between teams
# 
# starts a tcp server on port 12345 allowing the client
# application to request images for annotation, and send
# a resulting annotation file to the server.

ServerSideSocket = socket.socket()
host = '127.0.0.1'
port = 12345
existing_labels = []

def main():
    try:
        ServerSideSocket.bind((host, port))
    except socket.error as e:
        print(str(e))
    print('Socket is listening..')
    ServerSideSocket.listen(5)

    while True:
        Client, address = ServerSideSocket.accept()
        print('Connected to: ' + address[0] + ':' + str(address[1]))
        start_new_thread(multi_threaded_client, (Client, ))
    ServerSideSocket.close()

def multi_threaded_client(connection):
    while True:
        data = connection.recv(2048)
        response = data.decode('utf-8')
        if not data:
            break

        # if receive an image request
        if response == "r":
            print("client requests image")

            rand_img = random.choice(os.listdir("unannotated"))
            with open("unannotated/" + rand_img, "rb") as fd:
                buf = fd.read(1048576) # 10mb
                connection.send(buf)

        else:
            print("resp: " + response)

            # create annotation file
            with open("annotations/" + Path(rand_img).stem + ".txt" , 'w') as file:
                file.write(response)
            
            # move image to annotated folder
            Path("unannotated/" + rand_img).rename("annotated/" + rand_img)


    connection.close()

if __name__ == "__main__":
    main()
