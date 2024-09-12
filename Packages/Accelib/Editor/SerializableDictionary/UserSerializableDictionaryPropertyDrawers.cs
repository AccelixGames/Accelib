using Accelib.SerializableDictionary;
using UnityEditor;

namespace Editor.SerializableDictionary
{
    [CustomPropertyDrawer(typeof(StringColorDictionary))]
    [CustomPropertyDrawer(typeof(StringStringDictionary))]
    [CustomPropertyDrawer(typeof(ObjectColorDictionary))]
    [CustomPropertyDrawer(typeof(StringColorArrayDictionary))]
    public class AnySerializableDictionaryPropertyDrawer : SerializableDictionaryPropertyDrawer {}

    [CustomPropertyDrawer(typeof(ColorArrayStorage))]
    public class AnySerializableDictionaryStoragePropertyDrawer: SerializableDictionaryStoragePropertyDrawer {}
}