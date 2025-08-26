namespace Accelib.Module.AccelNovel.Control.Action
{
    public abstract class ActionController_Default : ActionControllerT<ActionController_Default.EmptyData>
    {
        [System.Serializable]
        public class EmptyData { }

        protected override void Internal_FromJson(EmptyData data) { }
        protected override EmptyData Internal_GetData() => null;
    }
}