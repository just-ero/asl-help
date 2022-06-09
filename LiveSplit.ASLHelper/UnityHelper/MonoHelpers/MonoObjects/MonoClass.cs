using System;
using System.Collections.Generic;
using System.Linq;

namespace ASLHelper.UnityHelper
{
    public class MonoClass
	{
		public string NameSpace { get; internal set; }
		public string Name { get; internal set; }
		public IntPtr Address { get; internal set; }
		public IntPtr Static { get; internal set; }
		public List<MonoField> Fields { get; internal set; }

		public int this[string fieldName]
		{
			get => Fields.First(f => f.Name == fieldName).Offset;
		}
	}
}