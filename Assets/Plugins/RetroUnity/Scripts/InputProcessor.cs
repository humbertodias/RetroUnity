using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace RetroUnity
{
    // https://tcrf.net/Controller_Test_Cartridge
    public class InputProcessor : MonoBehaviour
    {
        private string currentAction = "";
        Dictionary<string, bool> action = new Dictionary<string,bool>();

        private void Awake()
        {
            InputSystem.onActionChange +=
                (obj, change) =>
                {
                    // obj can be either an InputAction or an InputActionMap
                    // depending on the specific change.
                    switch (change)
                    {
                        case InputActionChange.ActionStarted:
                        case InputActionChange.ActionPerformed:
                            var keyActionTrue = ((InputAction) obj).name;
                            action[keyActionTrue] = true;
                            currentAction = $"{((InputAction) obj).name}  {change}";
                            break;
                        case InputActionChange.ActionCanceled:
                            var keyActionFalse = ((InputAction) obj).name;
                            action[keyActionFalse] = false;
                            currentAction = $"{((InputAction) obj).name}  {change}";
                            break;
                    }
                };
        }

        // https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/manual/Gamepad.html
        void FixedUpdate()
        {
            var gamepad = Gamepad.current;
            if (gamepad != null)
            {
                ProcessGamePad(gamepad);
            }
        }

        private void ProcessGamePad(Gamepad gamepad)
        {
            action["Up"] = gamepad[GamepadButton.DpadUp].isPressed;
            action["Down"] = gamepad[GamepadButton.DpadDown].isPressed;
            action["Left"] = gamepad[GamepadButton.DpadLeft].isPressed;
            action["Right"] = gamepad[GamepadButton.DpadRight].isPressed;
            action["Select"] = gamepad[GamepadButton.Select].isPressed;
            action["Start"] = gamepad[GamepadButton.Start].isPressed;
            action["Square"] = gamepad[GamepadButton.Square].isPressed;
            action["Triangle"] = gamepad[GamepadButton.Triangle].isPressed;
            action["Cross"] = gamepad[GamepadButton.Cross].isPressed;
            action["Circle"] = gamepad[GamepadButton.Circle].isPressed;
            action["L1"] = gamepad[GamepadButton.LeftShoulder].isPressed;
            action["L2"] = gamepad[GamepadButton.LeftStick].isPressed;
            action["L3"] = gamepad[GamepadButton.LeftTrigger].isPressed;
            action["R1"] = gamepad[GamepadButton.RightShoulder].isPressed;
            action["R2"] = gamepad[GamepadButton.RightStick].isPressed;
            action["R3"] = gamepad[GamepadButton.RightTrigger].isPressed;
        }
        public short ProcessInputState(uint port, uint device, uint index, uint id)
        {
            //currentAction = $"port: {port} device: {device} index: {index} id: {id}";

            switch(device)
            {
                case 1: //retro device joypad
                    return ProcessJoyPad(id);
                case 5: //retro device analog
                    return ProcessAnalogic(index, id);
                default:
                    return 0;

            }
        }

        short ProcessJoyPad(uint id)
        {
                switch (id) {
                case 0:
                    return action.ContainsKey("Cross") && action["Cross"] ? (short) 1 : (short) 0; // B
                case 1:
                    return action.ContainsKey("Square") && action["Square"] ? (short) 1 : (short) 0; // Y
                case 2:
                    return action.ContainsKey("Select") && action["Select"] ? (short) 1 : (short) 0; // SELECT
                case 3:
                    return action.ContainsKey("Start") && action["Start"] ? (short) 1 : (short) 0; // START
                case 4:
                    return action.ContainsKey("Up") && action["Up"] ? (short) 1 : (short) 0; // UP
                case 5:
                    return action.ContainsKey("Down") && action["Down"] ? (short) 1 : (short) 0; // DOWN
                case 6:
                    return action.ContainsKey("Left") && action["Left"] ? (short) 1 : (short) 0; // LEFT
                case 7:
                    return action.ContainsKey("Right") && action["Right"] ? (short) 1 : (short) 0; // RIGHT
                case 8:
                    return action.ContainsKey("Circle") && action["Circle"] ? (short) 1 : (short) 0; // A
                case 9:
                    return action.ContainsKey("Triangle") && action["Triangle"] ? (short) 1 : (short) 0; // X
                case 10:
                    return action.ContainsKey("L1") && action["L1"] ? (short) 1 : (short) 0; // L
                case 11:
                    return action.ContainsKey("R1") && action["R1"] ? (short) 1 : (short) 0; // R
                case 12:
                    return action.ContainsKey("L2") && action["L2"] ? (short)1 : (short)0; //L2?
                case 13:
                    return action.ContainsKey("R2") && action["R2"] ? (short)1 : (short)0; //R2?
                case 14:
                    return action.ContainsKey("L3") && action["L3"] ? (short)1 : (short)0; //L3? (Left stick press?)
                case 15:
                    return action.ContainsKey("R3") && action["R3"] ? (short)1 : (short)0; //R3? (Right stick press?)
                default:
                    return 0;
            }            

        }

        short ProcessAnalogic(uint index, uint id)
        {
                // * axis values in the full analog range of [-0x7fff, 0x7fff], (-32767 to 32767)
                // *although some devices may return -0x8000.
                //* Positive X axis is right.Positive Y axis is down.
                //* Buttons are returned in the range[0, 0x7fff]. (0 to 32767)
                //#define RETRO_DEVICE_INDEX_ANALOG_LEFT       0
                //#define RETRO_DEVICE_INDEX_ANALOG_RIGHT      1
                //#define RETRO_DEVICE_INDEX_ANALOG_BUTTON     2
                //#define RETRO_DEVICE_ID_ANALOG_X             0
                //#define RETRO_DEVICE_ID_ANALOG_Y             1
                switch (index)
                {
                    case 0: //analog left (stick)
                        switch (id)
                        {
                            case 0:
                                return action.ContainsKey("Left") && action["Left"] ? (short)1 : (short)0; //L analog X
                            case 1:
                                return action.ContainsKey("Up") && action["Up"] ? (short)1 : (short)0; //L analog Y
                            default: return 0;
                        }
                    case 1: //analog right (stick)
                        switch (id)
                        {
                            case 0:
                                return action.ContainsKey("Right") && action["Right"] ? (short)1 : (short)0; //R analog X
                            case 1:
                                return action.ContainsKey("Down") && action["Down"] ? (short)1 : (short)0; //R analog Y
                            default: return 0;
                        }
                    case 2: //analog button?
                        return 0;
                    default: return 0;
                }
        }

        public override string ToString()
        {
            return currentAction;
        }

        private void OnGUI() {
            GUI.Label(new Rect(Screen.width - 200, 0, 300f, 20f), ToString());
        }

    }
    
}