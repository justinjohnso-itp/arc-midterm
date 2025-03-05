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

// Map encoder position to joystick movement
int mapEncoderToJoystick(long encoderPos) {
  // Map encoder position to joystick range (-512 to 512)
  // Adjust the division factor to control sensitivity
  return constrain(encoderPos / 2, -512, 512);
}

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
  
  // Map encoder to X (horizontal) axis of joystick
  Joystick.X(mapEncoderToJoystick(encoderPosition) + 512); // Add 512 to get 0-1023 range

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

  // Reset encoder position if it gets too large or small
  if (abs(encoderPosition) > 1000) {
    myEncoder.write(encoderPosition > 0 ? 1000 : -1000);
  }

  // Small delay to avoid flooding
  delay(10);
}