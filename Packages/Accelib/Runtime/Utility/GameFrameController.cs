using System;
using UnityEngine;

namespace Accelib.Utility
{
    public class GameFrameController : MonoBehaviour
    {
        [SerializeField, Range(-1, 165)] private int targetFrameRate = 60;
        
        private void Awake()
        {
            Application.targetFrameRate = targetFrameRate;
        }
    }
}