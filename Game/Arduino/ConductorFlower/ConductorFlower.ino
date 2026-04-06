#include <Arduino.h>
#include <FastLED.h>

// Define stepper motor connections and steps per revolution:
#define dirPin 2
#define stepPin 3
#define indLED 4
#define flowerLED 5
#define stepsPerRevolution 200

bool stringComplete = false;
String userInput = "";

int bloom = 1;
CRGB indLEDs[2];
CRGB flowerLEDs[2];

void setup() {
  pinMode(stepPin, OUTPUT);
  pinMode(dirPin, OUTPUT);
  pinMode(indLED, OUTPUT);
  pinMode(flowerLED, OUTPUT);

  pinMode(indLED, OUTPUT);
  FastLED.addLeds<WS2812, indLED, GRB>(indLEDs, 2);
  pinMode(flowerLED, OUTPUT);
  FastLED.addLeds<WS2812, flowerLED, GRB>(flowerLEDs, 2);

  userInput.reserve(100);
  Serial.begin(9600);
}

void flowersDown(int stage) {
  digitalWrite(dirPin, HIGH);

  // Spin the stepper motor 1 revolution slowly:
  for (int i = 0; i < stage * stepsPerRevolution; i++) {
    // These four lines result in 1 step:
    digitalWrite(stepPin, HIGH);
    delay(3);
    digitalWrite(stepPin, LOW);
    delay(3);
  }
}

void flowersUp(int stage) {
  digitalWrite(dirPin, LOW);

  // Spin the stepper motor 1 revolution quickly:
  for (int i = 0; i < stage * stepsPerRevolution; i++) {
    // These four lines result in 1 step:
    digitalWrite(stepPin, HIGH);
    delay(3);
    digitalWrite(stepPin, LOW);
    delay(3);
  }
}

void flowerToPosition(int pos) {
  int move = pos - bloom;
  if(move == 3) {
    flowersDown(1);
    flowersDown(1);
    flowersDown(1);
  } else if(move == 2) {
    flowersDown(1);
    flowersDown(1);
  } else if(move == 1) {
    flowersDown(1);
  } else if(move == -1) {
    flowersUp(1);
  } else if(move == -2) {
    flowersUp(1);
    flowersUp(1);
  } else if(move == -3) {
    flowersUp(1);
    flowersUp(1);
    flowersUp(1);
  }
  bloom = pos;
}

void setPos(int pos) {
  bloom = pos;
}

void loop() {
  readSerial();
  userInput.trim();

  if(stringComplete) {
    // DEBUG MODE: Move flower up and down manually
    if(userInput.equals("d")) {
      flowersDown(1);
    } else if(userInput.equals("u")) {
      flowersUp(1);
    // Set LED colors
    } else if(userInput.equals("r1")) {
      indLEDs[0] = CRGB(255, 0, 0);
      indLEDs[1] = CRGB(255, 0, 0);
      FastLED.show();
    } else if(userInput.equals("r2")) {
      flowerLEDs[0] = CRGB(255, 0, 0);
      flowerLEDs[1] = CRGB(255, 0, 0);
      FastLED.show();
    } else if(userInput.equals("b1")) {
      indLEDs[0] = CRGB(0, 0, 255);
      indLEDs[1] = CRGB(0, 0, 255);
      FastLED.show();
    } else if(userInput.equals("b2")) {
      flowerLEDs[0] = CRGB(0, 0, 255);
      flowerLEDs[1] = CRGB(0, 0, 255);
      FastLED.show();
    } else if(userInput.equals("c1")) {
      indLEDs[0] = CRGB(0, 255, 127);
      indLEDs[1] = CRGB(0, 255, 127);
      FastLED.show();
    } else if(userInput.equals("c2")) {
      flowerLEDs[0] = CRGB(0, 255, 127);
      flowerLEDs[1] = CRGB(0, 255, 127);
      FastLED.show();
    } else if(userInput.equals("y1")) {
      indLEDs[0] = CRGB(255, 255, 0);
      indLEDs[1] = CRGB(255, 255, 0);
      FastLED.show();
    } else if(userInput.equals("y2")) {
      flowerLEDs[0] = CRGB(255, 255, 0);
      flowerLEDs[1] = CRGB(255, 255, 0);
      FastLED.show();
    // Turn LED off
    } else if(userInput.equals("k1")) {
      indLEDs[0] = CRGB(0, 0, 0);
      indLEDs[1] = CRGB(0, 0, 0);
      FastLED.show();
    } else if(userInput.equals("k2")) {
      flowerLEDs[0] = CRGB(0, 0, 0);
      flowerLEDs[1] = CRGB(0, 0, 0);
      FastLED.show();
    // Set flower position
    } else if(userInput.equals("0")) {
      flowerToPosition(1);
    } else if(userInput.equals("1")) {
      flowerToPosition(2);
    } else if(userInput.equals("2")) {
      flowerToPosition(3);
    } else if(userInput.equals("3")) {
      flowerToPosition(4);
    // DEBUG MODE: Set global flower position var
    } else if(userInput.equals("0p")) {
      setPos(1);
    } else if(userInput.equals("1p")) {
      setPos(2);
    } else if(userInput.equals("2p")) {
      setPos(3);
    } else if(userInput.equals("3p")) {
      setPos(4);
    }
    stringComplete = false;
    userInput = "";
  }
}

void readSerial() {
  if (Serial.available()) {
    char inChar = (char)Serial.read();
    userInput += inChar;
    if (inChar == '\n') {
      stringComplete = true;
    }
  }
}