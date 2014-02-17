using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace population
{

	public static class Enumerable
	{
		public static IEnumerable<long> Range(int start, long count)
		{
			var end = start + count;
			for (var current = start; current < end; ++current)
			{
				yield return current;
			}
		}
	}
	class Program
	{
		private static int _InitialPopulationMale = 0;
		private static int _InitialPopulationFemale = 0;
		private static int _AverageLifeSpan = 0;
		private static int _ReproductiveStartAge = 0;
		private static int _ReproductiveEndAge = 0;
		private static int _YearsBetweenChildren = 0;
		private static int _PercentReproductiveFemales = 0;
		private static int _EndYear = 0;

		private static Random rnd;

		class Person
		{
			public int BirthYear { get; set; }
			public bool IsFemale { get; set; }
			public int Age { get; set; }
			public bool WillReproduce { get; set; }

			public Guid Id { get; set; }

			public Person(int birthYear, bool? isFemale, bool? willReproduce)
			{
				Id = Guid.NewGuid();
				BirthYear = birthYear;
				IsFemale = isFemale ?? rnd.Next(0, 999) < 500;

				if (IsFemale)
				{
					WillReproduce = willReproduce ?? rnd.Next(0, 999) < (_PercentReproductiveFemales * 1000);
				}
				Age = 0;
			}


			//public void LiveAYear()
			//{
			//	if (IsFemale)
			//	{
			//		if (Age >= _ReproductiveStartAge && Age < _ReproductiveEndAge && WillReproduce)
			//		{

			//			if ( % _YearsBetweenChildren == 0)
			//			{
			//				// have a baby
			//				_NewPeopleQuantity += 1;
			//				ReproductiveYearCounter = 0;
			//			}
			//		}
			//	}

			//	if (Age >= _AverageLifeSpan)
			//	{
			//		// die
			//		_Dead.Add(this.Id);
			//	}

			//	Age += 1;
			//}
		}

		class LifeProcess
		{
			private int _thisYear;

			public LifeProcess(int thisYear)
			{
				_thisYear = thisYear;
			}

			public void DoWork()
			{

				_People.RemoveAll(x => _People.Where(y => y.Age >= _AverageLifeSpan).Select(y => y.Id).Contains(x.Id));

				//viable is who's left

				_NewPeopleQuantity = _People
					.Where(x => x.IsFemale && x.WillReproduce)
					.Where(x => x.Age >= _ReproductiveStartAge && x.Age < _ReproductiveEndAge)
					.Where(x => (x.Age - _ReproductiveStartAge) % _YearsBetweenChildren == 0)
					.Count();

				_People.AddRange(Enumerable.Range(1, _NewPeopleQuantity).Select(x => new Person(_thisYear, null, null)));


				_People.ForEach(x => x.Age += 1);
			}
		}


		private static List<Person> _People;
		private static int _NewPeopleQuantity;
		private static List<Guid> _Dead;

		static void Main(string[] args)
		{
			Console.WriteLine("Initial Population (Males): ");
			_InitialPopulationMale = Convert.ToInt16(Console.ReadLine());
			Console.WriteLine("Initial Population (Females): ");
			_InitialPopulationFemale = Convert.ToInt16(Console.ReadLine());
			Console.WriteLine("Average Life Span: ");
			_AverageLifeSpan = Convert.ToInt16(Console.ReadLine());
			Console.WriteLine("Reproductive Start Age: ");
			_ReproductiveStartAge = Convert.ToInt16(Console.ReadLine());
			Console.WriteLine("Reproductive End Age: ");
			_ReproductiveEndAge = Convert.ToInt16(Console.ReadLine());
			Console.WriteLine("Years Between Children: ");
			_YearsBetweenChildren = Convert.ToInt16(Console.ReadLine());
			Console.WriteLine("Percentage of Reproductive Females: ");
			_PercentReproductiveFemales = Convert.ToInt16(Console.ReadLine());
			Console.WriteLine("End Year: ");
			_EndYear = Convert.ToInt16(Console.ReadLine());

			_People = new List<Person>();
			_Dead = new List<Guid>();

			rnd = new Random();

			var stp = new Stopwatch();
			stp.Start();

			for (int i = 0; i < _InitialPopulationMale; i++)
			{
				_People.Add(new Person(0, false, false));
			}

			for (int i = 0; i < _InitialPopulationFemale; i++)
			{
				_People.Add(new Person(0, true, true));
			}

			for (int i = 0; i < _EndYear; i++)
			{
				var init = stp.ElapsedMilliseconds;
				Console.WriteLine("Year {0}: {1} People", i, _People.Count(x => x.Age < _AverageLifeSpan));
				//Console.WriteLine("YEAR {0}", i);
				//var life = new LifeProcess(i);
				//var lifeThread = new Thread(new ThreadStart(life.DoWork));

				//lifeThread.Start();
				//while (!lifeThread.IsAlive) ;

				//lifeThread.Join();

				var a = stp.ElapsedMilliseconds;
				Console.WriteLine("A:{0}", a - init);
				//_People.RemoveAll(x => _People.Where(y => y.Age >= _AverageLifeSpan).Select(y => y.Id).Contains(x.Id));

				//viable is who's left

				var b = stp.ElapsedMilliseconds;
				Console.WriteLine("B:{0}", b - a);
				long newPeeps = _People
					.Where(x => x.Age < _AverageLifeSpan && x.IsFemale && x.WillReproduce)
					.Where(x => x.Age >= _ReproductiveStartAge && x.Age < _ReproductiveEndAge)
					.Where(x => (x.Age - _ReproductiveStartAge) % _YearsBetweenChildren == 0)
					.Count();

				var c = stp.ElapsedMilliseconds;
				Console.WriteLine("C:{0}", c - b);
				_People.AddRange(Enumerable.Range(1, newPeeps).Select(x => new Person(i, null, null)));


				var d = stp.ElapsedMilliseconds;
				Console.WriteLine("D:{0}", d - c);
				_People.ForEach(x => x.Age += (x.Age < _AverageLifeSpan ? 1 : 0));


				var e = stp.ElapsedMilliseconds;
				Console.WriteLine("E:{0}", e - d);
			}

			Console.WriteLine("Year {0}: {1} People", _EndYear, _People.Count);
			Console.WriteLine("-----");
			Console.WriteLine("DONE");
			Console.WriteLine("-----");
			Console.ReadLine();
		}
	}
}
