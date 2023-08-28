using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Amatib
{
    public class GetAngleComponent
    {
        public enum DirectionType
        {
            None,
            Up,
            Left,
            Down,
            Right
        }

        public static DirectionType GetDirectionType(Vector3 inputStart, Vector3 inputEnd)
        {
            float dx = inputEnd.x - inputStart.x;
            float dy = inputEnd.y - inputStart.y;
            float rad = Mathf.Atan2(dy, dx);

            return GetDirection( rad * Mathf.Rad2Deg);
        }

        private static DirectionType GetDirection(float angle)
        {
            DirectionType directionType = DirectionType.None;

            var normalizedAngle = Mathf.Repeat(angle, 360);

            if (45f <= normalizedAngle && normalizedAngle < 135f)
            {
                directionType = DirectionType.Up;
            }
            else if (135f <= normalizedAngle && normalizedAngle < 225f)
            {
                directionType = DirectionType.Left;
            }
            else if (225f <= normalizedAngle && normalizedAngle < 315f)
            {
                directionType = DirectionType.Down;
            }
            else if ((315f <= normalizedAngle && normalizedAngle <= 360f) || (0f <= normalizedAngle && normalizedAngle < 45f))
            {
                directionType = DirectionType.Right;
            }

            return directionType;
        }
    }
}
