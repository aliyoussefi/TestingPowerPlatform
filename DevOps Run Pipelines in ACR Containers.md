# DevOps: Run Pipelines in ACR Containers

## Summary

This will detail my efforts to build a Dockerfile locally, run with Docker locally then push to ACR with a Build Pipeline. From there we will run another pipeline using this container image as the build agent. Finally we will look how to pass in the name of the image or tag as a variable to the yml based pipeline.

## Pre Reqs

Docker local

Azure Container Registry

Azure DevOps

Visual Studio Code or IDE

PowerShell

## Build Dockerfile

### Locate an image to pull from

Start by going to hub.docker.com and look for windows server core or nano.

#### Windows Server Core

 https://hub.docker.com/_/microsoft-windows-servercore 

Tags:

- `ltsc2019` ([LTSC](https://docs.microsoft.com/en-us/windows-server/get-started-19/servicing-channels-19#long-term-servicing-channel-ltsc))
  docker pull mcr.microsoft.com/windows/servercore:ltsc2019
- `1903` ([SAC](https://docs.microsoft.com/en-us/windows-server/get-started-19/servicing-channels-19#semi-annual-channel))
  docker pull mcr.microsoft.com/windows/servercore:1903

#### Windows Server Core Insider

 https://hub.docker.com/_/microsoft-windows-servercore-insider 

Tags:

 `10.0.19008.1` (20H1)
docker pull mcr.microsoft.com/windows/servercore/insider:10.0.19008.1 

#### Windows Server Nano

 https://hub.docker.com/_/microsoft-windows-nanoserver 

Tags:

 `1903` ([SAC](https://docs.microsoft.com/en-us/windows-server/get-started-19/servicing-channels-19#semi-annual-channel))
docker pull mcr.microsoft.com/windows/nanoserver:1903 

#### Windows Nano Server Insider

 https://hub.docker.com/_/microsoft-windows-nanoserver-insider/ 

### Create the Dockerfile

Begin by creating a new text file using your IDE of choice. Then for the first line pull in the image you want to start with:

Windows Server Core LTSC 2019

```
# Indicates that the windowsservercore image will be used as the base image.
FROM mcr.microsoft.com/windows/servercore:ltsc2019
```

Next add some metadata to help describe the container:

```
# Metadata indicating an image maintainer.
LABEL maintainer="jshelton@contoso.com"
```



## Build the Container locally

```
docker build
```



## Create the Container Locally

Once the Dockerfile is complete and the image has been built we need to run the container and send some commands to it locally.

![image-20191110073703736](C:\Users\alyousse\AppData\Roaming\Typora\typora-user-images\image-20191110073703736.png)

```
docker create
```

