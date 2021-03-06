using System;
using Microsoft.SPOT;

namespace HERO_Code {
    class ControlModeHandler {
        private enum AutonomousState {
            STOPPED,

            STARTUP,
            ORIENT,
            NAVIGATE_TO_EXCAVATION_ZONE,
            EXCAVATE,
            NAVIGATE_TO_DUMPING_ZONE,
            HOPPER_LINEUP,
            DUMP_THE_GOODS
        }

        AutonomousState autonState;

        Orient orient;

        public static class ControlMode {
            public const int TEST = 3;

            public const int DISABLED = 0;
            public const int TELEOP = 1;
            public const int AUTONOMOUS = 2;
        }

        //Stores the current control mode of the rover
        private int mode;

        //Equivalent to the dead man's switch
        private bool isRobotActive;

        //Initalize the control mode to a safe, disabled state
        public ControlModeHandler() {
            mode = ControlMode.DISABLED;
            isRobotActive = false;

            autonState = AutonomousState.STARTUP;

            
            orient = new Orient();
        }

        //Applies different values to the controller object based on the current control mode
        public void updateControllerValues(ref Controller controller, ref SerialCommsHandler serial) {
            isRobotActive = false;

            switch (mode) {

                //Use sensors and algorithms to apply values to the controller instead of a human operator
                case ControlMode.AUTONOMOUS:
                    AutonomousMode(ref controller, ref serial);
                    isRobotActive = true;
                    break;

                //Read in the human-controlled joystick over the serial connection
                case ControlMode.TELEOP:
                    TeleopMode(ref controller, ref serial);

                    //Activate the robot in teleop mode if the dead man's switch is held
                    isRobotActive = controller.BUTTONS.LB;
                    break;

                //For testing new code/functions
                case ControlMode.TEST:
                    TestMode(ref controller, ref serial);
                    isRobotActive = true;
                    break;

                //Do nothing
                case ControlMode.DISABLED:
                    controller.ResetValues();
                    break;

                //Invalid mode - print an error message
                default:
                    Debug.Print("ERROR - Invalid Mode: " + mode.ToString());
                    break;

            }
        }


        //Autonomous
        private void AutonomousMode(ref Controller logitechController, ref SerialCommsHandler NUC_SerialConnection) {
            switch (autonState) {
                case AutonomousState.STOPPED:
                    logitechController.ResetValues();
                    break;
                case AutonomousState.STARTUP:


                    break;

                case AutonomousState.ORIENT:

                    break;
                case AutonomousState.HOPPER_LINEUP:
                    HopperLineup.HopperLineupLoop(ref logitechController, ref NUC_SerialConnection);
                    break;
                default:
                    break;
            }
            HopperLineup.HopperLineupLoop(ref logitechController, ref NUC_SerialConnection);
        }

        //Update logitechController values with those sent wirelessly from the driver station
        private void TeleopMode(ref Controller logitechController, ref SerialCommsHandler NUC_SerialConnection) {
            NUC_SerialConnection.UpdateJoystickValues(ref logitechController);
        }

        //For running test code, testing new functions
        private void TestMode(ref Controller logitechController, ref SerialCommsHandler NUC_SerialConnection) {
            //HopperLineup.HorizontalDisplacementLoop(ref logitechController, ref NUC_SerialConnection);
            orient.Update(ref logitechController, ref NUC_SerialConnection);
        }




        // ---------------------- Getters and Setters ---------------------- //


        //Get and set the robot control mode
        public int GetMode() {
            return mode;
        }

        public void SetMode(int MODE) {
            this.mode = MODE;
        }

        //Returns if the robot is allowed to move, different from being disabled
        public bool IsRobotActive() {
            return isRobotActive;
        }


    }
}
