using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Accelib.Module.AccelNovel.Control.Action
{
    public abstract class ActionControllerT<T> : ActionController where T: class
    {
        public override void FromJson(string json)
        {
            try
            {
                var data = JsonConvert.DeserializeObject<T>(json);
                if(data != null)
                    Internal_FromJson(data);
            }
            catch (Exception e)
            {
                Debug.LogException(e, this);
            }
        }

        public override string ToJson()
        {
            try
            {
                var saveData = Internal_GetData();
                if (saveData != null)
                {
                    var json = JsonConvert.SerializeObject(saveData);
                    return json;
                }

                return null;
            }
            catch (Exception e)
            {
                Debug.LogException(e, this);
                return null;
            }
        }

        protected abstract void Internal_FromJson(T data);
        protected abstract T Internal_GetData();
    }
}