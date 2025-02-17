using Accelib.Module.UI.InfoBox.Base.Model;

namespace Accelib.Module.UI.InfoBox.Base.View
{
    public abstract class InfoBoxViewBaseT<T> : _InfoBoxViewBase where T : InfoDataBase
    {
        protected internal override void DrawInfo(InfoDataBase info)
        {
            if(this == null || gameObject == null) return;
            
            if(info == null)
                SetActive(false);
            else
            {
                SetActive(true);
                OnDrawInfo((T)info);   
            }
        }

        protected virtual void SetActive(bool enable)
        {
            gameObject.SetActive(enable);
        }

        protected abstract void OnDrawInfo(T info);
    }
}