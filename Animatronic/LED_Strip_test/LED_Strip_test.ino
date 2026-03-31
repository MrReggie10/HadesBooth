
#include <FastLED.h>

const int NUM_LEDS = 144;

CRGB leds[NUM_LEDS];

void setup() {
  // put your setup code here, to run once:
  pinMode(3, OUTPUT);
  FastLED.addLeds<WS2812, 3, GRB>(leds, NUM_LEDS);
}

void loop() {
  for(int i = 0; i < NUM_LEDS; i++) {
    leds[i] = CRGB(255, 0, 0);
  }

  FastLED.show();
  delay(1000);
  // put your main code here, to run repeatedly:
  
}
