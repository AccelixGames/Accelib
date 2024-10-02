using Accelib.SerializableDictionary;
using UnityEditor;

namespace Accelib.Editor.SerializableDictionary
{
    [CustomPropertyDrawer(typeof(StringColorDictionary))]
    [CustomPropertyDrawer(typeof(StringStringDictionary))]
    [CustomPropertyDrawer(typeof(ObjectColorDictionary))]
    [CustomPropertyDrawer(typeof(StringColorArrayDictionary))]
    [CustomPropertyDrawer(typeof(StringColorGroupDictionary))]
    [CustomPropertyDrawer(typeof(StringMaterialGroupDictionary))]
    public class AnySerializableDictionaryPropertyDrawer : SerializableDictionaryPropertyDrawer {}

    [CustomPropertyDrawer(typeof(ColorArrayStorage))]
    public class AnySerializableDictionaryStoragePropertyDrawer: SerializableDictionaryStoragePropertyDrawer {}
}