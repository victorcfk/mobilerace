using FullSerializer;
using FullSerializer.Internal;
using System;
using UnityObject = UnityEngine.Object;

namespace FullInspector.Serializers.FullSerializer {
    /// <summary>
    /// Converts all types that derive from UnityObject.
    /// </summary>
    public class UnityObjectConverter : fsConverter {
        public override bool CanProcess(Type type) {
            return typeof(UnityObject).Resolve().IsAssignableFrom(type.Resolve());
        }

        public override bool RequestCycleSupport(Type storageType) {
            return false;
        }

        public override bool RequestInheritanceSupport(Type storageType) {
            return false;
        }

        public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType) {
            var obj = (UnityObject)instance;
            var serializationOperator = Serializer.Context.Get<ISerializationOperator>();

            int id = serializationOperator.StoreObjectReference(obj);
            return Serializer.TrySerialize<int>(id, out serialized);
        }

        public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType) {
            var serializationOperator = Serializer.Context.Get<ISerializationOperator>();

            int id = default(int);
            var fail = Serializer.TryDeserialize(data, ref id);
            if (fail.Failed) return fail;

            instance = serializationOperator.RetrieveObjectReference(id);
            return fsResult.Success;
        }

        public override object CreateInstance(fsData data, Type storageType) {
            return storageType;
        }
    }
}