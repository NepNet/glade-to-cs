using System.Collections.Generic;

namespace GladeToCs
{
	public class Widget
	{
		public string Class;
		public string Id;

		public Dictionary<string, string> Properties;
		public Dictionary<string, string> Packing;
		public List<Widget> Children = new List<Widget>();
	}
}