# Prototyping Physical Interactions with VR

Code samples for a quick and easy approach to prototyping physical interactions with mobile VR.

This system uses an Arduino microcontroller with WIFI, a node server, and a VR app built in Unity to a mobile platform (Google Cardboard or Gear VR).  All of these need to be on the same wifi network.

The node server offers a REST interface to receive messages from the arduino and connects to the Unity app using socket.io.

This project basically provides the plumbing between whatever kind of controller you build with your arduino, and the VR experience you build in Unity. For the purposes of demonstrating how it works, I've provided code assuming that you have two tilt switches attached to your arduino (put them perpendicular to each other, so a "throw" motion would trigger each in quick succession.)  And I've given you a basic Unity scene with some cardboard boxes, and a collection of balls you can throw.


**Note:** There are many different ways to connect a microcontroller wirelessly to a Unity app, including ones that don't rely on an intermediary server.  The advantages of this method are: simple, cheap, reliable and easy to debug.  Having the intermediate server lets you monitor the communication flow between your Arduino and your Unity app, and allows you to test each component individually.  It is particularly useful when doing live demos, as it gives you a continuous health check, and lets you send commands directly to Unity, for example, to reset the game state without removing the phone from the VR headset.  The one big limitation of this approach is if you want continuously-varying inputs: using REST limits the rate at which you can send data from the controller, so it is really only suitable for sending discrete values that don't change too quickly.


## Software and Hardware

**You'll need the following:**

* A local clone of this repo: `git clone https://github.com/dagoch/vrixd_examples.git`

* A wifi microcontroller that can be programmed using Arduino.  Sample code is written for the Arduino Yun and the Adafruit Huzzah ESP8266 module.  

* A switch or sensor that will be the input for your game.  Prototyping supplies like a breadboard and wires.  I also recommend using a battery to power your micrrocontroller and circuit.  If your micrcontroller board has a USB jack, you can use a USB battery to power it after you've programmed it.

  If you want to use tilt switches, I'd recommend these:  
  http://tinkersphere.com/sensors/750-tilt-switch.html  
  http://tinkersphere.com/sensors/1013-mercury-tilt-switch-6mm.html

* The Arduino IDE plus the libraries for your board.

* node.js

* Unity and the SocketIO plugin by Fabio Panettieri (from the Unity asset store)

* If you're building for Google Cardboard, you'll need the Cardboard library for Unity.  https://developers.google.com/vr/unity/

* Either XCode or the Android SDK, depending on whether you want to deploy your VR app to an iOs or Android phone.  Also, the appropriate developer signing certificates so you can build onto your mobile phone.

### Choose your microcontroller

**Yun**:  the Yun must first be configured to connect to your wifi network.  When the Yun first starts up, if it can't find a wifi network it knows how to connect to, it will create its own network, with a name like ArduinoYun-XXXXXXXXXXXX. Connect to that network, and then open the Yun config page at http://arduino.local or 192.168.240.1. After a few moments, a web page will appear asking for a password. Enter "arduino" and click the Log In button.  Then click the "Configure" button.  You'll be able to configure a name and password for your Yun, and also select a wifi network and log in to that.  After you click the Configure & Restart button, the Yun will kill its own wifi network and connect to the one you specified.  (Much more detail on setting up and using a Yun is at: https://www.arduino.cc/en/Guide/ArduinoYun).  

The sketch we will run on the Yun uses the Process class of the Bridge library.  This lets us run commands on the linux processor part of the Yun.  We will call the "curl" command to access our REST server.  More info on the Yun Bridge library is here: https://www.arduino.cc/en/Reference/YunBridgeLibrary

**ESP8266**: Off-the-shelf these come with a Lua interpreter installed.  But you can use Arduino to program them, by following these instructions: https://learn.adafruit.com/adafruit-huzzah-esp8266-breakout/using-arduino-ide.

I recommend the Adafruit HUZZAH ESP8266 breakout or the Feather HUZZAH.  The former requires an FTDI adapter for USB programming.  This board from Tinkersphere looks like it will do about the same thing (and has built-in usb): http://tinkersphere.com/electronic-components/1388-nodemcu-esp8266-iot-board.html. To use that one as an Arduino, you program it by selecting NodeMCU 1.0 in the list of Boards.

Sparkfun and others also have similar boards.

The sketch for the ESP8266 boards uses the ESP8266WiFi, ESP8266WiFiMulti, and ESP8266HTTPClient libraries, which you will install through the "Manage Libraries" tool in the Arduino IDE.

A note on the ESP8266 boards: these have fewer I/O pins than the Arduino Yun, and have only one analog input.  If you want to build a controller with multiple analog inputs, it's probably not the best bet.  Also, the analog input has a maximum voltage of less than half of Vcc for the board, so you'll need to add a resistor as a voltage divider.

### Set up your Node server

* On your computer, install node.js.  Then cd into the node-server directory of this project and type "npm install".  This will install the libraries the server code depends on.

### Set up your Unity project

* In Unity, we use the SocketIO plugin by Fabio Panettieri.  The sample project in this repo already has the plugin, but if you want to create your own project, you can download the plugin from the Asset Store.  It's free.

* For Google Cardboard, you need to install the Google Cardboard plugin.  Again, the sample project already includes this plugin.  

* If, instead, you want to use Gear VR, you'll need to remove the CarboardMain object from the scene and replace it with a regular Main Camera.  Then you'll need to enable "Virtual Reality Supported" in the Player Settings, before you build the app.  There will be a few small code changes you'd have to make as well.


*Thanks to http://www.robinwood.com/Catalog/FreeStuff/Textures/TexturePages/BallMaps.html for the ball textures.*

### Wire Your Circuit

For the purposes of this workshop, we'll work with simple switches, and use the internal pullup resistors in the micrcontroller.  So the switch will simply be connected between one of the input pins and ground.

### Program Your Arduino

The arduino-code folder contains two sketches, one for the Yun and another for the ESP8266.  In either case, you need to specify the ip address of your computer in the code, so the module knows where the node server is running.  When your computer is on the wifi network that you'll be using, go to network settings and look for the ip address of your machine.  Enter that into the code.

In the case of the ESP8266, you'll also have to enter the wifi network info.  (If you're using a Yun, you should configure it to find the wifi network according to the instructions, above.)

When you know which pins you'll connect your switches to, you'll put those in the code as well.

Finally, you'll program the board.  In the Arduino IDE you should select the board name and the serial port it's on.  The Yun can be programmed over-the-air as well.  








