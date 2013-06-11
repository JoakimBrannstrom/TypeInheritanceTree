using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TypeInheritanceTree.Tests
{
	[TestClass]
	public class TypeInheritanceParserTests
	{
		#region Types
		class Vanilla { }
		class Chocolate : Vanilla { }

		interface IIceCream { }
		interface IVanilla : IIceCream { }
		interface IChocolate : IIceCream { }
		class Mixed : IVanilla, IChocolate { }

		class SuperMixed : Mixed { }
		#endregion

		[TestMethod]
		public void GivenVanillaType_WhenBuildingTree_ThenVanillaAndObjectShouldShowUp()
		{
			// Arrange & Act
			var tree = GetTree<Vanilla>();

			// Assert
			VerifyVanillaTree(tree);
		}

		void VerifyVanillaTree(TypeInheritance tree)
		{
			Assert.IsNotNull(tree);
			Assert.AreEqual(typeof(Vanilla), tree.Item1);
			Assert.AreEqual(typeof(object), tree.Item2.First().Item1);
		}

		[TestMethod]
		public void GivenChocolateType_WhenBuildingTree_ThenChocolateAndVanillaAndObjectShouldShowUp()
		{
			// Arrange & Act
			var tree = GetTree<Chocolate>();

			// Assert
			Assert.IsNotNull(tree);
			Assert.AreEqual(typeof(Chocolate), tree.Item1);
			VerifyVanillaTree(tree.Item2.Single());
		}

		[TestMethod]
		public void GivenMixedType_WhenBuildingTree_ThenMixedAndIChocolateAndIVanillaAndObjectShouldShowUp()
		{
			// Arrange & Act
			var tree = GetTree<Mixed>();

			// Assert
			VerifyMixedTree(tree);
		}

		void VerifyMixedTree(TypeInheritance tree)
		{
			Assert.IsNotNull(tree);
			Assert.AreEqual(typeof(Mixed), tree.Item1);

			var subTree = tree.Item2;
			Assert.IsNotNull(subTree);
			Assert.IsTrue(subTree.Any(t => t.Item1 == typeof(object)));
			Assert.IsTrue(subTree.Any(t => t.Item1 == typeof(IVanilla)));
			Assert.IsTrue(subTree.Any(t => t.Item1 == typeof(IChocolate)));
			Assert.IsFalse(subTree.Any(t => t.Item1 == typeof(IIceCream)));
		}

		[TestMethod]
		public void GivenSuperMixedType_WhenBuildingTree_ThenSuperMixedAndMixedAndIChocolateAndIVanillaAndObjectShouldShowUp()
		{
			// Arrange & Act
			var tree = GetTree<SuperMixed>();

			// Assert
			Assert.IsNotNull(tree);
			Assert.AreEqual(typeof(SuperMixed), tree.Item1);
			VerifyMixedTree(tree.Item2.Single());
		}

		TypeInheritance GetTree<T>()
		{
			// Arrange
			var parser = new TypeInheritanceParser();

			// Act
			var tree = parser.GetTree(typeof(T));
			PrintInheritanceTree(tree, 0);

			return tree;
		}

		void PrintInheritanceTree(TypeInheritance tree, int level)
		{
			var indent = Enumerable.Range(0, level).Aggregate("", (current, i) => string.Format("{0}{1}", current, '\t'));
			Console.WriteLine(indent + tree.Item1.Name);

			foreach(var type in tree.Item2)
			{
				PrintInheritanceTree(type, level + 1);
			}
		}
	}
}
