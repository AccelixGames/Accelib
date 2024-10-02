using System;
using Accelib.Data;
using UnityEngine;

namespace Accelib.SerializableDictionary
{
    [Serializable]
    public class StringStringDictionary : SerializableDictionary<string, string> {}
    
    [Serializable]
    public class StringColorDictionary : SerializableDictionary<string, Color> {}
    
    [Serializable]
    public class StringColorGroupDictionary : SerializableDictionary<string, ColorGroup> {}
    
    [Serializable]
    public class StringMaterialGroupDictionary : SerializableDictionary<string, Material> {}

    [Serializable]
    public class ObjectColorDictionary : SerializableDictionary<UnityEngine.Object, Color> {}

    [Serializable]
    public class ColorArrayStorage : SerializableDictionary.Storage<Color[]> {}

    [Serializable]
    public class StringColorArrayDictionary : SerializableDictionary<string, Color[], ColorArrayStorage> {}
}