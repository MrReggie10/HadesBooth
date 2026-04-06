#include <Arduino.h>
#include <FastLED.h>

// ─── Configuration ────────────────────────────────────────────────────────────
#define MAX_EVENTS    64     // Maximum number of tuples we can store
#define MAX_COLOR_LEN 32     // Maximum length of a color string
#define SERIAL_BAUD   9600

// ─── Data Structure ───────────────────────────────────────────────────────────
struct Event {
  unsigned long triggerTime;   // Absolute time (ms) to fire this event
  uint8_t       address;       // 0–6
  char          color[MAX_COLOR_LEN];
};

Event    events[MAX_EVENTS];
int      eventCount  = 0;
int      nextEvent   = 0;        // Index of the next event to process

// ─── Serial Input Buffer ──────────────────────────────────────────────────────
#define INPUT_BUFFER_SIZE 512
char    inputBuffer[INPUT_BUFFER_SIZE];
int     bufferIndex  = 0;
bool    receiving    = false;    // True once we've seen the leading 'L'

// ─── LED Initialization ───────────────────────────────────────────────────────
CRGB leds[7];

// ─── Helpers ──────────────────────────────────────────────────────────────────

// Grab the next whitespace-delimited token from *p, advance *p past it.
// Returns true if a token was found.
bool nextToken(char **p, char *out, size_t outLen) {
  // Skip leading spaces
  while (**p == ' ') (*p)++;

  if (**p == '\0') return false;

  size_t i = 0;
  while (**p != ' ' && **p != '\0' && i < outLen - 1) {
    out[i++] = **p;
    (*p)++;
  }
  out[i] = '\0';
  return i > 0;
}

// Sort events[0..eventCount-1] by triggerTime ascending (insertion sort).
// Insertion sort is preferred here — it's simple, in-place, and perfectly
// efficient for the small N values expected on a microcontroller.
void sortEvents() {
    for (int i = 1; i < eventCount; i++) {
        Event key = events[i];
        int j = i - 1;
        while (j >= 0 && (long)(events[j].triggerTime - key.triggerTime) > 0) {
            events[j + 1] = events[j];
            j--;
        }
        events[j + 1] = key;
    }
}

// Parse and load events from inputBuffer. Returns true on success.
bool parseMessage(unsigned long receiveTime) {
  char *p = inputBuffer;
  char  tok[MAX_COLOR_LEN];

  // Consume the leading 'L' token
  if (!nextToken(&p, tok, sizeof(tok)) || tok[0] != 'L') return false;

  // Read tuple count
  if (!nextToken(&p, tok, sizeof(tok))) return false;
  int count = atoi(tok);

  if (count <= 0 || count > MAX_EVENTS) return false;

  // Read each tuple
  for (int i = 0; i < count; i++) {
    char offsetTok[16], addrTok[4], colorTok[MAX_COLOR_LEN];

    if (!nextToken(&p, offsetTok, sizeof(offsetTok))) return false;
    if (!nextToken(&p, addrTok,   sizeof(addrTok)))   return false;
    if (!nextToken(&p, colorTok,  sizeof(colorTok)))  return false;

    unsigned long offset = strtoul(offsetTok, nullptr, 10);
    int           addr   = atoi(addrTok);

    if (addr < 0 || addr > 6) return false;

    events[i].triggerTime = receiveTime + offset;
    events[i].address     = (uint8_t)addr;
    strncpy(events[i].color, colorTok, MAX_COLOR_LEN - 1);
    events[i].color[MAX_COLOR_LEN - 1] = '\0';
  }

  eventCount = count;
  nextEvent  = 0;
  sortEvents();
  return true;
}

// ─── Process a fired event ────────────────────────────────────────────────────
void handleEvent(const Event &e) {
  // Replace this body with your real logic (e.g. set an LED color).
  Serial.print(F("[EVENT] time="));
  Serial.print(e.triggerTime);
  Serial.print(F("  addr="));
  Serial.print(e.address);
  Serial.print(F("  color="));
  Serial.println(e.color);
  int addr = (int) e.address;
  if(e.color[0] == 'r') {
    leds[addr] = CRGB(255, 0, 0);
    Serial.println("Red");
  } else if(e.color[0] == 'b') {
    leds[addr] = CRGB(0, 0, 255);
    Serial.println("Blue");
  } else if(e.color[0] == 'c') {
    leds[addr] = CRGB(0, 255, 127);
    Serial.println("Cyan");
  } else if(e.color[0] == 'y') {
    leds[addr] = CRGB(255, 255, 0);
    Serial.println("Yellow");
  } else {
    leds[addr] = CRGB(0, 255, 0);
  }
  FastLED.show();
}

// ─── Arduino Lifecycle ────────────────────────────────────────────────────────
void setup() {
  pinMode(4, OUTPUT);
  FastLED.addLeds<WS2812, 4, GRB>(leds, 7);
  Serial.begin(SERIAL_BAUD);
  Serial.println(F("Ready. Send: L <count> <ms> <addr> <color> ..."));
}

void loop() {
  // ── 1. Read serial input character by character ──────────────────────────
  while (Serial.available()) {
    char c = (char)Serial.read();

    // 'L' at the start of a fresh line begins a new message
    if (!receiving && c == 'L') {
      receiving   = true;
      bufferIndex = 0;
      inputBuffer[bufferIndex++] = c;
      continue;
    }

    if (receiving) {
      if (c == '\n' || c == '\r') {
        // End of message – terminate string and parse
        inputBuffer[bufferIndex] = '\0';
        unsigned long receiveTime = millis();

        if (parseMessage(receiveTime)) {
          Serial.print(F("Loaded "));
          Serial.print(eventCount);
          Serial.println(F(" event(s)."));
        } else {
          Serial.println(F("Parse error."));
        }

        receiving   = false;
        bufferIndex = 0;

      } else if (bufferIndex < INPUT_BUFFER_SIZE - 1) {
        inputBuffer[bufferIndex++] = c;
      } else {
        // Buffer overflow – discard message
        Serial.println(F("Buffer overflow."));
        receiving   = false;
        bufferIndex = 0;
      }
    }
  }

  // ── 2. Fire any events whose time has come ───────────────────────────────
  unsigned long now = millis();
  while (nextEvent < eventCount && (long)(now - events[nextEvent].triggerTime) >= 0) {
    handleEvent(events[nextEvent]);
    nextEvent++;
  }
}
