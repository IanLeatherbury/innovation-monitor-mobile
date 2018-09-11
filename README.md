# InnovationMonitor aka BeerMonitor

This project combines IoT, Serverless, and Mobile into a nice packaged solution that you can leverage to monitor temperature, pressure, images, and video of an industrial process – in this case, beer. 

Consists of the following:

- [A Xamarin.Forms Mobile App](https://github.com/IanLeatherbury/innovation-monitor-mobile)
- [A Python Raspberry Pi App](https://github.com/IanLeatherbury/innovation-monitor-razzberry-pi)
- [An Azure Function App](https://github.com/IanLeatherbury/innovation-monitor-functions)

The accompanying projects can be found at the links above. This guide will break each repo down individually.

Also, take a look at the high-level architecture diagram:

<img src="https://github.com/IanLeatherbury/innovation-monitor-mobile/raw/master/imgs/arch-diagram.png" width="600">

Let's get started!

# The IoT Project

This is broken down into 4 parts:
- Getting set up and configuring the BME280
- Setting up the camera
- Setting up the additional temperature sensor
- Building the relay and light 

## IoT Part 1: Setting up the BME280

This is project uses the code in [this repo](https://github.com/Azure-Samples/iot-hub-python-raspberrypi-client-app) to get set up and connect the BME280 (temp, pressure, humidity) sensor. I highly recommend you visit that repo for the latest maintained version of the sample. I've included the instructions at the time of writing for convenience.

> This repo contains the source code to help you get started with Azure IoT using the Microsoft IoT Pack for Raspberry Pi 3 Starter Kit. You will find the [full tutorial on Docs.microsoft.com](https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-raspberry-pi-kit-c-get-started).

This repo contains a python application that runs on Raspberry Pi 3 with a BME280 temperature&humidity sensor, and then sends these data to your IoT hub. At the same time, this application receives Cloud-to-Device messages from your IoT hub, and takes actions according to the C2D command.

### Step 1: Set up your Pi
#### Enable SSH on your Pi
Follow [this page](https://www.raspberrypi.org/documentation/remote-access/ssh/) to enable SSH on your Pi.

#### Enable I2C on your Pi
Follow [this page](https://www.raspberrypi.org/documentation/configuration/raspi-config.md) to enable I2C on your Pi

### Step 2: Connect your sensor with your Pi
#### Connect with a physical BME280 sensor and LED
You can follow the image to connect your BME280 and an LED with your Raspberry Pi 3.

<img src="https://docs.microsoft.com/en-us/azure/iot-hub/media/iot-hub-raspberry-pi-kit-node-get-started/3_raspberry-pi-sensor-connection.png" width="600">

#### DON'T HAVE A PHYSICAL BME280?
You can use the application to simulate temperature&humidity data and send to your IoT hub.
1. Open the `config.py` file.
2. Change the `SIMULATED_DATA` value from `False` to `True`.

### Step 3: Download and setup referenced modules

1. Clone the client application to local:

   ```
   sudo apt-get install git-core

   git clone https://github.com/Azure-Samples/iot-hub-python-raspberrypi-client-app.git

   ```

2. Because the Azure IoT SDKs for Python are wrappers on top of the [SDKs for C][azure-iot-sdk-c], you will need to compile the C libraries if you want or need to generate the Python libraries from source code.

   ```
   cd ./iot-hub-python-raspberrypi-client-app
   sudo chmod u+x setup.sh
   sudo ./setup.sh
   ```
   In the above script, we run **./setup.sh** without parameter, so the shell will automatically detect and use the version of python installed (Search sequence 2.7->3.4->3.5). Alternatively, you can use a parameter to specify the python version which you want to use like this: **sudo ./setup.sh [--python-version|-p] [2.7|3.4|3.5]**


    Known build issues:

    1.) On building the Python client library (`iothub_client.so`) on Linux devices that have less than **1GB** RAM, you may see build getting **stuck** at **98%** while building `iothub_client_python.cpp` as shown below

    ``[ 98%] Building CXX object python/src/CMakeFiles/iothub_client_python.dir/iothub_client_python.cpp.o``

    If you run into this issue, check the **memory consumption** of the device using `free -m command` in another terminal window during that time. If you are running out of memory while compiling iothub_client_python.cpp file, you may have to temporarily increase the **swap space** to get more available memory to successfully build the Python client side device SDK library.

### Step 4: Run your client application

Run the client application, and you need to provide your Azure IoT hub device connection string, note your connection string should be quoted in the command:
   ```
   python app.py '<your Azure IoT hub device connection string>'
   ```
If you use the python 3, then you can use the command below:
   ```
   python3 app.py '<your Azure IoT hub device connection string>'
   ```

If the application works normally, then you will see the screen like this:

![](https://github.com/IanLeatherbury/innovation-monitor-mobile/raw/master/imgs/success.png)

## IoT Part 2: Set up the camera and upload to Azure

- Follow [this tutorial](https://www.raspberrypi.org/documentation/configuration/camera.md) on how to set up the camera and
- [this tutorial](https://edi.wang/post/2016/8/8/raspbian-picamera-upload-azure) to upload it into Azure Blob storage

## IoT Part 3: Set up the other temperature sensor
- Follow [this tutorial](https://cdn-learn.adafruit.com/downloads/pdf/adafruits-raspberry-pi-lesson-11-ds18b20-temperature-sensing.pdf). Note that I did not use the Pi Cobbler jumper cable.

## IoT Part 4: Build the relay and wire it up
- Follow [this tutorial](https://learn.sparkfun.com/tutorials/beefcake-relay-control-hookup-guide) and be sure to do this safely. 

# The Functions Project
Up next, we'll get some Azure Functions running to your IoT Hub.

The project repo is located [here](https://github.com/IanLeatherbury/innovation-monitor-functions).

Each of these functions is responsible for a different aspect of functionality within the mobile app.
- GetInternalTemp
- GetTempHumidity
- TakeInternalTemp
- TakePicture
- TakeTempHumidity
- Take Video
- TempHumidityEventTrigger

# The Mobile Project
Finally, we get to see all of our hard work in action.

<img src="https://github.com/IanLeatherbury/innovation-monitor-mobile/raw/master/imgs/mobile-app-homepage.png" width="600">
