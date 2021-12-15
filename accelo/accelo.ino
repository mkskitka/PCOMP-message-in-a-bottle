#include "Arduino_LSM6DS3.h"
#include "MadgwickAHRS.h"

// initialize a Madgwick filter:
Madgwick filter;
// sensor's sample rate is fixed at 104 Hz:
const float sensorRate = 104.00;

// values for orientation:
float roll = 0.0;
float pitch = 0.0;
float heading = 0.0;
int frame = 0;
int increment = 10;

void setup() {
  Serial.begin(9600);
  Serial.flush();
  // attempt to start the IMU:
  if (!IMU.begin()) {
    Serial.println("Failed to initialize IMU");
    // stop here if you can't access the IMU:
    while (true);
  }


  // start the filter to run at the sample rate:
  filter.begin(sensorRate);
}

void loop() {
  frame = (frame + 1) % increment;
  // values for acceleration and rotation:
  float xAcc, yAcc, zAcc;
  float xGyro, yGyro, zGyro;

  // check if the IMU is ready to read:
  if (IMU.accelerationAvailable() &&
      IMU.gyroscopeAvailable()) {
    // read accelerometer and gyrometer:
    IMU.readAcceleration(xAcc, yAcc, zAcc);
    IMU.readGyroscope(xGyro, yGyro, zGyro);

    // update the filter, which computes orientation:
    filter.updateIMU(xGyro, yGyro, zGyro, xAcc, yAcc, zAcc);

    // print the heading, pitch and roll
    roll = filter.getRoll();
    pitch = filter.getPitch();
    heading = filter.getYaw();


    // if you get a byte in the serial port,
    // send the latest heading, pitch, and roll:

  }
  if (Serial.available()) {
    char input = Serial.read();

    //print the filter's results:

      Serial.print(roll);
      Serial.print(",");
      Serial.print(pitch);
      Serial.print(",");
      Serial.print(heading);
      Serial.print(",");
      Serial.print(xGyro);
      Serial.print(",");
      // up down exceleration 
      Serial.print(yGyro);
      Serial.print(",");
      //rotate on table around center acc
      Serial.println(zGyro);
  }
}
