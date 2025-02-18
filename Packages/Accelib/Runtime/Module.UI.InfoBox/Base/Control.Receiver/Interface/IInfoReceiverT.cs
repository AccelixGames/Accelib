using Accelib.Module.UI.InfoBox.Base.Model;

namespace Accelib.Module.UI.InfoBox.Base.Control.Receiver.Interface
{
    public interface IInfoReceiverT<in T> : _IInfoReceiver where T : InfoDataBase
    {
        void _IInfoReceiver.Receive(InfoDataBase info) => ReceiveInfo((T)info);

        public void ReceiveInfo(T info);
    }
}