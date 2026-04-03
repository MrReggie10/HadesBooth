
#include <FastLED.h>

CRGB leds[2];

void setup() {
  pinMode(3, OUTPUT);
  FastLED.addLeds<WS2812, 3, GRB>(leds, 2);
  Serial.begin(115200);
}

void loop() {
  leds[0] = CRGB(255, 0, 0);
  leds[1] = CRGB(255, 0, 0);
  FastLED.show();
  delay(100);

  // put your main code here, to run repeatedly:
  
}
