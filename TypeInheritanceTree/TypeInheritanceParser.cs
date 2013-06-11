using System;
using System.Collections.Generic;
using System.Linq;

namespace TypeInheritanceTree
{
	public class TypeInheritance : Tuple<Type, List<TypeInheritance>>
	{
		public TypeInheritance(Type type) : base(type, new List<TypeInheritance>())
		{
		}
	}

    public class TypeInheritanceParser
    {
		public TypeInheritance GetTree(Type type)
		{
			if(type == null)
				return null;

			var tree = new TypeInheritance(type);

			var subTree = GetTree(type.BaseType);
			if(subTree != null)
				tree.Item2.Add(subTree);

			var allTypes = GetAllTypes(tree).ToList();

			foreach (var contract in type.GetInterfaces())
			{
				if(allTypes.Contains(contract))
					continue;

				var subContractTree = GetTree(contract);
				if (subContractTree != null)
					tree.Item2.Add(subContractTree);

				allTypes.AddRange(GetAllTypes(subContractTree));
			}

			return tree;
		}

		IEnumerable<Type> GetAllTypes(TypeInheritance tree)
		{
			yield return tree.Item1;

			foreach (var subTree in tree.Item2)
			{
				var subTypes = GetAllTypes(subTree);
				foreach(var type in subTypes)
				{
					yield return type;
				}
			}
		}
    }
}
