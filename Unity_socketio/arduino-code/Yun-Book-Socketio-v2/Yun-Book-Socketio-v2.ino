/* 
  Code for a simple book controller that sends commands through a web server to a Unity game

  Circuit:
  1. A long flex sensor is threaded inside the spine of the book from the front to the back 
   cover.  It is connected to pins A4 and A5
  2. A simple ball tilt switch is attached to the back cover of the book; pointing down 
   about 20 degrees when the book is held upright.  It is attached to GND and pin 5;
  3. A magnetic switch is at the top of the back cover, with magnets attached at the top of
   the front cover to detect when the book is closed (not used yet).  It is attached to
   GND and pin 4;
  4. A "fast" vibration sensor is attached to the back cover and connected to GND and pin 2.
   It is caught with an external interrupt.
   
created 12 Dec 2015
by David Gochfeld

*/
#include <Process.h>

const String ipAddr = "http://192.168.0.100:4567/";  // node server's ip, on dagoch wifi network
const String moveCmd = "move";
const String stopCmd = "stop";
const String throwCmd = "throw";
const String forwardCmd = "forward";
const String helloCmd = "hello";

const int vibFastPin = 2;  // has to be on pin with hardware external interrupt
const int vibGnd = 1;  // NOTE: This doesn't work; vib sensor must be connected to real GND
          // to reliably trigger interrupt (that kinda makes sense actually, if LOW is not 
          // reliably GND)
volatile boolean shook = false;  // set in ISR
volatile long shooktime = 0;   // also set in ISR
const int clearshook = 500;    // if shook event isn't used within 1/2 sec, clear it.

const int flexGnd = A4;
const int flexSense = A5;
const int tiltPin = 7;
const int magPin = 4;
const int tiltGnd = 6;
const int magGnd = 5;


    // book closed value seems to settle around 397, but can go as high as 425
    // book all the way open roughly between 250 - 230
int closedFlexVal = 400;
int openFlexVal = 245;

const int sendInterval = 200; // minimum time between sending move speed messages to the server 
long lastTimeSent = 0;       // timestamp of the last server message


int tiltprevious =0;
int tilted = 0;
int closedprevious  = 0;
int closed = 0;


long closedtime = 0;         // the last time the magnetic switch was toggled
long tilttime = 0;         // the last time the tilt switch was toggled

int lastspeed = 0;

long debounce = 50;   // the debounce time, increase if the output flickers

 

void setup() {
  // Bridge takes about two seconds to start up
  // it can be helpful to use the on-board LED
  // as an indicator for when it has initialized
  pinMode(13, OUTPUT);
  digitalWrite(13, LOW);
  Bridge.begin();
  digitalWrite(13, HIGH);

  
  // Initialize Serial
  Serial.begin(9600);

  // Set up inputs
  pinMode(flexGnd, OUTPUT);
  digitalWrite(flexGnd, LOW);
  pinMode(vibGnd, OUTPUT);
  digitalWrite(vibGnd, LOW);
  pinMode(tiltGnd, OUTPUT);
  digitalWrite(tiltGnd, LOW);
  pinMode(magGnd, OUTPUT);
  digitalWrite(magGnd, LOW);
  

  pinMode(flexSense, INPUT_PULLUP);

  pinMode(tiltPin, INPUT_PULLUP);
  pinMode(magPin, INPUT_PULLUP);
  
  pinMode(vibFastPin, INPUT_PULLUP);
  // Configure motion pin for change detect & interrupt
  attachInterrupt(digitalPinToInterrupt(vibFastPin), throwBookA, FALLING);

    // run various example processes
  runCurl(helloCmd);
  runCpuInfo();

  calibrateClosed();
}

// flex sensor values shift, so under certain conditions recalibrate the value that
// corresponds to "closed"
void calibrateClosed() {
    int closedreading = !digitalRead(magPin);  // magPin is Low when book is closed
  if (closedreading == 1) {
        int flexSensor = analogRead(flexSense);
        closedFlexVal = flexSensor;
        Serial.print("Calibrate closed = ");
        Serial.println(closedFlexVal);
  }
}

int serialOutCounter = 0;
void loop() {

  long now = millis();

if (shook && (now -shooktime > clearshook)) shook = false;

  if (now - lastTimeSent > sendInterval) {
    lastTimeSent = now;
      // read the flex sensor
    int flexSensor = analogRead(flexSense);


    int speed = map(flexSensor, closedFlexVal,openFlexVal,0,5);
    if ((++serialOutCounter %4)==0) {  // only print to serial every nth loop
        Serial.print("Flex = ");
    Serial.print(flexSensor);
    Serial.print(" : speed = ");
    Serial.println(speed);
    }

    if ((speed < 0) ||
         (speed > 0 && closed)) { // flex sensor bounds shift a bit
      speed=0;  
      calibrateClosed();
    }
    if (speed != lastspeed) {
      String cmd = moveCmd;
      cmd += "/";
      cmd += speed;
      runCurl(cmd);
      lastspeed = speed;
    }
  }

  // Debounce tilt switch
  int tiltswitchstate;
 
  int tiltreading = digitalRead(tiltPin);  // tilt is low when book is upright; 1 when tilted horiz
 
  // If the switch changed, due to bounce or pressing...
  if (tiltreading != tiltprevious) {
    // reset the debouncing timer
    tilttime = millis();
  } 
 
  if ((millis() - tilttime) > debounce) {
     // whatever the switch is at, its been there for a long time
     // so lets settle on it!
     tiltswitchstate = tiltreading;
    if (tiltswitchstate != tilted) {
       
      Serial.print("tilt changed: ");
      Serial.print(tiltswitchstate);
      Serial.print(" and shook= ");
      Serial.println(shook);
      
      if ((tilted == 0) && shook) {
        Serial.println("Throw book!");
        runCurl(throwCmd);
      }
      tilted = tiltswitchstate;

      shook=false;
    }

  } 
  // Save the last reading so we keep a running tally
  tiltprevious = tiltreading;

// Debounce magnetic switch
int closedswitchstate;
 
  int closedreading = !digitalRead(magPin);  // magPin is Low when book is closed
 
  // If the switch changed, due to bounce or pressing...
  if (closedreading != closedprevious) {
    // reset the debouncing timer
    closedtime = millis();
  } 
 
  if ((millis() - closedtime) > debounce) {
     // whatever the switch is at, its been there for a long time
     // so lets settle on it!
     closedswitchstate = closedreading;

    if (closedswitchstate != closed) {
      closed = closedswitchstate;
            Serial.print("Closed state changed: ");
      Serial.println(closed);
      if (closed) {
        // send stop command
            String cmd = moveCmd;
            cmd += "/0";
            runCurl(cmd);        
      }
    }
  }

 
  // Save the last reading so we keep a running tally
  closedprevious = closedreading;
  
}


// vibration sensor ISR
void throwBookA() {
      shook = true;    
      shooktime = millis();
}


void runCurl(String cmd) {
  // Launch "curl" command and get Arduino ascii art logo from the network
  // curl is command line program for transferring data using different internet protocols
  Process p;    // Create a process and call it "p"
  p.begin("curl");  // Process that launch the "curl" command
  String url = ipAddr+cmd;
  
  p.addParameter(url); // Add the URL parameter to "curl"
  p.run();    // Run the process and wait for its termination

  // A process output can be read with the stream methods
  while (p.available() > 0) {
    char c = p.read();
    Serial.print(c);
  }
  Serial.println();
  // Ensure the last bit of data is sent.
  Serial.flush();
}

void runCpuInfo() {
  // Launch "cat /proc/cpuinfo" command (shows info on Atheros CPU)
  // cat is a command line utility that shows the content of a file
  Process p;    // Create a process and call it "p"
  p.begin("cat"); // Process that launch the "cat" command
  p.addParameter("/proc/cpuinfo"); // Add the cpuifo file path as parameter to cut
  p.run();    // Run the process and wait for its termination

  // Print command output on the Serial.
  // A process output can be read with the stream methods
  while (p.available() > 0) {
    char c = p.read();
    Serial.print(c);
  }
  // Ensure the last bit of data is sent.
  Serial.flush();
}



