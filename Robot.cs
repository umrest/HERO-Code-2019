using System;
using System.Collections;
using Microsoft.SPOT;
using CTRE.Phoenix;
using CTRE.Phoenix.Controller;
using CTRE.Phoenix.MotorControl;
using CTRE.Phoenix.MotorControl.CAN;

namespace HERO_Code_2019 {
    class Robot {

        //Create a serial connection with the NUC
        SerialCommsHandler NUC_SerialConnection;

        //Initialize the handler for switching between different control modes (ie Teleop, Autonomous, Test, etc)
        ControlModeHandler controlModeHandler;

        //Initialize a logitech joystick object
        Controller logitechController;

        //Initialize the drive base motor controllers
        DriveBase driveBase = new DriveBase();



        public Robot() {

            //Initialize a serial connection with the Intel NUC
            NUC_SerialConnection = new SerialCommsHandler(SerialCommsHandler.Constants.Port.Port1);

            //Initialize the handler for switching between different control modes (ie Teleop, Autonomous, Test, etc)
            controlModeHandler = new ControlModeHandler();

            //Initialize a logitech joystick object
            logitechController = new Controller();

            //Initialize the drive base motor controllers
            driveBase = new DriveBase();
        }

        public void Run() {

            //Read incoming serial packets
            NUC_SerialConnection.ReadFromNUC();

            //Check for dashboard enable signal in order for the robot to be enabled
            bool ROBOT_ENABLED = NUC_SerialConnection.isRobotEnabled();

            //Reset the robot control mode to disabled
            controlModeHandler.SetMode(ControlModeHandler.ControlMode.DISABLED);


            if (ROBOT_ENABLED) {

                Debug.Print("ENABLED!!!");

                //Heartbeat pulse to the motors to enable them
                //Motor controllers will disable automatically if this is not regularly received
                CTRE.Phoenix.Watchdog.Feed();

                //Send signal to overwrite all motors to 0 if dead man's switch is not pressed
                ROBOT_ENABLED = logitechController.BUTTONS.LB;

                //Update the robot control mode and send over the logitech controller object to be updated
                if (ROBOT_ENABLED) {
                    controlModeHandler.SetMode(ControlModeHandler.ControlMode.TEST);
                    controlModeHandler.updateControllerValues(ref logitechController, ref NUC_SerialConnection);
                }




                //MoveMotors
                driveBase.Drive(ref logitechController, ROBOT_ENABLED);
            }
        }


    }

}
