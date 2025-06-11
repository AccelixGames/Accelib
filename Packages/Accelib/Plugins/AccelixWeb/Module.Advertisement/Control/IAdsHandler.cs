using UnityEngine;

namespace Accelib.AccelixWeb.Module.Advertisement.Control
{
    public interface IAdsHandler
    {
        public void OnLoad(bool success);
        public void OnEvent(string code);
        public void OnShow(bool success);

        public GameObject GetInstance() => null;
    }
}