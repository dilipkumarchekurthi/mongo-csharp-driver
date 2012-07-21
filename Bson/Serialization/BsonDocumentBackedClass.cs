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
using MongoDB.Bson.IO;

namespace MongoDB.Bson.Serialization
{
    /// <summary>
    /// A class backed by a BsonDocument.
    /// </summary>
    public abstract class BsonDocumentBackedClass
    {
        // private fields
        private readonly BsonDocument _backingDocument;
        private readonly IBsonDocumentSerializer _serializer;

        // constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="BsonDocumentBackedClass"/> class.
        /// </summary>
        /// <param name="serializer">The serializer.</param>
        protected BsonDocumentBackedClass(IBsonDocumentSerializer serializer)
            : this(new BsonDocument(), serializer)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BsonDocumentBackedClass"/> class.
        /// </summary>
        /// <param name="backingDocument">The backing document.</param>
        /// <param name="serializer">The serializer.</param>
        protected BsonDocumentBackedClass(BsonDocument backingDocument, IBsonDocumentSerializer serializer)
        {
            if (backingDocument == null)
            {
                throw new ArgumentNullException("backingDocument");
            }
            if (serializer == null)
            {
                throw new ArgumentNullException("serializer");
            }

            _backingDocument = backingDocument;
            _serializer = serializer;
        }

        // protected internal properties
        /// <summary>
        /// Gets the backing document.
        /// </summary>
        protected internal BsonDocument BackingDocument
        {
            get { return _backingDocument; }
        }

        // protected methods
        /// <summary>
        /// Gets the value from the backing document.
        /// </summary>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        protected T GetValue<T>(string memberName, T defaultValue)
        {
            var info = _serializer.GetMemberSerializationInfo(memberName);

            BsonValue bsonValue;
            if (!_backingDocument.TryGetValue(info.ElementName, out bsonValue))
            {
                return defaultValue;
            }

            return (T)info.DeserializeValue(bsonValue);
        }

        /// <summary>
        /// Sets the value in the backing document.
        /// </summary>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="value">The value.</param>
        protected void SetValue(string memberName, object value)
        {
            var info = _serializer.GetMemberSerializationInfo(memberName);
            var bsonValue = info.SerializeValue(value);
            _backingDocument.Set(info.ElementName, bsonValue);
        }
    }
}