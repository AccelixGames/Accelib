namespace Accelib.Module.AccelNovel.Model
{
    [System.Serializable]
    public class ActionResponse
    {
        public ActionResponse(bool waitForInput)
        {
            this.waitForInput = waitForInput;
        }

        public bool waitForInput;
    }
}