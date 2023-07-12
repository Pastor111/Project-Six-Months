using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

namespace MyInputSystem
{

    public enum GamePadButton { A, X, B, Y, Start, Menu, RB, LB, RT, LT }
    public enum GamepadNumber { Gamepad01, Gamepad02, Gamepad03, Gamepad04 }

    [System.Serializable]
    public class Axis
    {
        public KeyCode Positive;
        public KeyCode Negative;

        public float GetAxis()
        {
            float a = 0;

            if (Input.GetKey(Positive))
                a += 1f;
            if (Input.GetKey(Negative))
                a -= 1f;

            return a;

        }
    }


    public class GamePadInput
    {

        public static void Init()
        {

        }

        public static void SaveChnages()
        {

        }

        static IEnumerator Vibrate(float time, PlayerIndex index, float StartL, float StartR)
        {
            float l = StartL;
            float r = StartR;
            float StartTime = time;
            while (time >= 0 && l > 0 && r > 0)
            {


                float progress = time / StartTime;
                l = Mathf.Lerp(0, StartL, progress);
                r = Mathf.Lerp(0, StartR, progress);

                //Debug.Log($"L : {l} || R : {r} || Time : {time} || progress : {progress}");

                GamePad.SetVibration(index, l, r);


                time -= Time.deltaTime;
                yield return null;
            }

            GamePad.SetVibration(index, 0, 0);
        }

        public static void VibrateController(GamepadNumber number, MonoBehaviour handler)
        {
            if (!IsAnyControllerConnected)
                return;

            handler.StartCoroutine(Vibrate(0.5f, (PlayerIndex)number, 1f, 1f));
        }

        public static void VibrateController(GamepadNumber number, MonoBehaviour handler, float Left, float Right)
        {
            if (!IsAnyControllerConnected)
                return;


            handler.StartCoroutine(Vibrate(0.5f, (PlayerIndex)number, Left, Right));
        }


        public static bool IsControllerButtonPressed(GamePadButton button)
        {
            if (!IsAnyControllerConnected)
                return false;


            if (button == GamePadButton.A)
            {
                return Input.GetKeyDown(KeyCode.Joystick1Button0);
            }

            if (button == GamePadButton.B)
            {
                return Input.GetKeyDown(KeyCode.Joystick1Button1);
            }

            if (button == GamePadButton.X)
            {
                return Input.GetKeyDown(KeyCode.Joystick1Button2);
            }


            if (button == GamePadButton.Y)
            {
                return Input.GetKeyDown(KeyCode.Joystick1Button3);
            }


            if (button == GamePadButton.RB)
            {
                return Input.GetKeyDown(KeyCode.Joystick1Button5);
            }

            if (button == GamePadButton.LB)
            {
                return Input.GetKeyDown(KeyCode.Joystick1Button4);
            }

            if (button == GamePadButton.Start)
            {
                return Input.GetKeyDown(KeyCode.Joystick1Button7);
            }

            if (button == GamePadButton.Menu)
            {
                return Input.GetKeyDown(KeyCode.Joystick1Button6);
            }

            if (button == GamePadButton.LT)
            {
                return Input.GetKeyDown(KeyCode.Joystick1Button11);
            }

            if (button == GamePadButton.RT)
            {
                return Input.GetKeyDown(KeyCode.Joystick1Button12);
            }


            return false;
        }

        public static bool IsControllerButtonHeld(GamePadButton button)
        {

            if (!IsAnyControllerConnected)
                return false;


            if (button == GamePadButton.A)
            {
                return Input.GetKey(KeyCode.Joystick1Button0);
            }

            if (button == GamePadButton.B)
            {
                return Input.GetKey(KeyCode.Joystick1Button1);
            }

            if (button == GamePadButton.X)
            {
                return Input.GetKey(KeyCode.Joystick1Button2);
            }

            if (button == GamePadButton.Y)
            {
                return Input.GetKey(KeyCode.Joystick1Button3);
            }


            if (button == GamePadButton.RB)
            {
                return Input.GetKey(KeyCode.Joystick1Button5);
            }

            if (button == GamePadButton.LB)
            {
                return Input.GetKey(KeyCode.Joystick1Button4);
            }

            if (button == GamePadButton.Start)
            {
                return Input.GetKey(KeyCode.Joystick1Button7);
            }

            if (button == GamePadButton.Menu)
            {
                return Input.GetKey(KeyCode.Joystick1Button6);
            }

            if (button == GamePadButton.LT)
            {
                return Input.GetKey(KeyCode.Joystick1Button11);
            }

            if (button == GamePadButton.RT)
            {
                return Input.GetKey(KeyCode.Joystick1Button12);
            }


            return false;
        }

        public static bool IsControllerButtonUp(GamePadButton button)
        {
            if (!IsAnyControllerConnected)
                return false;


            if (button == GamePadButton.A)
            {
                return Input.GetKeyUp(KeyCode.Joystick1Button0);
            }

            if (button == GamePadButton.B)
            {
                return Input.GetKeyUp(KeyCode.Joystick1Button1);
            }

            if (button == GamePadButton.X)
            {
                return Input.GetKeyUp(KeyCode.Joystick1Button2);
            }

            if (button == GamePadButton.Y)
            {
                return Input.GetKeyUp(KeyCode.Joystick1Button3);
            }


            if (button == GamePadButton.RB)
            {
                return Input.GetKeyUp(KeyCode.Joystick1Button5);
            }

            if (button == GamePadButton.LB)
            {
                return Input.GetKeyUp(KeyCode.Joystick1Button4);
            }

            if (button == GamePadButton.Start)
            {
                return Input.GetKeyUp(KeyCode.Joystick1Button7);
            }

            if (button == GamePadButton.Menu)
            {
                return Input.GetKeyUp(KeyCode.Joystick1Button6);
            }

            if (button == GamePadButton.LT)
            {
                return Input.GetKeyUp(KeyCode.Joystick1Button11);
            }

            if (button == GamePadButton.RT)
            {
                return Input.GetKeyUp(KeyCode.Joystick1Button12);
            }


            return false;
        }

        public static bool IsControllerButtonPressed(GamePadButton button, GamepadNumber number)
        {
            if (!IsAnyControllerConnected)
                return false;

            if (button == GamePadButton.A)
            {
                if (number == GamepadNumber.Gamepad01)
                    return Input.GetKeyDown(KeyCode.Joystick1Button0);
                else if (number == GamepadNumber.Gamepad02)
                    return Input.GetKeyDown(KeyCode.Joystick2Button0);
                else if (number == GamepadNumber.Gamepad03)
                    return Input.GetKeyDown(KeyCode.Joystick3Button0);
                else
                    return Input.GetKeyDown(KeyCode.Joystick4Button0);
            }

            if (button == GamePadButton.B)
            {
                if (number == GamepadNumber.Gamepad01)
                    return Input.GetKeyDown(KeyCode.Joystick1Button1);
                else if (number == GamepadNumber.Gamepad02)
                    return Input.GetKeyDown(KeyCode.Joystick2Button1);
                else if (number == GamepadNumber.Gamepad03)
                    return Input.GetKeyDown(KeyCode.Joystick3Button1);
                else
                    return Input.GetKeyDown(KeyCode.Joystick4Button1);
            }

            if (button == GamePadButton.X)
            {
                if (number == GamepadNumber.Gamepad01)
                    return Input.GetKeyDown(KeyCode.Joystick1Button2);
                else if (number == GamepadNumber.Gamepad02)
                    return Input.GetKeyDown(KeyCode.Joystick2Button2);
                else if (number == GamepadNumber.Gamepad03)
                    return Input.GetKeyDown(KeyCode.Joystick3Button2);
                else
                    return Input.GetKeyDown(KeyCode.Joystick4Button2);
            }

            if (button == GamePadButton.Y)
            {
                if (number == GamepadNumber.Gamepad01)
                    return Input.GetKeyDown(KeyCode.Joystick1Button3);
                else if (number == GamepadNumber.Gamepad02)
                    return Input.GetKeyDown(KeyCode.Joystick2Button3);
                else if (number == GamepadNumber.Gamepad03)
                    return Input.GetKeyDown(KeyCode.Joystick3Button3);
                else
                    return Input.GetKeyDown(KeyCode.Joystick4Button3);
            }

            if (button == GamePadButton.RB)
            {

                if (number == GamepadNumber.Gamepad01)
                    return Input.GetKeyDown(KeyCode.Joystick1Button5);
                else if (number == GamepadNumber.Gamepad02)
                    return Input.GetKeyDown(KeyCode.Joystick2Button5);
                else if (number == GamepadNumber.Gamepad03)
                    return Input.GetKeyDown(KeyCode.Joystick3Button5);
                else
                    return Input.GetKeyDown(KeyCode.Joystick4Button5);
            }

            if (button == GamePadButton.LB)
            {
                if (number == GamepadNumber.Gamepad01)
                    return Input.GetKeyDown(KeyCode.Joystick1Button4);
                else if (number == GamepadNumber.Gamepad02)
                    return Input.GetKeyDown(KeyCode.Joystick2Button4);
                else if (number == GamepadNumber.Gamepad03)
                    return Input.GetKeyDown(KeyCode.Joystick3Button4);
                else
                    return Input.GetKeyDown(KeyCode.Joystick4Button4);
            }

            if (button == GamePadButton.Start)
            {
                if (number == GamepadNumber.Gamepad01)
                    return Input.GetKeyDown(KeyCode.Joystick1Button7);
                else if (number == GamepadNumber.Gamepad02)
                    return Input.GetKeyDown(KeyCode.Joystick2Button7);
                else if (number == GamepadNumber.Gamepad03)
                    return Input.GetKeyDown(KeyCode.Joystick3Button7);
                else
                    return Input.GetKeyDown(KeyCode.Joystick4Button7);
            }

            if (button == GamePadButton.Menu)
            {
                if (number == GamepadNumber.Gamepad01)
                    return Input.GetKeyDown(KeyCode.Joystick1Button6);
                else if (number == GamepadNumber.Gamepad02)
                    return Input.GetKeyDown(KeyCode.Joystick2Button6);
                else if (number == GamepadNumber.Gamepad03)
                    return Input.GetKeyDown(KeyCode.Joystick3Button6);
                else
                    return Input.GetKeyDown(KeyCode.Joystick4Button6);
            }

            if (button == GamePadButton.RT)
            {
                if (number == GamepadNumber.Gamepad01)
                    return Input.GetKeyDown(KeyCode.Joystick1Button12);
                else if (number == GamepadNumber.Gamepad02)
                    return Input.GetKeyDown(KeyCode.Joystick2Button12);
                else if (number == GamepadNumber.Gamepad03)
                    return Input.GetKeyDown(KeyCode.Joystick3Button12);
                else
                    return Input.GetKeyDown(KeyCode.Joystick4Button12);
            }

            if (button == GamePadButton.LT)
            {
                if (number == GamepadNumber.Gamepad01)
                    return Input.GetKeyDown(KeyCode.Joystick1Button11);
                else if (number == GamepadNumber.Gamepad02)
                    return Input.GetKeyDown(KeyCode.Joystick2Button11);
                else if (number == GamepadNumber.Gamepad03)
                    return Input.GetKeyDown(KeyCode.Joystick3Button11);
                else
                    return Input.GetKeyDown(KeyCode.Joystick4Button11);
            }


            return false;
        }

        public static bool IsControllerButtonHeld(GamePadButton button, GamepadNumber number)
        {
            if (!IsAnyControllerConnected)
                return false;


            if (button == GamePadButton.A)
            {
                if (number == GamepadNumber.Gamepad01)
                    return Input.GetKey(KeyCode.Joystick1Button0);
                else if (number == GamepadNumber.Gamepad02)
                    return Input.GetKey(KeyCode.Joystick2Button0);
                else if (number == GamepadNumber.Gamepad03)
                    return Input.GetKey(KeyCode.Joystick3Button0);
                else
                    return Input.GetKey(KeyCode.Joystick4Button0);
            }

            if (button == GamePadButton.B)
            {

                if (number == GamepadNumber.Gamepad01)
                    return Input.GetKey(KeyCode.Joystick1Button1);
                else if (number == GamepadNumber.Gamepad02)
                    return Input.GetKey(KeyCode.Joystick2Button1);
                else if (number == GamepadNumber.Gamepad03)
                    return Input.GetKey(KeyCode.Joystick3Button1);
                else
                    return Input.GetKey(KeyCode.Joystick4Button1);
            }

            if (button == GamePadButton.X)
            {
                if (number == GamepadNumber.Gamepad01)
                    return Input.GetKey(KeyCode.Joystick1Button2);
                else if (number == GamepadNumber.Gamepad02)
                    return Input.GetKey(KeyCode.Joystick2Button2);
                else if (number == GamepadNumber.Gamepad03)
                    return Input.GetKey(KeyCode.Joystick3Button2);
                else
                    return Input.GetKey(KeyCode.Joystick4Button2);
            }

            if (button == GamePadButton.Y)
            {
                if (number == GamepadNumber.Gamepad01)
                    return Input.GetKey(KeyCode.Joystick1Button3);
                else if (number == GamepadNumber.Gamepad02)
                    return Input.GetKey(KeyCode.Joystick2Button3);
                else if (number == GamepadNumber.Gamepad03)
                    return Input.GetKey(KeyCode.Joystick3Button3);
                else
                    return Input.GetKey(KeyCode.Joystick4Button3);
            }

            if (button == GamePadButton.RB)
            {
                if (number == GamepadNumber.Gamepad01)
                    return Input.GetKey(KeyCode.Joystick1Button5);
                else if (number == GamepadNumber.Gamepad02)
                    return Input.GetKey(KeyCode.Joystick2Button5);
                else if (number == GamepadNumber.Gamepad03)
                    return Input.GetKey(KeyCode.Joystick3Button5);
                else
                    return Input.GetKey(KeyCode.Joystick4Button5);
            }

            if (button == GamePadButton.LB)
            {
                if (number == GamepadNumber.Gamepad01)
                    return Input.GetKey(KeyCode.Joystick1Button4);
                else if (number == GamepadNumber.Gamepad02)
                    return Input.GetKey(KeyCode.Joystick2Button4);
                else if (number == GamepadNumber.Gamepad03)
                    return Input.GetKey(KeyCode.Joystick3Button4);
                else
                    return Input.GetKey(KeyCode.Joystick4Button4);
            }

            if (button == GamePadButton.Start)
            {
                if (number == GamepadNumber.Gamepad01)
                    return Input.GetKey(KeyCode.Joystick1Button7);
                else if (number == GamepadNumber.Gamepad02)
                    return Input.GetKey(KeyCode.Joystick2Button7);
                else if (number == GamepadNumber.Gamepad03)
                    return Input.GetKey(KeyCode.Joystick3Button7);
                else
                    return Input.GetKey(KeyCode.Joystick4Button7);
            }

            if (button == GamePadButton.Menu)
            {
                if (number == GamepadNumber.Gamepad01)
                    return Input.GetKey(KeyCode.Joystick1Button6);
                else if (number == GamepadNumber.Gamepad02)
                    return Input.GetKey(KeyCode.Joystick2Button6);
                else if (number == GamepadNumber.Gamepad03)
                    return Input.GetKey(KeyCode.Joystick3Button6);
                else
                    return Input.GetKey(KeyCode.Joystick4Button6);
            }

            if (button == GamePadButton.RT)
            {
                if (number == GamepadNumber.Gamepad01)
                    return Input.GetKey(KeyCode.Joystick1Button12);
                else if (number == GamepadNumber.Gamepad02)
                    return Input.GetKey(KeyCode.Joystick2Button12);
                else if (number == GamepadNumber.Gamepad03)
                    return Input.GetKey(KeyCode.Joystick3Button12);
                else
                    return Input.GetKey(KeyCode.Joystick4Button12);
            }

            if (button == GamePadButton.LT)
            {
                if (number == GamepadNumber.Gamepad01)
                    return Input.GetKey(KeyCode.Joystick1Button11);
                else if (number == GamepadNumber.Gamepad02)
                    return Input.GetKey(KeyCode.Joystick2Button11);
                else if (number == GamepadNumber.Gamepad03)
                    return Input.GetKey(KeyCode.Joystick3Button11);
                else
                    return Input.GetKey(KeyCode.Joystick4Button11);
            }

            return false;
        }

        public static bool IsControllerButtonUp(GamePadButton button, GamepadNumber number)
        {
            if (button == GamePadButton.A)
            {
                if (number == GamepadNumber.Gamepad01)
                    return Input.GetKeyUp(KeyCode.Joystick1Button0);
                else if (number == GamepadNumber.Gamepad02)
                    return Input.GetKeyUp(KeyCode.Joystick2Button0);
                else if (number == GamepadNumber.Gamepad03)
                    return Input.GetKeyUp(KeyCode.Joystick3Button0);
                else
                    return Input.GetKeyUp(KeyCode.Joystick4Button0);
            }

            if (button == GamePadButton.B)
            {
                if (number == GamepadNumber.Gamepad01)
                    return Input.GetKeyUp(KeyCode.Joystick1Button1);
                else if (number == GamepadNumber.Gamepad02)
                    return Input.GetKeyUp(KeyCode.Joystick2Button1);
                else if (number == GamepadNumber.Gamepad03)
                    return Input.GetKeyUp(KeyCode.Joystick3Button1);
                else
                    return Input.GetKeyUp(KeyCode.Joystick4Button1);
            }

            if (button == GamePadButton.X)
            {
                if (number == GamepadNumber.Gamepad01)
                    return Input.GetKeyUp(KeyCode.Joystick1Button2);
                else if (number == GamepadNumber.Gamepad02)
                    return Input.GetKeyUp(KeyCode.Joystick2Button2);
                else if (number == GamepadNumber.Gamepad03)
                    return Input.GetKeyUp(KeyCode.Joystick3Button2);
                else
                    return Input.GetKeyUp(KeyCode.Joystick4Button2);
            }

            if (button == GamePadButton.Y)
            {
                if (number == GamepadNumber.Gamepad01)
                    return Input.GetKeyUp(KeyCode.Joystick1Button3);
                else if (number == GamepadNumber.Gamepad02)
                    return Input.GetKeyUp(KeyCode.Joystick2Button3);
                else if (number == GamepadNumber.Gamepad03)
                    return Input.GetKeyUp(KeyCode.Joystick3Button3);
                else
                    return Input.GetKeyUp(KeyCode.Joystick4Button3);
            }


            if (button == GamePadButton.RB)
            {
                if (number == GamepadNumber.Gamepad01)
                    return Input.GetKeyUp(KeyCode.Joystick1Button5);
                else if (number == GamepadNumber.Gamepad02)
                    return Input.GetKeyUp(KeyCode.Joystick2Button5);
                else if (number == GamepadNumber.Gamepad03)
                    return Input.GetKeyUp(KeyCode.Joystick3Button5);
                else
                    return Input.GetKeyUp(KeyCode.Joystick4Button5);
            }

            if (button == GamePadButton.LB)
            {
                ;

                if (number == GamepadNumber.Gamepad01)
                    return Input.GetKeyUp(KeyCode.Joystick1Button4);
                else if (number == GamepadNumber.Gamepad02)
                    return Input.GetKeyUp(KeyCode.Joystick2Button4);
                else if (number == GamepadNumber.Gamepad03)
                    return Input.GetKeyUp(KeyCode.Joystick3Button4);
                else
                    return Input.GetKeyUp(KeyCode.Joystick4Button4);
            }

            if (button == GamePadButton.Start)
            {

                if (number == GamepadNumber.Gamepad01)
                    return Input.GetKeyUp(KeyCode.Joystick1Button7);
                else if (number == GamepadNumber.Gamepad02)
                    return Input.GetKeyUp(KeyCode.Joystick2Button7);
                else if (number == GamepadNumber.Gamepad03)
                    return Input.GetKeyUp(KeyCode.Joystick3Button7);
                else
                    return Input.GetKeyUp(KeyCode.Joystick4Button7);
            }

            if (button == GamePadButton.Menu)
            {
                if (number == GamepadNumber.Gamepad01)
                    return Input.GetKeyUp(KeyCode.Joystick1Button6);
                else if (number == GamepadNumber.Gamepad02)
                    return Input.GetKeyUp(KeyCode.Joystick2Button6);
                else if (number == GamepadNumber.Gamepad03)
                    return Input.GetKeyUp(KeyCode.Joystick3Button6);
                else
                    return Input.GetKeyUp(KeyCode.Joystick4Button6);
            }

            if (button == GamePadButton.RT)
            {
                if (number == GamepadNumber.Gamepad01)
                    return Input.GetKeyUp(KeyCode.Joystick1Button12);
                else if (number == GamepadNumber.Gamepad02)
                    return Input.GetKeyUp(KeyCode.Joystick2Button12);
                else if (number == GamepadNumber.Gamepad03)
                    return Input.GetKeyUp(KeyCode.Joystick3Button12);
                else
                    return Input.GetKeyUp(KeyCode.Joystick4Button12);
            }

            if (button == GamePadButton.LT)
            {
                if (number == GamepadNumber.Gamepad01)
                    return Input.GetKeyUp(KeyCode.Joystick1Button11);
                else if (number == GamepadNumber.Gamepad02)
                    return Input.GetKeyUp(KeyCode.Joystick2Button11);
                else if (number == GamepadNumber.Gamepad03)
                    return Input.GetKeyUp(KeyCode.Joystick3Button11);
                else
                    return Input.GetKeyUp(KeyCode.Joystick4Button11);
            }

            return false;
        }

        public static Vector2 GetHorizontalAndVerticalAxis()
        {

            if (!IsAnyControllerConnected)
                return Vector2.zero;


            //Controller
            float x = 0;
            float y = 0;
            //if (IsAnyControllerConnected)
            //{
            //    x = Input.GetAxisRaw("Horizontal");
            //    y = Input.GetAxisRaw("Vertical");

            //}

            //#region KeyBoard

            x = GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X;
            y = GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y;

            //if (Input.GetKey(KeyCode.W))
            //    y = 1;
            //else if (Input.GetKey(KeyCode.S))
            //    y = -1;

            //if (Input.GetKey(KeyCode.D))
            //    x = 1;
            //else if (Input.GetKey(KeyCode.A))
            //    x = -1;

            return new Vector2(x, y);

        }

        public static Vector2 GetHorizontalAndVerticalAxis(GamepadNumber number)
        {
            if (!IsAnyControllerConnected)
                return Vector2.zero;


            //Controller
            float x = 0;
            float y = 0;
            //if (IsAnyControllerConnected)
            //{

            //    string horizontalAxis = "Horizontal";
            //    string verticalAxis = "Vertical";

            //    if (number == GamepadNumber.Gamepad02)
            //    {
            //        horizontalAxis += "_P2";
            //        verticalAxis += "_P2";
            //    }

            //    if (number == GamepadNumber.Gamepad03)
            //    {
            //        horizontalAxis += "_P3";
            //        verticalAxis += "_P3";
            //    }

            //    if (number == GamepadNumber.Gamepad04)
            //    {
            //        horizontalAxis += "_P4";
            //        verticalAxis += "_P4";
            //    }

            //    x = Input.GetAxisRaw(horizontalAxis);
            //    y = Input.GetAxisRaw(verticalAxis);

            //}

            //#region KeyBoard

            switch (number)
            {
                case GamepadNumber.Gamepad01:
                    x = GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X;
                    y = GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y;
                    break;
                case GamepadNumber.Gamepad02:
                    x = GamePad.GetState(PlayerIndex.Two).ThumbSticks.Left.X;
                    y = GamePad.GetState(PlayerIndex.Two).ThumbSticks.Left.Y;
                    break;
                case GamepadNumber.Gamepad03:
                    x = GamePad.GetState(PlayerIndex.Three).ThumbSticks.Left.X;
                    y = GamePad.GetState(PlayerIndex.Three).ThumbSticks.Left.Y;
                    break;
                case GamepadNumber.Gamepad04:
                    x = GamePad.GetState(PlayerIndex.Four).ThumbSticks.Left.X;
                    y = GamePad.GetState(PlayerIndex.Four).ThumbSticks.Left.Y;
                    break;
            }



            //if (Input.GetKey(KeyCode.W))
            //    y = 1;
            //else if (Input.GetKey(KeyCode.S))
            //    y = -1;

            //if (Input.GetKey(KeyCode.D))
            //    x = 1;
            //else if (Input.GetKey(KeyCode.A))
            //    x = -1;


            //#endregion


            return new Vector2(x, y);

        }

        public static Vector2 GetRightAxis(GamepadNumber number = GamepadNumber.Gamepad01)
        {

            if (!IsAnyControllerConnected)
                return Vector2.zero;


            //Controller
            float x = 0;
            float y = 0;
            if (IsAnyControllerConnected)
            {
                switch (number)
                {
                    case GamepadNumber.Gamepad01:
                        x = GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.X;
                        y = GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.Y;
                        break;
                    case GamepadNumber.Gamepad02:
                        x = GamePad.GetState(PlayerIndex.Two).ThumbSticks.Right.X;
                        y = GamePad.GetState(PlayerIndex.Two).ThumbSticks.Right.Y;
                        break;
                    case GamepadNumber.Gamepad03:
                        x = GamePad.GetState(PlayerIndex.Three).ThumbSticks.Right.X;
                        y = GamePad.GetState(PlayerIndex.Three).ThumbSticks.Right.Y;
                        break;
                    case GamepadNumber.Gamepad04:
                        x = GamePad.GetState(PlayerIndex.Four).ThumbSticks.Right.X;
                        y = GamePad.GetState(PlayerIndex.Four).ThumbSticks.Right.Y;
                        break;
                }

            }

            #region KeyBoard

            //if (Input.GetKey(KeyCode.W))
            //    y = 1;
            //else if (Input.GetKey(KeyCode.S))
            //    y = -1;

            //if (Input.GetKey(KeyCode.D))
            //    x = 1;
            //else if (Input.GetKey(KeyCode.A))
            //    x = -1;


            #endregion


            return new Vector2(x, y);

        }

        public static float GetLeftTrigger(GamepadNumber number = GamepadNumber.Gamepad01)
        {
            //Controller

            if (IsAnyControllerConnected)
            {
                switch (number)
                {
                    case GamepadNumber.Gamepad01:
                        return GamePad.GetState(PlayerIndex.One).Triggers.Left;
                    case GamepadNumber.Gamepad02:
                        return GamePad.GetState(PlayerIndex.Two).Triggers.Left;
                    case GamepadNumber.Gamepad03:
                        return GamePad.GetState(PlayerIndex.Three).Triggers.Left;
                    case GamepadNumber.Gamepad04:
                        return GamePad.GetState(PlayerIndex.Four).Triggers.Left;
                }


            }

            return 0;

        }

        public static float GetRightTrigger(GamepadNumber number = GamepadNumber.Gamepad01)
        {
            //Controller

            if (IsAnyControllerConnected)
            {

                switch (number)
                {
                    case GamepadNumber.Gamepad01:
                        return GamePad.GetState(PlayerIndex.One).Triggers.Right;
                    case GamepadNumber.Gamepad02:
                        return GamePad.GetState(PlayerIndex.Two).Triggers.Right;
                    case GamepadNumber.Gamepad03:
                        return GamePad.GetState(PlayerIndex.Three).Triggers.Right;
                    case GamepadNumber.Gamepad04:
                        return GamePad.GetState(PlayerIndex.Four).Triggers.Right;
                }


            }

            return 0;

        }


        public static bool IsAnyControllerConnected
        {
            get
            {
                for (int i = 0; i < 4; i++)
                {
                        if (!GamePad.GetState((PlayerIndex)i).IsConnected)
                            continue;
                        else
                            return true;
                }

                return false;
            }
        }

        //public static int NumberOfConnectedControllers
        //{
        //    get
        //    {
        //        return Input.GetJoystickNames().Length;
        //    }
        //}
    }


    public class KeyBoardInput
    {
        public static float GetAxis(KeyCode positive, KeyCode negative)
        {
            if (Input.GetKey(positive))
                return 1f;
            else if (Input.GetKey(negative))
                return -1f;

            return 0f;
        }
    }
}
