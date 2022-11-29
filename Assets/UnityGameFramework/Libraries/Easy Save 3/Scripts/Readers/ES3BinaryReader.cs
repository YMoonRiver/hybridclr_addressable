using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System;
using ES3Types;
using System.Globalization;

namespace ES3Internal
{
	/*
	 * 	Specific ES3Reader for reading Binary data.
	 * 
	 */
	public class ES3BinaryReader : ES3Reader
	{
		public BinaryReader baseReader;

		internal ES3BinaryReader(Stream stream, ES3Settings settings, bool readHeaderAndFooter = true) : base(settings, readHeaderAndFooter)
		{
			this.baseReader = new BinaryReader(stream, settings.encoding);
		}

		#region Property/Key Methods


        /*
		 * 	Reads the name of a property, and must be positioned immediately before a property name.
		 */
        public override string ReadPropertyName()
        {
            var propertyName = baseReader.ReadString();

            if (propertyName == null)
                throw new FormatException("Stream isn't positioned before a property.");
            else if (propertyName == ES3Binary.ObjectTerminator)
                return null;
            ES3Debug.Log("<b>" + propertyName + "</b> (reading property)", null, serializationDepth);
            return propertyName;
        }

        /*
		 * 	Reads the type data prefixed to this key.
		 * 	If ignore is true, it will return null to save the computation of converting
		 * 	the string to a Type.
		 */
        protected override Type ReadKeyPrefix(bool ignoreType=false)
		{
            Read7BitEncodedInt(); // Read length because at this point we no longer need it.

            StartReadObject();

			Type dataType = null;

            string propertyName = ReadPropertyName();

            if (propertyName == ES3Type.typeFieldName)
			{
                Read7BitEncodedInt(); // Read length because at this point we no longer need it.
                baseReader.ReadByte(); // Read the Type byte as we won't need this.
                string typeString = baseReader.ReadString();
				dataType = ignoreType ? null : Type.GetType(typeString);
				propertyName  = ReadPropertyName();
			}
				
			if(propertyName != "value")
				throw new FormatException("This data is not Easy Save Key Value data. Expected property name \"value\", found \""+propertyName+"\".");

			return dataType;
		}

		protected override void ReadKeySuffix()
        {
            var suffix = baseReader.ReadString();
            if(suffix != ES3Binary.ObjectTerminator)
                throw new FormatException("This data is not Easy Save Key Value data. Expected terminator, found \"" + suffix + "\".");
        }

		internal override bool StartReadObject() { /* Read the Type byte as we won't need this*/ baseReader.ReadByte(); return base.StartReadObject(); }
		internal override void EndReadObject(){ base.EndReadObject(); }
		internal override bool StartReadDictionary() {  /* Read the Type byte as we won't need this*/ baseReader.ReadByte(); return true; }
        internal override void EndReadDictionary(){}
		internal override bool StartReadDictionaryKey() {  /* Read the Type byte as we won't need this*/ baseReader.ReadByte(); return true; }
		internal override void EndReadDictionaryKey(){}
		internal override void StartReadDictionaryValue(){}
		internal override bool EndReadDictionaryValue(){ return true; }
		internal override bool StartReadCollection() {  /* Read the Type byte as we won't need this*/ baseReader.ReadByte(); return true; }
		internal override void EndReadCollection(){}
		internal override bool StartReadCollectionItem(){ return true; }
		internal override bool EndReadCollectionItem(){ return true; }

		#endregion

		#region Seeking Methods

        internal override byte[] ReadElement(bool skip = false)
        {
            using (var writer = skip ? null : new BinaryWriter(new MemoryStream(settings.bufferSize)))
            {
                ReadElement(writer, skip);
                if (skip)
                    return null;
                writer.Flush();
                return ((MemoryStream)writer.BaseStream).ToArray();
            }
        }

        /*
		 * 	Reads the current object in the stream.
		 * 	Stream position should be immediately after the key/property name.
		 */
        private void ReadElement(BinaryWriter writer, bool skip=false)
		{
            if (!skip)
                writer.Write(baseReader.ReadBytes(Read7BitEncodedInt()));
            else
                baseReader.ReadBytes(Read7BitEncodedInt());
        }

		#endregion

		#region Primitive Read() Methods.

        internal override long Read_ref()
        {
            if (ES3ReferenceMgr.Current == null)
                throw new InvalidOperationException("An Easy Save 3 Manager is required to load references. To add one to your scene, exit playmode and go to Assets > Easy Save 3 > Add Manager to Scene");
            Read7BitEncodedInt(); // Read length as we won't need this.
            baseReader.ReadByte(); // Read type byte as we won't need this.
            return long.Parse(baseReader.ReadString());
        }

        internal override string    Read_string()   { baseReader.ReadByte(); return baseReader.ReadString();    }
        internal override char		Read_char()		{ baseReader.ReadByte(); return baseReader.ReadChar();      }
		internal override float		Read_float()	{ baseReader.ReadByte(); return baseReader.ReadSingle();    }
		internal override int 		Read_int()		{ baseReader.ReadByte(); return Read7BitEncodedInt();       }
		internal override bool 		Read_bool()		{ baseReader.ReadByte(); return baseReader.ReadBoolean(); 	}
		internal override decimal 	Read_decimal()	{ baseReader.ReadByte(); return baseReader.ReadDecimal(); 	}
		internal override double 	Read_double()	{ baseReader.ReadByte(); return baseReader.ReadDouble(); 	}
		internal override long 		Read_long()		{ baseReader.ReadByte(); return baseReader.ReadInt64();	    }
		internal override ulong 	Read_ulong()	{ baseReader.ReadByte(); return baseReader.ReadUInt64();	}
		internal override uint 		Read_uint()		{ baseReader.ReadByte(); return baseReader.ReadUInt32(); 	}
		internal override byte 		Read_byte()		{ baseReader.ReadByte(); return baseReader.ReadByte(); 	    }
		internal override sbyte 	Read_sbyte()	{ baseReader.ReadByte(); return baseReader.ReadSByte(); 	}
		internal override short 	Read_short()	{ baseReader.ReadByte(); return baseReader.ReadInt16(); 	}
		internal override ushort 	Read_ushort()	{ baseReader.ReadByte(); return baseReader.ReadUInt16(); 	}
		internal override byte[] 	Read_byteArray(){ baseReader.ReadByte(); return baseReader.ReadBytes(baseReader.ReadInt32()); }

        #endregion

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public override T Read<T>(ES3Type type)
        {
            Read7BitEncodedInt(); // Read length because at this point we no longer need it.
            return base.Read<T>(type);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public override void ReadInto<T>(object obj, ES3Type type)
        {
            Read7BitEncodedInt(); // Read length because at this point we no longer need it.
            base.ReadInto<T>(obj, type);
        }

        #region Binary-specific methods

        private int Read7BitEncodedInt()
        {
            // Read out an Int32 7 bits at a time.  The high bit
            // of the byte when on means to continue reading more bytes.
            int count = 0;
            int shift = 0;
            byte b;
            do
            {
                // Check for a corrupted stream.  Read a max of 5 bytes.
                // In a future version, add a DataFormatException.
                if (shift == 5 * 7)  // 5 bytes max per Int32, shift += 7
                    throw new FormatException("The int being read is not a 7-bit encoded int");

                // ReadByte handles end of stream cases for us.
                b = baseReader.ReadByte();
                count |= (b & 0x7F) << shift;
                shift += 7;
            } while ((b & 0x80) != 0);
            return count;
        }

        #endregion


        public override void Dispose()
		{
        #if NETFX_CORE
            baseReader.Dispose();
        #else
            baseReader.Close();
        #endif
        }
    }
}