using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using ES3Internal;

public class ES3Spreadsheet
{
	private int cols = 0;
	private int rows = 0;
	private Dictionary<Index, string> cells = new Dictionary<Index, string>();

	private const string QUOTE = "\"";
	private const char QUOTE_CHAR = '"';
	private const char COMMA_CHAR = ',';
	private const char NEWLINE_CHAR = '\n';
	private const string ESCAPED_QUOTE = "\"\"";
	private static char[] CHARS_TO_ESCAPE = { ',', '"', '\n', ' ' };

    public ES3Spreadsheet()
    {
        ES3Debug.Log("ES3Spreadsheet created");
    }

	public int ColumnCount
	{
		get{ return cols; }
	}

	public int RowCount
	{
		get{ return rows; }
	}

	public void SetCell<T>(int col, int row, T value)
	{
        ES3Debug.Log("Setting cell (" + col + "," + row + ") to value " + value);

        // If we're writing a string, add it without formatting.
        if (value.GetType() == typeof(string))
		{
			SetCellString(col, row, (string)(object)value);
			return;
		}

		var settings = new ES3Settings ();
		using(var ms = new MemoryStream())
		{
			using (var jsonWriter = new ES3JSONWriter (ms, settings, false, false))
				jsonWriter.Write(value, ES3.ReferenceMode.ByValue);

			SetCellString(col, row, settings.encoding.GetString(ms.ToArray()));
		}

		// Expand the spreadsheet if necessary.
		if(col >= cols)
			cols = (col+1);
		if(row >= rows)
			rows = (row+1);
	}

	private void SetCellString(int col, int row, string value)
	{
		cells [new Index (col, row)] = value;

		// Expand the spreadsheet if necessary.
		if(col >= cols)
			cols = (col+1);
        if (row >= rows)
            rows = (row + 1);
    }


    // Don't create non-generic version of this. Generic parameter is necessary as no type data is stored in the CSV file.
    public T GetCell<T>(int col, int row)
	{
        var val = GetCell(typeof(T), col, row);

        if (val == null)
            return default(T);
        return (T)val;
	}

    internal object GetCell(System.Type type, int col, int row)
    {
        string value;

        if (col >= cols || row >= rows)
            throw new System.IndexOutOfRangeException("Cell (" + col + ", " + row + ") is out of bounds of spreadsheet (" + cols + ", " + rows + ").");

        if (!cells.TryGetValue(new Index(col, row), out value) || string.IsNullOrEmpty(value))
        {
            ES3Debug.Log("Getting cell (" + col + "," + row + ") is empty, so default value is being returned");
            return null;
        }

        // If we're loading a string, simply return the string value.
        if (type == typeof(string))
        {
            var str = (object)value;
            ES3Debug.Log("Getting cell (" + col + "," + row + ") with value " + str);
            return str;
        }

        var settings = new ES3Settings();
        using (var ms = new MemoryStream(settings.encoding.GetBytes(value)))
        {
            using (var jsonReader = new ES3JSONReader(ms, settings, false))
            {
                var obj = ES3TypeMgr.GetOrCreateES3Type(type, true).Read<object>(jsonReader);
                ES3Debug.Log("Getting cell (" + col + "," + row + ") with value " + obj);
                return obj;
            }
        }
    }

    public void Load(string filePath)
	{
		Load(new ES3Settings (filePath));
	}

	public void Load(string filePath, ES3Settings settings)
	{
		Load(new ES3Settings (filePath, settings));
	}

	public void Load(ES3Settings settings)
	{
		Load(ES3Stream.CreateStream(settings, ES3FileMode.Read), settings);
	}

	public void LoadRaw(string str)
	{
		Load(new MemoryStream (((new ES3Settings ()).encoding).GetBytes(str)), new ES3Settings());
	}

	public void LoadRaw(string str, ES3Settings settings)
	{
		Load(new MemoryStream ((settings.encoding).GetBytes(str)), settings);
	}

	private void Load(Stream stream, ES3Settings settings)
	{
		using (var reader = new StreamReader(stream))
		{
			int c_int;
			char c;
			string value = "";
			int col = 0;
			int row = 0;

            ES3Debug.Log("Reading spreadsheet "+settings.path+" from "+settings.location);

			// Read until the end of the stream.
			while(true)
			{
				c_int = reader.Read();
				c = (char)c_int;
				if(c == QUOTE_CHAR)
				{
					while (true)
					{
						c = (char)reader.Read();

						if(c == QUOTE_CHAR)
						{
							// If this quote isn't escaped by another, it is the last quote, so we should stop parsing this value.
							if(((char)reader.Peek()) != QUOTE_CHAR)
								break;
							else
								c = (char)reader.Read();
						}
						value += c;
					}
				}
				// If this is the end of a column, row, or the stream, add the value to the spreadsheet.
				else if(c == COMMA_CHAR || c == NEWLINE_CHAR || c_int == -1)
				{
					SetCell(col, row, value);
					value = "";
					if(c == COMMA_CHAR)
						col++;
					else if(c == NEWLINE_CHAR)
					{
						col = 0;
						row++;
					}
					else
						break;
				}
				else
					value += c;
			}
		}
        ES3Debug.Log("Finished reading spreadsheet " + settings.path + " from " + settings.location);
    }

	public void Save(string filePath)
	{
		Save(new ES3Settings (filePath), false);
	}

	public void Save(string filePath, ES3Settings settings)
	{
		Save(new ES3Settings (filePath, settings), false);
	}

	public void Save(ES3Settings settings)
	{
		Save(settings, false);
	}

	public void Save(string filePath, bool append)
	{
		Save(new ES3Settings (filePath), append);
	}

	public void Save(string filePath, ES3Settings settings, bool append)
	{
		Save(new ES3Settings (filePath, settings), append);
	}

	public void Save(ES3Settings settings, bool append)
	{
		using (var writer = new StreamWriter(ES3Stream.CreateStream(settings, append ? ES3FileMode.Append : ES3FileMode.Write)))
		{
			// If data already exists and we're appending, we need to prepend a newline.
			if(append && ES3.FileExists(settings))
				writer.Write(NEWLINE_CHAR);

			var array = ToArray();
			for(int row = 0; row < rows; row++)
			{
				if(row != 0)
					writer.Write(NEWLINE_CHAR);

				for(int col = 0; col < cols; col++)
				{
					if(col != 0)
						writer.Write(COMMA_CHAR);

                    ES3Debug.Log("Writing cell (" + col + "," + row + ") to file with value "+ array[col, row]);

                    writer.Write( Escape(array [col, row]) );
				}
			}
		}
		if(!append)
			ES3IO.CommitBackup(settings);
	}

	private static string Escape(string str, bool isAlreadyWrappedInQuotes=false)
	{
		if(string.IsNullOrEmpty(str))
			return null;

		// Now escape any other quotes.
		if(str.Contains(QUOTE))
			str = str.Replace(QUOTE, ESCAPED_QUOTE);
		
		// If there's chars to escape, wrap the value in quotes.
		if(str.IndexOfAny(CHARS_TO_ESCAPE) > -1)
			str = QUOTE + str + QUOTE;
		return str;
	}

	private static string Unescape(string str)
	{
		if(str.StartsWith(QUOTE) && str.EndsWith(QUOTE))
		{
			str = str.Substring(1, str.Length-2);
			if(str.Contains(ESCAPED_QUOTE))
				str = str.Replace(ESCAPED_QUOTE, QUOTE);
		}
		return str;
	}

	private string[,] ToArray()
	{
		var array = new string[cols, rows];
		foreach (var cell in cells)
			array [cell.Key.col, cell.Key.row] = cell.Value;
		return array;
	}

	protected struct Index
	{
		public int col;
		public int row;

		public Index(int col, int row)
		{
			this.col = col;
			this.row = row;
		}
	}
}
