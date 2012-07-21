﻿/* Copyright 2010-2012 10gen Inc.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson.IO;

namespace MongoDB.Bson.Serialization
{
    /// <summary>
    /// Represents the information needed to serialize a member.
    /// </summary>
    public class BsonSerializationInfo
    {
        // private fields
        private string _elementName;
        private IBsonSerializer _serializer;
        private Type _nominalType;
        private IBsonSerializationOptions _serializationOptions;

        // constructors
        /// <summary>
        /// Initializes a new instance of the BsonSerializationInfo class.
        /// </summary>
        /// <param name="elementName">The element name.</param>
        /// <param name="serializer">The serializer.</param>
        /// <param name="nominalType">The nominal type.</param>
        /// <param name="serializationOptions">The serialization options.</param>
        public BsonSerializationInfo(string elementName, IBsonSerializer serializer, Type nominalType, IBsonSerializationOptions serializationOptions)
        {
            _elementName = elementName;
            _serializer = serializer;
            _nominalType = nominalType;
            _serializationOptions = serializationOptions;
        }

        // public properties
        /// <summary>
        /// Gets or sets the dotted element name.
        /// </summary>
        public string ElementName
        {
            get { return _elementName; }
        }

        /// <summary>
        /// Gets or sets the serializer.
        /// </summary>
        public IBsonSerializer Serializer
        {
            get { return _serializer; }
        }

        /// <summary>
        /// Gets or sets the nominal type.
        /// </summary>
        public Type NominalType
        {
            get { return _nominalType; }
        }

        /// <summary>
        /// Gets or sets the serialization options.
        /// </summary>
        public IBsonSerializationOptions SerializationOptions
        {
            get { return _serializationOptions; }
        }

        /// <summary>
        /// Deserializes the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public object DeserializeValue(BsonValue value)
        {
            var tempDocument = new BsonDocument();
            tempDocument.Add("temp", value);
            using (var buffer = new BsonBuffer())
            {
                tempDocument.WriteTo(buffer);
                buffer.Position = 0;
                using (var reader = BsonReader.Create(buffer))
                {
                    reader.ReadStartDocument();
                    reader.ReadName("temp");
                    return _serializer.Deserialize(reader, _nominalType, _serializationOptions);
                }
            }
        }

        /// <summary>
        /// Serializes the value.
        /// </summary>
        /// <param name="value">The value.</param>
        public BsonValue SerializeValue(object value)
        {
            var tempDocument = new BsonDocument();
            using (var bsonWriter = BsonWriter.Create(tempDocument))
            {
                bsonWriter.WriteStartDocument();
                bsonWriter.WriteName("value");
                Serialize(bsonWriter, value);
                bsonWriter.WriteEndDocument();
                return tempDocument[0];
            }
        }

        // private methods
        private void Serialize(BsonWriter bsonWriter, object value)
        {
            var serializer = _serializer;
            var actualType = (value == null) ? _nominalType : value.GetType();
            if (actualType != _nominalType)
            {
                serializer = BsonSerializer.LookupSerializer(actualType);
            }
            serializer.Serialize(bsonWriter, _nominalType, value, _serializationOptions);
        }
    }
}
