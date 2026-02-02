/******************************************************************************
**************************** HADESBOOTH ANIMATRONIC ***************************
******************************************************************************/

/*
 Description:


 Created 2 Feb 2026
 by Jacob Yakubisin
*/

#include <Stepper.h>

const int stepsPerRevolution = 200;  // change this to fit the number of steps per revolution
// for your motor


// initialize the stepper library on pins 8 through 11:
Stepper myStepper(stepsPerRevolution, 8, 9, 10, 11);

int stepCount = 1;  // number of steps the motor has taken

int motorSpeed = 0;
int speedDelta = 1;

void setup() {
  // nothing to do inside the setup
}

void loop() {
  if(motorSpeed > 50 || motorSpeed <= 0) {
    speedDelta *= -1;
  }
  motorSpeed += speedDelta;
  // set the motor speed:
  if (motorSpeed > 0) {
    myStepper.setSpeed(motorSpeed);
    myStepper.step(stepsPerRevolution / 100);
  }
}