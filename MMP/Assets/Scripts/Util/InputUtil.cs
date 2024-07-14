using System;
using UnityEngine;

namespace Util
{
    public static class InputUtil
    {
        private static bool useSpaceForJump = false; 

        public static void SetUseSpaceForJump(bool useForJump)
        {
            useSpaceForJump = useForJump;
        }
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
            return Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetAxisRaw("Vertical") > 0 || (useSpaceForJump && Input.GetKeyDown(KeyCode.Space));
        }

        public static bool Down()
        {
            return Input.GetKeyDown(KeyCode.S) || Input.GetAxisRaw("Vertical") < 0 || Input.GetKey(KeyCode.DownArrow);
        }

        public static float HorizontalInput() {
            if(Right()) return 1f;
            if (Left()) return -1f;
            return 0f;
        }

        public static bool Portal()
        {
            return Down() || (!useSpaceForJump && Input.GetKeyDown(KeyCode.Space));
        }
    }
}
