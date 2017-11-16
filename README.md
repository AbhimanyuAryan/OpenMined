OpenMined Unity Application
=============================================

## Fast setup

Open this project in Unity, hit play, then open the jupyter notebook in "notebooks". If everything if working fine, you are done. If not, go to "Detailed Setup".

## Detailed Setup

### Windows

On Unity:

1. Open this project in Unity

2. Check "Main Camera" object has "SyftServer.cs" component attached to it

Go to "Assets/OpenMined/Network/Servers" drag "SyftServer.cs" to "Main Camera" object

3. Add a "Compute Shader" to the "Shader" variable of "SyftServer.cs" script

Go to "Assets/OpenMined/Syft/Math/Shaers" drag "NewComputeShader" to "SyftServer (Script)" component recently attached to "Main Camera"

4. Hit "Play" on the Unity Editor

On Jupyter Notebook:

5. Open "basic-python-network-gpu.ipynb" 

6. Run the Jupyter Notebook

#### Extra considerations

*Check if the Server is running*
It should run on port 5555 and this can be checked by running the following command on CMD with administrator permissions.
```
netstat -a -b | findstr :5555
```
If just the Server is running, the output should be:
```
TCP    0.0.0.0:5555           YOUR_PC_NAME:0      LISTENING
```
If both Server and Jupyter Notebook are running and communicating, the output should be:
```
TCP    0.0.0.0:5555           YOUR_PC_NAME:0      LISTENING
TCP    127.0.0.1:5555         YOUR_PC_NAME:63956  ESTABLISHED
TCP    127.0.0.1:63956        YOUR_PC_NAME:5555   ESTABLISHED
```

*Jupyter Notebook only works if Unity has focus*
By default, the "Run in background" options is disabled. So if the Unity Editor loses focus then the Jupyter Notebook won't work.
Go to Edit -> Project Settings -> Player. The inspector pane will now change to show the player settings. Look for the option that says "Run In Background" and check it [1]

### References

[1] [stop unity pausing when it loses focus](https://answers.unity.com/questions/42509/stop-unity-pausing-when-it-loses-focus.html)

