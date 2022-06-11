using System;
using System.IO;
using System.Text;

public class ByteBuffer
{
	private MemoryStream stream;

	private BinaryWriter writer;

	private BinaryReader reader;

	private int length;

	public int readPos
	{
		get
		{
			return (int)stream.Position;
		}
	}

	public int dataLength
	{
		get
		{
			return length;
		}
	}

	public ByteBuffer()
	{
		stream = new MemoryStream();
		writer = new BinaryWriter(stream);
	}

	public ByteBuffer(byte[] data)
	{
		if (data != null)
		{
			length = data.Length;
			stream = new MemoryStream(data);
			reader = new BinaryReader(stream);
		}
		else
		{
			length = 0;
			stream = new MemoryStream();
			writer = new BinaryWriter(stream);
		}
	}

	public void Close()
	{
		if (writer != null)
		{
			writer.Close();
		}
		if (reader != null)
		{
			reader.Close();
		}
		stream.Close();
		writer = null;
		reader = null;
		stream = null;
	}

	public void WriteByte(byte v)
	{
		writer.Write(v);
	}

	public void WriteInt(int v)
	{
		writer.Write(v);
	}

	public void WriteUInt(uint v)
	{
		writer.Write(v);
	}

	public void WriteShort(ushort v)
	{
		writer.Write(v);
	}

	public void WriteLong(long v)
	{
		writer.Write(v);
	}

	public void WriteFloat(float v)
	{
		byte[] bytes = BitConverter.GetBytes(v);
		Array.Reverse(bytes);
		writer.Write(BitConverter.ToSingle(bytes, 0));
	}

	public void WriteDouble(double v)
	{
		byte[] bytes = BitConverter.GetBytes(v);
		Array.Reverse(bytes);
		writer.Write(BitConverter.ToDouble(bytes, 0));
	}

	public void WriteString(string v)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(v);
		writer.Write((ushort)bytes.Length);
		writer.Write(bytes);
	}

	public void WriteBytes(byte[] v)
	{
		writer.Write((short)v.Length);
		writer.Write(v);
	}

	public byte ReadByte()
	{
		return reader.ReadByte();
	}

	public int ReadInt()
	{
		return reader.ReadInt32();
	}

	public ushort ReadShort()
	{
		return (ushort)reader.ReadInt16();
	}

	public long ReadLong()
	{
		return reader.ReadInt64();
	}

	public float ReadFloat()
	{
		byte[] bytes = BitConverter.GetBytes(reader.ReadSingle());
		Array.Reverse(bytes);
		return BitConverter.ToSingle(bytes, 0);
	}

	public double ReadDouble()
	{
		byte[] bytes = BitConverter.GetBytes(reader.ReadDouble());
		Array.Reverse(bytes);
		return BitConverter.ToDouble(bytes, 0);
	}

	public string ReadString()
	{
		int count = reader.ReadInt16();
		byte[] bytes = reader.ReadBytes(count);
		return Encoding.UTF8.GetString(bytes);
	}

	public void Reset()
	{
		stream.Seek(0L, SeekOrigin.Begin);
	}

	public byte[] ReadBytes()
	{
		int count = ReadShort();
		return reader.ReadBytes(count);
	}

	public byte[] ReadAllBytes()
	{
		return reader.ReadBytes(length - 4);
	}

	public byte[] ToBytes()
	{
		writer.Flush();
		return stream.ToArray();
	}

	public void Flush()
	{
		writer.Flush();
	}
}
