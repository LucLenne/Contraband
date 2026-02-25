/*
  SerialNumberRead
 Read the serial number of the IC card.
 
 TIDY by YFRobot < http://www.yfrobot.com >
 */

#include <SPI.h>
#include <RFID.h>

//D10 - 读卡器CS引脚、D5 - 读卡器RST引脚
//D10 - broche CS du lecteur de carte, D5 - broche RST du lecteur de carte
class Led {
public:
  int PinRed;
  int PinGreen;
  int PinBlue;
  int ledState = 0;

  Led(int redPin, int greenPin, int bluePin) {
    PinRed = redPin;
    PinGreen = greenPin;
    PinBlue = bluePin;
  }
};



Led *ledA = nullptr;
Led *ledB = nullptr;
RFID rfid(10, 9);
unsigned char status;
unsigned char str[MAX_LEN];

void setup() {
  Serial.begin(9600);
  // rfid
  SPI.begin();
  rfid.init();
  // led
  ledA = new Led(2, 3, 4);
  ledB = new Led(5, 6, 7);
  setLed(ledA);
  setLed(ledB);
  writeLed(ledA, 250, 0, 0);
  writeLed(ledB, 0, 0, 0);
}
// led
void setLed(Led *led) {
  pinMode(led->PinRed, OUTPUT);
  pinMode(led->PinBlue, OUTPUT);
  pinMode(led->PinGreen, OUTPUT);
}

void writeLed(Led *led, int rVal, int gVal, int bVal) {
  analogWrite(led->PinRed, rVal);
  analogWrite(led->PinGreen, gVal);
  analogWrite(led->PinBlue, bVal);
}


int recvSerial() {
  if (Serial.available()) {
    int serialData = Serial.read();
    switch (serialData) {
      case '4':
        return 4;
      case '3':
        return 3;
      case '2':
        return 2;
      case '1':
        return 1;
      case '0':
        return 0;
      default:
        return -1;
    }
  }
  return -1;
}

// void setup() {
//   Serial.begin(9600);
//   // rfid
//   SPI.begin();
//   rfid.init();
//   // led - CHANGED PINS to avoid SPI conflicts
//   ledA = new Led(2, 3, 4);
//   ledB = new Led(8, 9, A0);  // Using pin 8, 9 (PWM), and A0
//   setLed(ledA);
//   setLed(ledB);
//   writeLed(ledA, 250, 0, 0);
//   writeLed(ledB, 0, 0, 0);
// }

void ledCheck() {
  int ledState = recvSerial();
  switch (ledState) {
    case 4:
      {  // wait - added braces
        int i = 0;
        while (i < 5) {
          writeLed(ledB, 125, 125, 0);
          delay(20);
          writeLed(ledB, 0, 0, 0);
          delay(20);
          i++;
        }
        break;
      }
    case 3:  // failed
      writeLed(ledB, 250, 0, 0);
      break;
    case 2:  // mid
      writeLed(ledB, 250, 250, 0);
      break;
    case 1:  // correct
      writeLed(ledB, 0, 250, 0);
      break;
    case 0:  // off
      writeLed(ledB, 0, 0, 0);
      break;
    default:
      break;
  }
  delay(100);
  writeLed(ledB, 0, 0, 0);
}
// rncp

void RNCPRead() {
  //Search card, return card types
  if (rfid.findCard(PICC_REQIDL, str) == MI_OK) {
    // Détection anti-collision, lecture du numéro de série de la carte
    if (rfid.anticoll(str) == MI_OK) {
      //Afficher le numéro de série de la carte
      for (int i = 0; i < 4; i++) {
        Serial.print(0x0F & (str[i] >> 4), HEX);
        Serial.print(0x0F & str[i], HEX);
      }
    }
    rfid.selectTag(str);
  }
  rfid.halt();
}

void loop() {
  // rncp
  RNCPRead();
  Serial.println("");
  // led
  ledCheck();
}
