/* 
  Demonstration of how to use an ESP8266 as a wireless controller for a Unity game

  Using Adafruit Huzzah esp8266 breakout.  It connects over wifi to a REST service running on 
  a computer, which then sends commands to a Unity app on a phone via websockets.

 From Adafruit data sheet:
 This breakout has 9 GPIO: 
  #0, #2, #4, #5, #12, #13, #14, #15, #16 all GPIO are 3.3V logic level in and out
GPIO #0, does not have an internal pullup, is connected to  mini tactile switch and red LED. This pin is used by the ESP8266 to determine when to boot into the bootloader. If the pin is held low during power-up it will start bootloading! That said, you can always use it as an output, and blink the red LED.
GPIO #2, is also used to detect boot-mode. It also is connected to the blue LED that is near the WiFi antenna. It has a pullup resistor connected to it, and you can use it as any output (like #0) and blink the blue LED.
GPIO #15, is also used to detect boot-mode. It has a pulldown resistor connected to it, make sure this pin isn't pulled high on startup. You can always just use it as an output
GPIO #16 can be used to wake up out of deep-sleep mode, you'll need to connect it to the RESET pin
  
  Circuit:
  For this demo, we'll just use two tilt sensors.  When they are activated in quick succession, we'll
  send a "throw" command.

  Tilt1 is connected between pin13 and GND
  Tilt2 is connected between pin5 and GND
  
created 12 Jun 2016
by David Gochfeld

*/
#include <ESP8266WiFi.h>
#include <ESP8266WiFiMulti.h>

#include <ESP8266HTTPClient.h>

#define USE_SERIAL Serial

// Wifi network 
const char* ssid     = "dagoch";
const char* password = "mydevwifi";

// this is ip address and port of the node server running on my laptop
const char* host = "192.168.0.100"; // using my router  
const int httpPort = 5678;

const String moveCmd = "move";
const String stopCmd = "stop";
const String throwCmd = "throw";
const String forwardCmd = "forward";
const String helloCmd = "hello";

const int tilt1 = 13;
const int tilt2 = 5;

const int redLED = 0; // on Adafruit Huzzah esp8266

const int blueLED = 2; // on Adafruit Huzzah esp8266

long tilt1OnTime;

// This is how we communicate with the server
void runCurl(String cmd) {
   Serial.print("connecting to ");
  Serial.print(host);
    Serial.print(":");
    Serial.print(httpPort);
    Serial.print(" Get ");
    Serial.println(cmd);
    String request = "/"+cmd;
    
  // Use httpclient class 
  HTTPClient http;
  http.begin(host, httpPort, request);
  
  // Serial.println("Calling Get");
          int httpCode = http.GET();
        if(httpCode>0) {
            // HTTP header has been sent and Server response header has been handled
            USE_SERIAL.printf("[HTTP] GET... code: %d\n", httpCode);

            // file found at server
            if(httpCode == 200) {
                String payload = http.getString();
            }
        } else {
            USE_SERIAL.printf("[HTTP] GET... failed, error: %s\n", http.errorToString(httpCode).c_str());
        }
}



// Holds the current button state.
volatile int state1;
volatile int laststate1;
volatile int state2;
volatile int laststate2;


// Holds the last time debounce was evaluated (in millis).
volatile long lastDebounceTime1 = 0;
volatile long lastDebounceTime2 = 0;

// The delay threshold for debounce checking.
const int debounceDelay = 100;

// Gets called by the interrupt.
void onChange1() {
  // Get the pin reading.
  int reading = digitalRead(tilt1);

  // Ignore dupe readings.
  if(reading == state1) return;

  boolean debounce = false;
  
  // Check to see if the change is within a debounce delay threshold.
  if((millis() - lastDebounceTime1) <= debounceDelay) {
    debounce = true;
  }

  // This update to the last debounce check is necessary regardless of debounce state.
  lastDebounceTime1 = millis();

  // Ignore reads within a debounce delay threshold.
  if(debounce) return;  

  // All is good, persist the reading as the state.
  laststate1 = state1;
  state1 = reading;
  if (state1 == 1) {
    tilt1OnTime =lastDebounceTime1;
  }

}

// Gets called by the interrupt.
void onChange2() {
  // Get the pin reading.
  int reading = digitalRead(tilt2);

  // Ignore dupe readings.
  if(reading == state2) return;

  boolean debounce = false;
  
  // Check to see if the change is within a debounce delay threshold.
  if((millis() - lastDebounceTime2) <= debounceDelay) {
    debounce = true;
  }

  // This update to the last debounce check is necessary regardless of debounce state.
  lastDebounceTime2 = millis();

  // Ignore reads within a debounce delay threshold.
  if(debounce) return;  

  // All is good, persist the reading as the state.
  laststate2 = state2;
  state2 = reading;

}


void setup() {
    Serial.begin(115200);
     delay(1000);
     
    pinMode(blueLED, OUTPUT);
  digitalWrite(blueLED, LOW);
  
  // Use LED to Indicate that we're connecting to wifi 
  pinMode(redLED, OUTPUT);
    digitalWrite(redLED, HIGH);

     // We start by connecting to a WiFi network

  Serial.println();
  Serial.println();
  Serial.print("Connecting to ");
  Serial.println(ssid);
  Serial.print("WiFi Status = ");
  Serial.println(WiFi.status());
  
  WiFi.begin(ssid, password);
  
  // Configure the pin mode as an input.
  pinMode(tilt1, INPUT_PULLUP);
    pinMode(tilt2, INPUT_PULLUP);
  

    // Are we connected yet?
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }

  Serial.println("");
  Serial.println("WiFi connected");  
    Serial.print("WiFi Status = ");
  Serial.println(WiFi.status());
  Serial.println("IP address: ");
  Serial.println(WiFi.localIP());
    
  digitalWrite(redLED, LOW);
digitalWrite(blueLED, HIGH);

    // run various example processes
  runCurl(helloCmd);
  
    // Attach an interrupt to the pin, assign the onChange function as a handler and trigger on changes (LOW or HIGH).
    attachInterrupt(tilt1, onChange1, CHANGE);
    attachInterrupt(tilt2, onChange2, CHANGE);

  
  Serial.println();
  Serial.println("Starting");
    Serial.println();

    tilt1OnTime = 0;
        tilt1OnTime = 0;
}


void loop() {
  // Main part of your loop code.
    digitalWrite(blueLED, !state1);
    digitalWrite(redLED, !state2);
    
  if (laststate1 != state1) {
    
    // Work with the value now.
    Serial.println("button 1 state change: " + String(state1));
    laststate1 = state1;

  
  }
    if (laststate2 != state2) {
    
    // Work with the value now.
    Serial.println("button 2 state change: " + String(state2));
    laststate2 = state2;


  }
    if ((millis() - tilt1OnTime < 500) && state2) {
     Serial.println("THROW!");
     tilt1OnTime = 0;
             runCurl(throwCmd);
  }
}


