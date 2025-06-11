namespace Accelib.AccelixWeb.Module.Advertisement.Model
{
    [System.Serializable]
    public class AdsResponse
    {
        public string type;
        public string unitId;
        public string code;
        public string message;

        public AdsResponse(string type, string unitId, string code, string message)
        {
            this.type = type;
            this.unitId = unitId;
            this.code = code;
            this.message = message;
        }

        public AdsResponse() { }
    }
}