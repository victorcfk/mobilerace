#if !UNITY_4_3
using System;
using FullSerializer;
using FullSerializer.Internal;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace FullInspector.Serializers.FullSerializer {
    public class SerializationCallbackReceiverObjectProcessor : fsObjectProcessor {
        public override bool CanProcess(Type type) {
            return
                typeof(UnityObject).Resolve().IsAssignableFrom(type.Resolve()) == false &&
                typeof(ISerializationCallbackReceiver).Resolve().IsAssignableFrom(type.Resolve());
        }

        public override void OnBeforeSerialize(Type storageType, object instance) {
            var obj = (ISerializationCallbackReceiver)instance;
            if (obj != null) obj.OnBeforeSerialize();
        }

        public override void OnAfterSerialize(Type storageType, object instance, ref fsData data) {
        }

        public override void OnBeforeDeserialize(Type storageType, ref fsData data) {
        }

        public override void OnAfterDeserialize(Type storageType, object instance) {
            var obj = (ISerializationCallbackReceiver)instance;
            if (obj != null) obj.OnAfterDeserialize();
        }
    }
}
#endif