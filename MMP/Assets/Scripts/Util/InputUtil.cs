using System;
using UnityEngine;

namespace Util
{
    public static class InputUtil
    {
        public static bool Left()
        {
            return Input.GetAxisRaw("Horizontal") < 0 || Input.GetKey(KeyCode.LeftArrow);
        }

        public static bool Right()
        {
            return Input.GetAxisRaw("Horizontal") > 0 || Input.GetKey(KeyCode.RightArrow);
        }

        public static bool Up()
        {
            return Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetAxisRaw("Vertical") > 0;
        }

        public static bool Down()
        {
            return Input.GetAxisRaw("Vertical") < 0 || Input.GetKey(KeyCode.DownArrow);
        }

        public static float HorizontalInput() {
            if(Right()) return 1f;
            if (Left()) return -1f;
            return 0f;
        }

        public static bool Portal()
        {
            return Input.GetKeyDown(KeyCode.Space);
        }
    }
}
