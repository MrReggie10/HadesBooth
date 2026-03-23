/*
Animatronic Dino Controler
By: Matthew Saenz
*/

#include <Arduino.h>
#include <FastLED.h>

const int LED_ADDR_1 = 0;
const int LED_ADDR_2 = 6;
const int LED_ADDR_3 = 12;
const int LED_ADDR_4 = 18;

const int LED_D_PIN = 13;
const int NUM_LEDS = 144;

CRGB leds[NUM_LEDS];

// global variables for readSerial() to set
String userInput = "";
bool stringComplete = false;

void setup() {
  pinMode(LED_D_PIN, OUTPUT);
  
  userInput.reserve(100);

  FastLED.addLeds<WS2812, DATA_PIN, GRB>(leds, NUM_LEDS);

  Serial.begin(115200); // Starts the serial communication
}

void loop() {
  readSerial();
  
  if(stringComplete){
    // process user input
    userInput.trim();
    unsigned long start_time = millis();
    if(userInput.equals("r")){
      // red: come out of hiding, light flashes
      leds[LED_ADDR_1] = CRGB(255, 0, 0);
      leds[LED_ADDR_2] = CRGB(0, 0, 0);
      leds[LED_ADDR_3] = CRGB(0, 0, 0);
      leds[LED_ADDR_4] = CRGB(0, 0, 0);
      Serial.print("d\n");
    } else if (userInput.equals("b")){
      // hide: return to hiding
      leds[LED_ADDR_1] = CRGB(0, 0, 0);
      leds[LED_ADDR_2] = CRGB(0, 0, 255);
      leds[LED_ADDR_3] = CRGB(0, 0, 0);
      leds[LED_ADDR_4] = CRGB(0, 0, 0);
      Serial.print("d\n");
    } else if (userInput.equals("y")){
      // growl: gnash teeth
      leds[LED_ADDR_1] = CRGB(0, 0, 0);
      leds[LED_ADDR_2] = CRGB(0, 0, 0);
      leds[LED_ADDR_3] = CRGB(255, 255, 0);
      leds[LED_ADDR_4] = CRGB(0, 0, 0);
      Serial.print("d\n");
    } else if (userInput.equals("c")){
      // growl: gnash teeth
      leds[LED_ADDR_1] = CRGB(0, 0, 0);
      leds[LED_ADDR_2] = CRGB(0, 0, 0);
      leds[LED_ADDR_3] = CRGB(0, 0, 0);
      leds[LED_ADDR_4] = CRGB(255, 255, 0);
      Serial.print("d\n");
    } else {

      Serial.print("n\n");
    }
    stringComplete = false;
    userInput = "";
    FastLED.show();
  }
  delay(10);
}

// example from Christian's Microcontroller lecture
// read from serial in a non-blocking manner
void readSerial() {
  if (Serial.available()) {
    char inChar = (char)Serial.read();
    userInput += inChar;
    if (inChar == '\n') {
      stringComplete = true;
    }
  }
}
