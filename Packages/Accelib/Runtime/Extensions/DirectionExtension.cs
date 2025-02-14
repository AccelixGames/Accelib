using System;
using Accelib.Data;
using UnityEngine;

namespace Accelib.Extensions
{
    public static class DirectionExtension
    {
        public static readonly Vector2 RefVec = new(-1, 0);

        public static Direction Abs(this Direction dir) => (dir.ToVec2Int() * -Vector2Int.one).ToDirection();
        
        public static float ToAngle(this Vector2 dir) => Vector2.SignedAngle(RefVec, dir);
        public static float ToAngle(this Direction dir) => Vector2.SignedAngle(RefVec, dir.ToVec2Int());
        
        public static Vector2 RotateRadian(this Vector2 v, float radians)
        {
            var cos = Mathf.Cos(radians);
            var sin = Mathf.Sin(radians);

            return new Vector2(
                cos * v.x - sin * v.y,
                sin * v.x + cos * v.y
            );
        }

        public static Quaternion ToQuaternion(this Vector2 vec) => Quaternion.Euler(0f, 0f, Vector2.SignedAngle(RefVec, vec));

        public static Vector2Int ToVec2Int(this Direction dir) => dir switch
            {
                Direction.Left => Vector2Int.left,
                Direction.Right => Vector2Int.right,
                Direction.Up => Vector2Int.up,
                Direction.Down => Vector2Int.down,
                _ => Vector2Int.zero
            };
        
        public static Direction ToDirection(this Vector2Int vector) => Vector2.SignedAngle(RefVec, vector) switch
        {
            >= -45f and <= 45f => Direction.Left,
            >= 45f and <= 135f => Direction.Down,
            >= 135f or <= -135f => Direction.Right,
            _ => Direction.Up
        };

        public static Direction ToDirection(this Vector2 vector) => Vector2.SignedAngle(RefVec, vector) switch
            {
                >= -45f and <= 45f => Direction.Left,
                >= 45f and <= 135f => Direction.Down,
                >= 135f or <= -135f => Direction.Right,
                _ => Direction.Up
            };
    }
}