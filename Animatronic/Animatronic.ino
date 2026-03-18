// Example sketch to control a stepper motor with A4988 stepper motor driver 
// and Arduino without a library. 
// https://www.makerguides.com

// Define stepper motor connections and steps per revolution:
#define dirPin 2
#define stepPin 3
#define stepsPerRevolution 200

void setup() {
  // Declare pins as output:
  pinMode(stepPin, OUTPUT);
  pinMode(dirPin, OUTPUT);
}

void loop() {
  // Code to make flowers go down
  digitalWrite(dirPin, HIGH);

  // Spin the stepper motor 1 revolution slowly:
  for (int i = 0; i < stepsPerRevolution; i++) {
    // These four lines result in 1 step:
    digitalWrite(stepPin, HIGH);
    delayMicroseconds(2000);
    digitalWrite(stepPin, LOW);
    delayMicroseconds(2000);
  }

  delay(1000);

  // // Code to make flowers go up:
  // digitalWrite(dirPin, LOW);

  // // Spin the stepper motor 1 revolution quickly:
  // for (int i = 0; i < stepsPerRevolution; i++) {
  //   // These four lines result in 1 step:
  //   digitalWrite(stepPin, HIGH);
  //   delayMicroseconds(2000);
  //   digitalWrite(stepPin, LOW);
  //   delayMicroseconds(2000);
  // }

  // delay(1000);

}