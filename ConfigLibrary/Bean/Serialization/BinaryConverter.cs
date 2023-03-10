using System;
using System.Collections.Generic;
using System.Reflection;

namespace ConfigLibrary.Bean
{
    //This class is used to serialize/deserialize bean objects into special format
    //Serialize deserialize public properties and fields
    internal static class BinaryConverter
    {
        //Register available converters
        static BinaryConverter()
        {
            //Register serializers
            _registeredSerializers.Add(typeof(int), SerializeIntValue);
            _registeredSerializers.Add(typeof(long), SerializeLongValue);
            _registeredSerializers.Add(typeof(double), SerializeDoubleValue);
            _registeredSerializers.Add(typeof(DateTime), (dateObject) => 
            {
                DateTime date = (DateTime)dateObject;
                return SerializeLongValue(date.ToBinary());
            });
            //Register deserializers
            _registeredDeserializers.Add(typeof(int), DeserializeIntValue);
            _registeredDeserializers.Add(typeof(long), DeserializeLongValue);
            _registeredDeserializers.Add(typeof(double), DeserializeDoubleValue);
            _registeredDeserializers.Add(typeof(DateTime), (binaryList) =>
            {
                return DateTime.FromBinary((long)DeserializeLongValue(binaryList));
            });
        }


        //Serialize all public fields and properties of some type
        internal static List<byte> SerializeData<T>(T data)
        {
            List<byte> binaryData = new List<byte>();
            Type dataType = typeof(T);
            Func<object, List<byte>> serializeFunction;
            //Serialize fields
            foreach (var publicField in dataType.GetFields())
            {
                if (publicField.FieldType.IsDefined(typeof(NonSerializableAttribute)))
                {
                    continue;
                }
                if (!_registeredSerializers.TryGetValue(publicField.FieldType, out serializeFunction))
                {
                    throw new Exception("Type" + publicField.FieldType.ToString() + "can not be serialized");
                }
                binaryData.AddRange(serializeFunction(publicField.GetValue(data)));
            }
            //Serialize properties
            foreach (var publicProperty in dataType.GetProperties())
            {
                if (publicProperty.CanRead && !publicProperty.PropertyType.IsDefined(typeof(NonSerializableAttribute)))
                {
                    if (!_registeredSerializers.TryGetValue(publicProperty.PropertyType, out serializeFunction))
                    {
                        throw new Exception("Type" + publicProperty.PropertyType.ToString() + "can not be serialized");
                    }
                    binaryData.AddRange(serializeFunction(publicProperty.GetValue(data)));
                }
            }
            return binaryData;
        }


        //Deserialize all public fields and properties of some type
        internal static void DeserializeData<T>(List<byte> binaryList, T dataObject) where T: class
        {
            Type dataType = typeof(T);
            Func<List<byte>, object> deserializeFunction;
            //Serialize fields
            foreach (var publicField in dataType.GetFields())
            {
                if (publicField.FieldType.IsDefined(typeof(NonSerializableAttribute)))
                {
                    continue;
                }
                if (!_registeredDeserializers.TryGetValue(publicField.FieldType, out deserializeFunction))
                {
                    throw new Exception("Type" + publicField.FieldType.ToString() + "can not be deserialized");
                }
                publicField.SetValue(dataObject, deserializeFunction(binaryList));
            }
            //Serialize properties
            foreach (var publicProperty in dataType.GetProperties())
            {
                if (publicProperty.CanWrite && !publicProperty.PropertyType.IsDefined(typeof(NonSerializableAttribute)))
                {
                    if (!_registeredDeserializers.TryGetValue(publicProperty.PropertyType, out deserializeFunction))
                    {
                        throw new Exception("Type" + publicProperty.PropertyType.ToString() + "can not be deserialized");
                    }
                    publicProperty.SetValue(dataObject, deserializeFunction(binaryList));
                }
            }
        }


        //Serialize long value
        internal static List<byte> SerializeLongValue(object data)
        {
            long dataValue = (long)data;
            List<byte> dataArray = new List<byte>(BitConverter.GetBytes(dataValue));
            if (!BitConverter.IsLittleEndian)
            {
                dataArray.Reverse();
            }
            return dataArray;
        }


        //Deserialize long value
        //Remove data from the list (ref pointer)
        internal static object DeserializeLongValue(List<byte> binaryData)
        {
            List<byte> dataArray = binaryData.GetRange(0, sizeof(long));
            if (!BitConverter.IsLittleEndian)
            {
                dataArray.Reverse();
            }
            binaryData.RemoveRange(0, sizeof(long));
            return BitConverter.ToInt64(dataArray.ToArray(), 0);
        }


        //Serialize int value
        internal static List<byte> SerializeIntValue(object data)
        {
            int dataValue = (int)data;
            List<byte> dataArray = new List<byte>(BitConverter.GetBytes(dataValue));
            if (!BitConverter.IsLittleEndian)
            {
                dataArray.Reverse();
            }
            return dataArray;
        }


        //Deserialize int value
        //Remove data from the list (ref pointer)
        internal static object DeserializeIntValue(List<byte> binaryData)
        {
            List<byte> dataArray = binaryData.GetRange(0, sizeof(int));
            if (!BitConverter.IsLittleEndian)
            {
                dataArray.Reverse();
            }
            binaryData.RemoveRange(0, sizeof(int));
            return BitConverter.ToInt32(dataArray.ToArray(), 0);
        }


        //Serialize int value
        internal static List<byte> SerializeDoubleValue(object data)
        {
            double dataValue = (double)data;
            List<byte> dataArray = new List<byte>(BitConverter.GetBytes(dataValue));
            if (!BitConverter.IsLittleEndian)
            {
                dataArray.Reverse();
            }
            return dataArray;
        }


        //Deserialize double value
        //Remove data from the list (ref pointer)
        internal static object DeserializeDoubleValue(List<byte> binaryData)
        {
            List<byte> dataArray = binaryData.GetRange(0, sizeof(double));
            if (!BitConverter.IsLittleEndian)
            {
                dataArray.Reverse();
            }
            binaryData.RemoveRange(0, sizeof(double));
            return BitConverter.ToDouble(dataArray.ToArray(), 0);
        }



        private static Dictionary<Type, Func<object, List<byte>>> _registeredSerializers = new Dictionary<Type, Func<object, List<byte>>>();

        private static Dictionary<Type, Func<List<byte>, object>> _registeredDeserializers = 
            new Dictionary<Type, Func<List<byte>, object>>();
    }
}
