#include <Arduino.h>
#include <Encoder.h>
#include <Bounce.h>

// Pin definitions
const int BUTTON1_PIN = 2;
const int BUTTON2_PIN = 3;
const int ENCODER_PIN_A = 4;
const int ENCODER_PIN_B = 5;
const int ENCODER_BUTTON_PIN = 6;

// Create encoder object
Encoder myEncoder(ENCODER_PIN_A, ENCODER_PIN_B);

// Create Bounce objects for buttons with 10ms debounce time
Bounce button1 = Bounce(BUTTON1_PIN, 10);
Bounce button2 = Bounce(BUTTON2_PIN, 10);
Bounce encoderButton = Bounce(ENCODER_BUTTON_PIN, 10);

// Variable to store encoder position
long encoderPosition = 0;
long lastEncoderPosition = 0;

// Direction values for joystick (-1, 0, or 1)
int encoderDirection = 0;
unsigned long lastEncoderMoveTime = 0;
const unsigned long encoderTimeout = 50; // ms to reset direction to 0 after no movement

void setup() {
  // Configure pins
  pinMode(BUTTON1_PIN, INPUT_PULLUP);
  pinMode(BUTTON2_PIN, INPUT_PULLUP);
  pinMode(ENCODER_BUTTON_PIN, INPUT_PULLUP);

  // Start serial communication for debugging
  Serial.begin(9600);
  Serial.println("Tempest Controller Ready:");
}

void loop() {
  // Update bounce objects
  button1.update();
  button2.update();
  encoderButton.update();

  // Read encoder position
  encoderPosition = myEncoder.read();
  
  // Check if encoder position changed and set direction
  if (encoderPosition != lastEncoderPosition) {
    // Determine direction (-1 for left, +1 for right)
    // Note: We're swapping the directions as requested previously
    if (encoderPosition > lastEncoderPosition) {
      encoderDirection = -1; // LEFT
      Serial.print("Encoder: LEFT (");
      Serial.print(encoderDirection);
      Serial.println(")");
    } else {
      encoderDirection = 1;  // RIGHT
      Serial.print("Encoder: RIGHT (");
      Serial.print(encoderDirection);
      Serial.println(")");
    }
    lastEncoderPosition = encoderPosition;
    lastEncoderMoveTime = millis();
  } else {
    // Reset direction to 0 if no movement for a while
    if (encoderDirection != 0 && (millis() - lastEncoderMoveTime > encoderTimeout)) {
      encoderDirection = 0;
    }
  }
  
  // Map encoder direction to joystick X axis
  // Convert from -1/0/+1 to joystick range (0-1023)
  int joystickValue = 512 + (encoderDirection * 512); // 0=left, 512=center, 1023=right
  Joystick.X(joystickValue);

  // Handle button 1 (q key)
  if (button1.fallingEdge()) {
    Keyboard.press('q');
    Serial.println("Q pressed");
  }
  if (button1.risingEdge()) {
    Keyboard.release('q');
    Serial.println("Q released");
  }

  // Handle button 2 (w key)
  if (button2.fallingEdge()) {
    Keyboard.press('w');
    Serial.println("W pressed");
  }
  if (button2.risingEdge()) {
    Keyboard.release('w');
    Serial.println("W released");
  }

  // Handle encoder button (e key)
  if (encoderButton.fallingEdge()) {
    Keyboard.press('e');
    Serial.println("E pressed");
  }
  if (encoderButton.risingEdge()) {
    Keyboard.release('e');
    Serial.println("E released");
  }

  // Small delay to avoid flooding
  delay(10);
}