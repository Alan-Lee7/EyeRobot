#include "WiiChuck.h"
#include "Wire.h"

#include <WiFi.h>
#include <NetworkClient.h>
#include <WebServer.h>
#include <ESPmDNS.h>

// Declare variables
const int Motor1Pin1 = 23;
const int Motor1Pin2 = 22;
const int enA = 21;
const int Motor2Pin1 = 20;
const int Motor2Pin2 = 19;
const int enB = 18;
int pwmValue = 255;
int sda_pin = 4;
int scl_pin = 5;
Accessory nunchuck1;

const char *ssid = "EyeRobot2";
const char *password = "gimp-eating-taco";

bool ledON = false;

WebServer server(80);

void getLeashData()
{
  if (!ledON) {
    neopixelWrite(RGB_BUILTIN, 0, 32, 0);
    ledON = true;
  } else {
    digitalWrite(RGB_BUILTIN, LOW);
    ledON = false;
  }

  Serial.println("sending leash data");

  uint8_t joyX = nunchuck1.values[0];
  uint8_t joyY = nunchuck1.values[1];
  uint8_t rollAngle = nunchuck1.values[2];
  uint8_t pitchAngle = nunchuck1.values[3];
  uint8_t accelX = nunchuck1.values[4];
  uint8_t accelY = nunchuck1.values[6];
  uint8_t accelZ = nunchuck1.values[6];
  uint8_t buttonZ = nunchuck1.values[10];
  uint8_t buttonC = nunchuck1.values[11];

  String message = "";
  message += joyX;
  message += ",";
  message += joyY;
  message += ",";
  message += rollAngle;
  message += ",";
  message += pitchAngle;
  message += ",";
  message += accelX;
  message += ",";
  message += accelY;
  message += ",";
  message += accelZ;
  message += ",";
  message += buttonZ;
  message += ",";
  message += buttonC;

  server.send(200, "text/plain", message);
}

void setup() {
  pinMode(Motor1Pin1, OUTPUT);
  pinMode(Motor1Pin2, OUTPUT);
  pinMode(enA, OUTPUT);
  pinMode(enB, OUTPUT);
  pinMode(Motor2Pin1, OUTPUT);
  pinMode(Motor2Pin2, OUTPUT);
  Serial.begin(9600); // Start serial communication at 9600 baud rate
  Wire.setPins(sda_pin,scl_pin);
  Wire.begin();
  nunchuck1.begin();
  nunchuck1.type = NUNCHUCK;

  if (!WiFi.softAP(ssid, password)) {
    Serial.println("Failed to create WiFi AP");
    while (true);
  }

  Serial.println(WiFi.softAPIP());

  server.on("/", [](){
    server.send(200, "text/plain", "pong");
  });

  server.on("/leash", getLeashData);

  server.on("/forward", [](){
    Forward();
  });

  server.on("/left", [](){
    Left();
  });

  server.on("/right", [](){
    Right();
  });

  server.on("/backward", [](){
    Backward();
  });

  server.on("/stop", [](){
    Stop();
  });

  server.begin();
}

void Forward(){ // Drive Forward
    digitalWrite(Motor2Pin1, LOW);
        digitalWrite(Motor2Pin2, LOW);
        analogWrite(enA, pwmValue);
        digitalWrite(Motor1Pin1, HIGH);
        digitalWrite(Motor1Pin2, LOW);
       
        
}
void Backward(){// Drive Backwards
        digitalWrite(Motor2Pin1, LOW);
        digitalWrite(Motor2Pin2, LOW);
       analogWrite(enA, pwmValue);
        digitalWrite(Motor1Pin1, LOW);
        digitalWrite(Motor1Pin2, HIGH);
       
        
}
void Left(){// Drive Right
          digitalWrite(Motor1Pin1, LOW);
         digitalWrite(Motor1Pin2, LOW);
       analogWrite(enB, pwmValue);
       digitalWrite(Motor2Pin1, LOW);
        digitalWrite(Motor2Pin2, HIGH);
       
        
}
void Right(){ //Drive RIght
        digitalWrite(Motor1Pin1, LOW);
        digitalWrite(Motor1Pin2, LOW);
        analogWrite(enB, pwmValue);
        digitalWrite(Motor2Pin1, HIGH);
        digitalWrite(Motor2Pin2, LOW);
        
        
}
void Right_Diagonal(){ //Drive RIght
       
      

        analogWrite(enB, pwmValue);
        analogWrite(enA, pwmValue);

        digitalWrite(Motor1Pin1, HIGH);
        digitalWrite(Motor1Pin2, LOW);
        digitalWrite(Motor2Pin1, HIGH);
        digitalWrite(Motor2Pin2, LOW);
}
void Left_Diagonal(){ //Drive RIght
       
      

        analogWrite(enB, pwmValue);
        analogWrite(enA, pwmValue);

        digitalWrite(Motor1Pin1, HIGH);
        digitalWrite(Motor1Pin2, LOW);
        digitalWrite(Motor2Pin1, LOW);
        digitalWrite(Motor2Pin2, HIGH);
}
void Stop(){ //stop motors
   digitalWrite(Motor1Pin1, LOW);
      digitalWrite(Motor1Pin2, LOW);
        digitalWrite(Motor2Pin1, LOW);
        digitalWrite(Motor2Pin2, LOW);
}
void pwmUp(){ //Speed Motors Up
   pwmValue = min(pwmValue + 25, 255); // Ensure PWM value does not exceed 255
        Serial.print("PWM Value: ");
        Serial.println(pwmValue);
}
void pwmDown(){ //Slow Motors Down
  pwmValue = max(pwmValue - 25, 0); // Ensure PWM value does not go below 0
        Serial.print("PWM Value: ");
        Serial.println(pwmValue);
}

void loop() {
  nunchuck1.readData();
  //nunchuck1.printInputs();
  // Serial.println( String((uint8_t)nunchuck1.values[11]) + " " + String((uint8_t) nunchuck1.values[10]));

  if (Serial.available() > 0) {
    char command = Serial.read();

    switch (command) {
      case 's': // go forwards when 'w' pressed
         Forward();
          break;

      case 'w': // go backwards when 's' pressed
      Backward();
         break;

       case 'a': // go left when 'a' pressed
        Left();
        break;

        case 'd': // go right when 'd' pressed
         Right();
        break;

        case 'e': // go right diagonal when 'e' pressed
         Right_Diagonal();
        break;
        case 'q': // go right diagonal when 'e' pressed
         Left_Diagonal();
        break;

      case '3': // Stop when '3' pressed
       Stop();
        break;

      case '1': // Increase PWM by 25 when '1' pressed
        pwmUp();
        break;

      case '2': // Decrease PWM by 25 when '2' pressed
        pwmDown();
        break;
    
        
        
      default:

        // Invalid input, do nothing
        break;
    }
  }

  server.handleClient();
}