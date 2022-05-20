using System;
using System.IO;

namespace GladeToCs
{
	public class GladeParser
	{
		public GladeParser(string path)
		{
			var source = File.ReadAllText(path);

			int cursor = 0;
			
			bool ReadNext(out char c)
			{
				if (cursor < source.Length)
				{
					c = source[cursor++];
					return true;
				}
				c = '\0';
				return false;
			}

			while (ReadNext(out char c))
			{
				Console.Write(c);
			}
		}
	}
}