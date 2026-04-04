#include <Arduino.h>

// Define stepper motor connections and steps per revolution:
#define dirPin 2
#define stepPin 3
#define stepsPerRevolution 200

bool stringComplete = false;
String userInput = "";

void setup() {
  // Declare pins as output:
  pinMode(stepPin, OUTPUT);
  pinMode(dirPin, OUTPUT);

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

  delay(1000);
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

  delay(1000);
}

void loop() {
  readSerial();
  userInput.trim();

  if(stringComplete) {
    if(userInput.equals("d")) {
      flowersDown(1);
    } else if(userInput.equals("u")) {
      flowersUp(1);
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