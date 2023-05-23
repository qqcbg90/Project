using System;

namespace KingspModel.Attributes
{
	public class PriorityAttribute : Attribute
	{
		public int priority;
		public PriorityAttribute(int _priority)
		{
			this.priority = _priority;
		}
	}
}
