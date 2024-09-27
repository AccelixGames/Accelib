using Accelib.SerializableDictionary;
using UnityEditor;

namespace Accelib.Editor.SerializableDictionary
{
    [CustomPropertyDrawer(typeof(StringColorDictionary))]
    [CustomPropertyDrawer(typeof(StringStringDictionary))]
    [CustomPropertyDrawer(typeof(ObjectColorDictionary))]
    [CustomPropertyDrawer(typeof(StringColorArrayDictionary))]
    [CustomPropertyDrawer(typeof(StringColorGroupDictionary))]
    public class AnySerializableDictionaryPropertyDrawer : SerializableDictionaryPropertyDrawer {}

    [CustomPropertyDrawer(typeof(ColorArrayStorage))]
    public class AnySerializableDictionaryStoragePropertyDrawer: SerializableDictionaryStoragePropertyDrawer {}
}