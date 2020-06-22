using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Nito.AsyncEx;
using UnityAsync;
using System.Threading;

namespace Utility
{
    public static class Vector
    {
        public static Vector3 FlipY(Vector3 vector)
        {
            Vector3 FlippedVector = new Vector3(vector.x, Screen.height - vector.y, vector.z);
            return FlippedVector;
        }
        public static float FlipY(float TargetFloat)
        {
            float FlippedFloat = Screen.height - TargetFloat;
            return FlippedFloat;
        }

        public static Vector3 ScreenToCanvasSpace(Vector3 ScreenInput, RectTransform Canvas)
        {
            Vector3 CanvasSize = Canvas.rect.size;
            Vector3 ModifiedMousePosition = new Vector3(
                (ScreenInput.x / Screen.width) * CanvasSize.x,
                (ScreenInput.y / Screen.height) * -CanvasSize.y,
                ScreenInput.z
            );
            return ModifiedMousePosition;
        }
        public static Vector2 ScreenToCanvasSpace(Vector2 ScreenInput, RectTransform Canvas)
        {
            Vector3 CanvasSize = Canvas.rect.size;
            Vector3 ModifiedPosition = new Vector3(
                (ScreenInput.x / Screen.width) * CanvasSize.x,
                (ScreenInput.y / Screen.height) * -CanvasSize.y
            );
            return ModifiedPosition;
        }

        public static Vector3 SubtractVector2FromVector3(Vector3 BaseVector, Vector2 SubractVector)
        {
            Vector3 Result = new Vector3(BaseVector.x - SubractVector.x, BaseVector.y - SubractVector.y, BaseVector.z);
            return Result;
        }
        public static Vector3 AddVector2ToVector3(Vector3 BaseVector, Vector2 AddVector)
        {
            Vector3 Result = new Vector3(BaseVector.x + AddVector.x, BaseVector.y + AddVector.y, BaseVector.z);
            return Result;
        }
        public static Equipment.VectorResults RearrangeVector(Vector2 InputVector, out Vector2 ResultVector)
        {
            ResultVector = new Vector2
            {
                x = Mathf.Max(InputVector.x, InputVector.y),
                y = Mathf.Min(InputVector.x, InputVector.y)
            };

            if (ResultVector != InputVector)
            {
                return Equipment.VectorResults.y;
            }
            else
            {
                return Equipment.VectorResults.x;
            }
        }
    } 
}
