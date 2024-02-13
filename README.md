# OpenFileShare

## Usage: <br>
### Syntax:<br>
To send a file:<br>
```
OpenFileShare.exe push PORT PASSWORD
```
To receive a file:<br>
```
OpenFileShare.exe pull IP PORT PASSWORD
```
### Sending the file:<br> 
If the connection is successful, you should see "Passwords Match!" on both client and server.<br>
After that, on the client, enter the "name.extension" of the file.
Do the same for the server, but for the file you wanna send. (Note that you can use a Full Path for the file, but if the file is in the working directory, then you can just use "name.extension" too.)
<br></br>

## Example log if everything goes correctly: <br>
Server:<br>
```
OpenFileShare.exe push 25565 safePassword010$#
Path to send: testFile.txt
File found, sending...
File sent successfully!
Closing Connection...
```
Client:<br>
```
OpenFileShare.exe pull 192.168.1.2 25565 safePassword010$#
(Full Path optional) File name + extension to save the file: testFileClient.txt
Waiting for server to send the file...
File received successfully!
Path to created file: C:\Users\User\Desktop\OpenFileShare\testFileClient.txt
Closing Connection...
```

## Intent of this Project <br>
I had to send lots of files to my friends, but each time uploading them to a 3rd party website was both time consuming and untrustworthy.<br>
So I decided to make this simple program to make that process a lot easier.<br>
I am thinking about making a GUI version too using .net Forms. <br>
<br></br>
Since I made this around midnight, I didn't have time to test it other than locally with a virtual machine.<br>
If there are any issues, let me know and I will try to fix them.
