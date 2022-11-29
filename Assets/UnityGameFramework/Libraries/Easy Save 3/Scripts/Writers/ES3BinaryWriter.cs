using System.Collections.Generic;
using System.Collections;
using System.IO;
using System;
using UnityEngine;
using System.Text;
using System.Globalization;
using ES3Types;

namespace ES3Internal
{
	internal class ES3BinaryWriter : ES3Writer
	{
		internal BinaryWriter baseWriter;

		public ES3BinaryWriter(Stream stream, ES3Settings settings) : this(stream, settings, true, true){}

		internal ES3BinaryWriter(Stream stream, ES3Settings settings, bool writeHeaderAndFooter, bool mergeKeys) : base(settings, writeHeaderAndFooter, mergeKeys)
		{
			baseWriter = new BinaryWriter(stream, settings.encoding);
			StartWriteFile();
		}

        internal override void Write(string key, Type type, byte[] value)
        {

            StartWriteProperty(key);
            using (var ms = new MemoryStream())
            {
                using (var writer = ES3Writer.Create(ms, new ES3Settings(ES3.EncryptionType.None, ES3.CompressionType.None, ES3.Format.Binary_Alpha), false, false))
                {
                    writer.StartWriteObject(key);
                    writer.WriteType(type);
                    writer.WriteRawProperty("value", value);
                    writer.EndWriteObject(key);
                }
                var bytes = ms.ToArray();
                Write7BitEncodedInt(bytes.Length);
                baseWriter.Write(bytes);
            }

            EndWriteProperty(key);
        }

        /// <summary>Writes a value to the writer with the given key, using the given type rather than the generic parameter.</summary>
        /// <param name="key">The key which uniquely identifies this value.</param>
        /// <param name="value">The value we want to write.</param>
        /// <param name="type">The type we want to use for the header, and to retrieve an ES3Type.</param>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public override void Write(Type type, string key, object value)
        {
            StartWriteProperty(key);

            using (var ms = new MemoryStream())
            {
                using (var writer = ES3Writer.Create(ms, new ES3Settings(ES3.EncryptionType.None, ES3.CompressionType.None, ES3.Format.Binary_Alpha), false, false))
                {
                    writer.StartWriteObject(key);
                    writer.WriteType(type);
                    writer.WriteProperty("value", value, ES3TypeMgr.GetOrCreateES3Type(type), settings.referenceMode);
                    writer.EndWriteObject(key);
                }
                var bytes = ms.ToArray();
                Write7BitEncodedInt(bytes.Length);
                baseWriter.Write(bytes);
            }
            EndWriteProperty(key);
            MarkKeyForDeletion(key);
        }

        /// <summary>Writes a field or property to the writer. Note that this should only be called within an ES3Type.</summary>
        /// <param name="name">The name of the field or property.</param>
        /// <param name="value">The value we want to write.</param>
        /// <param name="memberReferenceMode">Whether we want to write the property by reference, by value, or both.</param>
        public override void WriteProperty(string name, object value, ES3.ReferenceMode memberReferenceMode)
        {
            if (SerializationDepthLimitExceeded())
                return;

            StartWriteProperty(name);

            using (var ms = new MemoryStream())
            {
                using (var writer = ES3Writer.Create(ms, new ES3Settings(ES3.EncryptionType.None, ES3.CompressionType.None, ES3.Format.Binary_Alpha), false, false))
                {
                    writer.Write(value, memberReferenceMode);
                }

                var bytes = ms.ToArray();
                Write7BitEncodedInt(bytes.Length);
                baseWriter.Write(bytes);
            }
            
            EndWriteProperty(name);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public override void WriteProperty(string name, object value, ES3Type type, ES3.ReferenceMode memberReferenceMode)
        {
            if (SerializationDepthLimitExceeded())
                return;

            StartWriteProperty(name);

            using (var ms = new MemoryStream())
            {
                using (var writer = ES3Writer.Create(ms, new ES3Settings(ES3.EncryptionType.None, ES3.CompressionType.None, ES3.Format.Binary_Alpha), false, false))
                {
                    writer.Write(value, type, memberReferenceMode);
                }

                var bytes = ms.ToArray();
                Write7BitEncodedInt(bytes.Length);
                baseWriter.Write(bytes);
            }

            EndWriteProperty(name);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public override void WritePropertyByRef(string name, UnityEngine.Object value)
        {
            if (SerializationDepthLimitExceeded())
                return;

            StartWriteProperty(name);
            

            using (var ms = new MemoryStream())
            {
                using (var writer = ES3Writer.Create(ms, new ES3Settings(ES3.EncryptionType.None, ES3.CompressionType.None, ES3.Format.Binary_Alpha), false, false))
                {
                    if (value == null)
                    {
                        WriteNull();
                        return;
                    };
                    writer.StartWriteObject(name);
                    writer.WriteRef(value);
                    writer.EndWriteObject(name);
                }

                var bytes = ms.ToArray();
                Write7BitEncodedInt(bytes.Length);
                baseWriter.Write(bytes);
            }

            EndWriteProperty(name);
        }

        #region WritePrimitive(value) methods.

        internal override void WritePrimitive(int value)		{ baseWriter.Write((byte)ES3SpecialByte.Int);       Write7BitEncodedInt(value); }
		internal override void WritePrimitive(float value)	    { baseWriter.Write((byte)ES3SpecialByte.Float);     baseWriter.Write(value); }
		internal override void WritePrimitive(bool value)		{ baseWriter.Write((byte)ES3SpecialByte.Bool);      baseWriter.Write(value); }
		internal override void WritePrimitive(decimal value)	{ baseWriter.Write((byte)ES3SpecialByte.Decimal);   baseWriter.Write(value); }
		internal override void WritePrimitive(double value)	    { baseWriter.Write((byte)ES3SpecialByte.Double);    baseWriter.Write(value); }
		internal override void WritePrimitive(long value)		{ baseWriter.Write((byte)ES3SpecialByte.Long);      baseWriter.Write(value); }
		internal override void WritePrimitive(ulong value)	    { baseWriter.Write((byte)ES3SpecialByte.Ulong);     baseWriter.Write(value); }
		internal override void WritePrimitive(uint value)		{ baseWriter.Write((byte)ES3SpecialByte.Uint);      baseWriter.Write(value); }
		internal override void WritePrimitive(byte value)		{ baseWriter.Write((byte)ES3SpecialByte.Byte);      baseWriter.Write(value); }
		internal override void WritePrimitive(sbyte value)	    { baseWriter.Write((byte)ES3SpecialByte.Sbyte);     baseWriter.Write(value); }
		internal override void WritePrimitive(short value)	    { baseWriter.Write((byte)ES3SpecialByte.Short);     baseWriter.Write(value); }
		internal override void WritePrimitive(ushort value)	    { baseWriter.Write((byte)ES3SpecialByte.Ushort);    baseWriter.Write(value); }
		internal override void WritePrimitive(char value)		{ baseWriter.Write((byte)ES3SpecialByte.Char);      baseWriter.Write(value); }
		internal override void WritePrimitive(byte[] value)		{ baseWriter.Write((byte)ES3SpecialByte.ByteArray); baseWriter.Write(value.Length);  baseWriter.Write(value); }
		internal override void WritePrimitive(string value)     { baseWriter.Write((byte)ES3SpecialByte.String);    baseWriter.Write(value);  }
        private void Write7BitEncodedInt(int value)
        {
            // Write out an int 7 bits at a time.  The high bit of the byte,
            // when on, tells reader to continue reading more bytes.
            uint v = (uint)value;   // support negative numbers
            while (v >= 0x80)
            {
                baseWriter.Write((byte)(v | 0x80));
                v >>= 7;
            }
            baseWriter.Write((byte)v);
        }

        internal override void WriteNull()
		{
			baseWriter.Write((byte)ES3SpecialByte.Null);
		}

		#endregion

		#region Overridden methods

		internal override void WriteRawProperty(string name, byte[] value)
		{
            StartWriteProperty(name);

                Write7BitEncodedInt(value.Length);
                baseWriter.Write(value);
            

            EndWriteProperty(name);
		}


        // File

		internal override void StartWriteFile()
        { 
            base.StartWriteFile(); 
        }
        internal override void EndWriteFile()
        {
            baseWriter.Write(ES3Binary.ObjectTerminator);
            base.EndWriteFile(); 
        }


        // Property

		internal override void StartWriteProperty(string name)
		{
            base.StartWriteProperty(name);
			baseWriter.Write(name);
        }

		internal override void EndWriteProperty(string name)
		{
            base.EndWriteProperty(name);
        }


        // Object

		internal override void StartWriteObject(string name)
		{
            base.StartWriteObject(name);
            baseWriter.Write((byte)ES3SpecialByte.Object);
        }

		internal override void EndWriteObject(string name)
		{
            baseWriter.Write(ES3Binary.ObjectTerminator);
            base.EndWriteObject(name);
        }


        // Collection

		internal override void StartWriteCollection()
		{
            base.StartWriteCollection();
            baseWriter.Write((byte)ES3SpecialByte.Collection);
        }

		internal override void EndWriteCollection()
		{
            baseWriter.Write((byte)ES3SpecialByte.Terminator);
            base.EndWriteCollection();
		}

		internal override void StartWriteCollectionItem(int index){}

		internal override void EndWriteCollectionItem(int index)
        { 
            baseWriter.Write((byte)ES3SpecialByte.CollectionItem); 
        }


        // Dictionary

		internal override void StartWriteDictionary()
		{
			StartWriteObject(null);
            baseWriter.Write((byte)ES3SpecialByte.Dictionary);
        }

		internal override void EndWriteDictionary()
		{
            baseWriter.Write((byte)ES3SpecialByte.Terminator);
            EndWriteObject(null);
		}

		internal override void StartWriteDictionaryKey(int index){}
		internal override void EndWriteDictionaryKey(int index){}
		internal override void StartWriteDictionaryValue(int index){}
		internal override void EndWriteDictionaryValue(int index){}

        #endregion

        #region Binary-specific methods

        #endregion

        public override void Dispose()
		{
        #if NETFX_CORE
            baseWriter.Dispose();
        #else
            baseWriter.Close();
        #endif
        }
    }
}
