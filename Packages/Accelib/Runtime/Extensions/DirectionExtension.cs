using System;
using Accelib.Data;
using UnityEngine;

namespace Accelib.Extensions
{
    public static class DirectionExtension
    {
        private static readonly Vector2 RefVec = new(-1, 0);

        public static Direction Abs(this Direction dir) => (dir.ToVector() * -Vector2Int.one).ToDirection();
        
        public static float ToAngle(this Direction dir) => Vector2.SignedAngle(RefVec, dir.ToVector());

        public static Vector2Int ToVector(this Direction dir) => dir switch
            {
                Direction.Left => Vector2Int.left,
                Direction.Right => Vector2Int.right,
                Direction.Up => Vector2Int.up,
                Direction.Down => Vector2Int.down,
                _ => Vector2Int.zero
            };
        
        public static Direction ToDirection(this Vector2Int vector) => Vector2.SignedAngle(RefVec, vector) switch
        {
            0 => Direction.None,
            >= -45f and <= 45f => Direction.Left,
            >= 45f and <= 135f => Direction.Down,
            >= 135f or <= -135f => Direction.Right,
            _ => Direction.Up
        };

        public static Direction ToDirection(this Vector2 vector) => Vector2.SignedAngle(RefVec, vector) switch
            {
                0 => Direction.None,
                >= -45f and <= 45f => Direction.Left,
                >= 45f and <= 135f => Direction.Down,
                >= 135f or <= -135f => Direction.Right,
                _ => Direction.Up
            };
    }
}