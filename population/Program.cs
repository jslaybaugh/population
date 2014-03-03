using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestSimpleRNG;

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
		private static int _InitialPopulationMale;
		private static int _InitialPopulationFemale;

		private static int _MaxLifeSpan = 1000;

		private static int _MinimumReproductiveAge;
		private static int _MaximumReproductiveAge;

		private static int _MinimumYearsBetweenChildren = 1;
		private static int _AverageYearsBetweenChildren;
		private static int _StdDevYearsBetweenChildren;

		private static int _PercentReproductiveFemales;

		private static int _EndYear;
		private static int _CurrentYear;

		//////////////////

		class Person
		{
			public Guid Id { get; set; }
			public int BirthYear { get; set; }
			public bool IsFemale { get; set; }
			public bool WillReproduce { get; set; }

			private int _LifeSpan;
			private int _ReproductiveStartAge = -1;
			private int _ReproductiveEndAge = -1;
			private int _AgeAtNextBirth;

			public Person(int birthYear, bool? isFemale, bool? willReproduce, bool fullLife)
			{
				Id = Guid.NewGuid();
				BirthYear = birthYear;
				IsFemale = isFemale ?? SimpleRNG.GetUniform() < 0.5;

				if (IsFemale)
				{
					WillReproduce = willReproduce ?? (SimpleRNG.GetUniform() < (_PercentReproductiveFemales / 100.0000));

					if (WillReproduce)
					{
						_ReproductiveStartAge = (int)(NormalizeGamma(SimpleRNG.GetGamma(2, 4), 32) * ((_MaximumReproductiveAge - _MinimumReproductiveAge) / 2)) + _MinimumReproductiveAge;
						_ReproductiveEndAge = (int)((1 - NormalizeGamma(SimpleRNG.GetGamma(2, 2), 14)) * ((_MaximumReproductiveAge - _MinimumReproductiveAge) / 2)) + _MinimumReproductiveAge + (int)((_MaximumReproductiveAge - _MinimumReproductiveAge) / 2);
						_AgeAtNextBirth = _ReproductiveStartAge;
					}
				}

				_LifeSpan = fullLife ? _MaxLifeSpan : (int)((1 - NormalizeGamma(SimpleRNG.GetGamma(2, 2), 14)) * _MaxLifeSpan);

				//Console.WriteLine("Life: {0}, ReproStart: {1}, ReproEnd: {2}", _LifeSpan, _ReproductiveStartAge, _ReproductiveEndAge);
				//_Ages.Add(_LifeSpan);
				//if (WillReproduce)
				//{
				//	_ReproStarts.Add(_ReproductiveStartAge);
				//	_ReproEnds.Add(_ReproductiveEndAge);
				//}
			}

			public int Age
			{
				get
				{
					return _CurrentYear - BirthYear;
				}
			}

			public bool IsAlive
			{
				get
				{
					return Age < _LifeSpan;
				}
			}

			public bool IsReproductive
			{
				get
				{
					return IsAlive && _ReproductiveStartAge != -1 && (Age >= _ReproductiveStartAge && Age < _ReproductiveEndAge);
				}
			}


			public bool IsMakinABaby
			{
				get
				{
					if (IsReproductive && Age == _AgeAtNextBirth)
					{
						var gap = SimpleRNG.GetNormal(_AverageYearsBetweenChildren, _StdDevYearsBetweenChildren);
						if (gap < 1) gap = 1;
						_AgeAtNextBirth = Age + (int)gap;
						return true;
					}

					return false;
				}
			}
		}


		private static C5.ArrayList<Person> _People;
		//private static List<int> _Ages;
		//private static List<int> _ReproStarts;
		//private static List<int> _ReproEnds;

		static void Main(string[] args)
		{

			InitAndRun();


		}

		private static void InitAndRun()
		{
			Console.Clear();

			Console.WriteLine("Initial Population (Males): ");
			_InitialPopulationMale = Convert.ToInt16(Console.ReadLine());

			Console.WriteLine("Initial Population (Females): ");
			_InitialPopulationFemale = Convert.ToInt16(Console.ReadLine());

			Console.WriteLine("Maximum Life Span: ");
			_MaxLifeSpan = Convert.ToInt16(Console.ReadLine());

			Console.WriteLine("Minimum Reproductive Age (<= {0}): ", _MaxLifeSpan);
			_MinimumReproductiveAge = Convert.ToInt16(Console.ReadLine());

			Console.WriteLine("Maximum Reproductive Age (<= {0}): ", _MaxLifeSpan);
			_MaximumReproductiveAge = Convert.ToInt16(Console.ReadLine());

			Console.WriteLine("Average Years Between Children (between {0} and {1}): ", _MinimumYearsBetweenChildren, _MaximumReproductiveAge - _MinimumReproductiveAge);
			_AverageYearsBetweenChildren = Convert.ToInt16(Console.ReadLine());

			Console.WriteLine("Std Deviation Years Between Children (ex: 2 would mean 68% happen between {0} and {1} years): ", _AverageYearsBetweenChildren - 2, _AverageYearsBetweenChildren + 2);
			_StdDevYearsBetweenChildren = Convert.ToInt16(Console.ReadLine());

			Console.WriteLine("Percentage of Reproductive Females (between 0 and 100): ");
			_PercentReproductiveFemales = Convert.ToInt16(Console.ReadLine());

			Console.WriteLine("End Year: ");
			_EndYear = Convert.ToInt16(Console.ReadLine());

			RunNumbers();
		}

		private static void RunNumbers()
		{
			_People = new C5.ArrayList<Person>();
			//_Ages = new List<int>();
			//_ReproStarts = new List<int>();
			//_ReproEnds = new List<int>();

			//var stp = new Stopwatch();
			//stp.Start();

			for (int i = 0; i < _InitialPopulationMale; i++)
			{
				_People.Add(new Person(0, false, false, true));
			}

			for (int i = 0; i < _InitialPopulationFemale; i++)
			{
				_People.Add(new Person(0, true, true, true));
			}

			for (int i = 0; i < _EndYear; i++)
			{
				_CurrentYear = i;
				//Console.Write(".");
				//Console.WriteLine("Year {0}", i);

				_People.RemoveAll(_People.Where(y => !y.IsAlive));
				Console.WriteLine("Year {0}: {1} People", i, _People.Count().ToString("#,##0"));
				long newPeeps = _People.Count(x => x.IsMakinABaby);
				newPeeps = (long)(newPeeps * 1.04); // account for twins, triplets, etc based upon estimate from http://www.cdc.gov/nchs/fastats/multiple.htm (33.1 twins per 1000, 3x or higeher 124.4/100,000)
				_People.AddAll(Enumerable.Range(1, newPeeps).Select(x => new Person(i, null, null, false)));

			}

			//Console.WriteLine("Year {0}: {1} People, Avg age: {2}, Avg Repro Start: {3}, Avg Repro End: {4}", _EndYear, _People.Count(x => x.IsAlive).ToString("#,##0"), _Ages.Average(), _ReproStarts.Average(), _ReproEnds.Average());
			Console.WriteLine("Year {0}: {1} People", _EndYear, _People.Count().ToString("#,##0"));
			Console.WriteLine("-----");
			Console.WriteLine("DONE");
			Console.WriteLine("-----");
			Console.WriteLine("What now? ([R]epeat, [S]tart over, [Q]uit");
			Console.Beep();
			var action = Console.ReadLine();

			if (action.ToUpper() == "REPEAT" || action.ToUpper() == "R")
			{
				RunNumbers();
			}
			else if (action.ToUpper() == "START OVER" || action.ToUpper() == "S")
			{
				InitAndRun();
			}
			else
			{
				// quit
			}
		}

		private static double NormalizeGamma(double val, double thebase)
		{
			var k = val / thebase;
			if (k > 1) return 1;
			return k;
		}
	}
}
