using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;

namespace GladeToCs
{
	class Program
	{
		static void Main(string[] args)
		{
			var source = args[0];
			var destination = args[1];

			XmlReader reader = XmlReader.Create(source);
			while (reader.Read())
			{
				if (reader.Name == "template" && reader.NodeType is XmlNodeType.Element)
				{
					var typeName = reader.GetAttribute("class");
					var typeParent = reader.GetAttribute("parent");
					var props = new Dictionary<string, string>();

					while (reader.Read())
					{
						if (reader.Name == "template" && reader.NodeType is XmlNodeType.EndElement)
						{
							break;
						}

						if (reader.NodeType is XmlNodeType.Element)
						{
							if (reader.Name is "property")
							{
								var propName = reader.GetAttribute("name");
								reader.Read();
								props.Add(propName, reader.Value);
							}
							else if (reader.Name is "child")
							{
								 //PrintObjectRecursively(ReadObject(reader));
								 CreateFile(destination, typeName, typeParent, ReadObject(reader));
							}
						}
					}
					
				}
			}
			reader.Dispose();
			
		}

		private static void CreateFile(string destination, string type, string @base, Widget parent)
		{
			StreamWriter writer = new StreamWriter(destination);
			
			writer.Write("using System;\nusing Gtk;\n");
			writer.Write($"public class {type} : {@base}\n{{");
			
			writer.Write($"\tpublic {type}()\n\t{{");
			
			writer.Write("\t}");
			writer.Write("}");
			
			writer.Flush();
			writer.Close();
			writer.Dispose();
		}
		
		private static void PrintObjectRecursively(Widget obj, int depth = 0)
		{
			Console.WriteLine($"{depth.ToString().PadRight(depth, '\t')} {obj.Class} {obj.Id}");
			Console.WriteLine($"{depth.ToString().PadRight(depth, '\t')}   -properties");
			foreach (var property in obj.Properties)
			{
				Console.WriteLine(($"{depth.ToString().PadRight(depth, '\t')}   {property.Key} {property.Value}"));
			}
			Console.WriteLine($"{depth.ToString().PadRight(depth, '\t')}   -packing");
			foreach (var property in obj.Packing)
			{
				Console.WriteLine(($"{depth.ToString().PadRight(depth, '\t')}   {property.Key} {property.Value}"));
			}
			
			foreach (var child in obj.Children)
			{
				PrintObjectRecursively(child, depth + 1);
			}
		}
		
		private static Widget ReadObject(XmlReader reader, int depth = 0)
		{
			Dictionary<string, string> properties = new Dictionary<string, string>();
			Dictionary<string, string> packing = new Dictionary<string, string>();
			var obj = new Widget();
			while (reader.Read())
			{
				if (reader.NodeType is XmlNodeType.Element)
				{
					switch (reader.Name)
					{
						case "object":
							obj.Class = reader.GetAttribute("class");
							obj.Id = reader.GetAttribute("id");
							break;
						case "property":
							var propName = reader.GetAttribute("name");
							reader.Read();
							var propValue = reader.Value;
							properties.Add(propName, propValue);
							break;
						case "packing":
							while (reader.Read())
							{
								if (reader.Name is "packing")
								{
									break;
								}
								else
								{
									if (reader.NodeType is XmlNodeType.Element)
									{
										propName = reader.GetAttribute("name"); 
										reader.Read();
										propValue = reader.Value;
										packing.Add(propName, propValue);
									}
								}
							}
							break;
						case "child":
							obj.Children.Add(ReadObject(reader, depth + 1));
							break;
						case "placeholder":
							obj.Class = "<placeholder>";
							obj.Id = obj.Class;
							break;
					}
				}
				else if (reader.NodeType is XmlNodeType.EndElement && reader.Name is "child")
				{
					obj.Properties = properties;
					obj.Packing = packing;
					return obj;
				}
			}
			
			obj.Properties = properties;
			obj.Packing = packing;
			return obj;
		}
	}
}