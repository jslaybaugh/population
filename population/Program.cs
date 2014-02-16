using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace population
{
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
				IsFemale = isFemale ?? rnd.Next(0,999) < 500;

				if (IsFemale)
				{
					WillReproduce = willReproduce ?? rnd.Next(0, 999) < (_PercentReproductiveFemales * 1000);
				}
				Age = 0;
			}

			private int _reproductiveYearCounter = 0;

			public void LiveAYear()
			{
				if (IsFemale)
				{
					if (Age >= _ReproductiveStartAge && Age < _ReproductiveEndAge && WillReproduce)
					{
						// female planning on reproducing within reproductive years
						_reproductiveYearCounter += 1;
						if (_reproductiveYearCounter % _YearsBetweenChildren == 0)
						{
							// have a baby
							_NewPeopleQuantity += 1;
							_reproductiveYearCounter = 0;
						}
					}
				}

				if (Age >= _AverageLifeSpan)
				{
					// die
					_Dead.Add(this.Id);
				}

				Age += 1;
			}
		}

		class LifeProcess
		{
			public void DoWork()
			{
				foreach (var person in _People)
				{
					person.LiveAYear();
				}
			}
		}

		class DeathProcess
		{
			public void DoWork()
			{
				foreach (var id in _Dead)
				{
					_People.Remove(_People.FirstOrDefault(x => x.Id == id));
				}

				_Dead = new List<Guid>();
			}
		}

		class BirthProcess
		{
			private int _thisYear;

			public BirthProcess(int thisYear)
			{
				_thisYear = thisYear;
			}
			public void DoWork()
			{
				for (int j = 0; j < _NewPeopleQuantity; j++)
				{
					_People.Add(new Person(_thisYear, null, null));
				}

				_NewPeopleQuantity = 0;
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
				Console.WriteLine("Year {0}: {1} People", i, _People.Count);

				var life = new LifeProcess();
				var lifeThread = new Thread(new ThreadStart(life.DoWork));

				lifeThread.Start();
				while (!lifeThread.IsAlive) ;

				lifeThread.Join();

				////////////

				var birth = new BirthProcess(i);
				var birthThread = new Thread(new ThreadStart(birth.DoWork));

				birthThread.Start();
				while (!birthThread.IsAlive);

				birthThread.Join();

				////////////

				var death = new DeathProcess();
				var deathThread = new Thread(new ThreadStart(death.DoWork));

				deathThread.Start();
				while (!deathThread.IsAlive);

				deathThread.Join();

			}

			Console.WriteLine("Year {0}: {1} People", _EndYear, _People.Count);
			Console.WriteLine("-----");
			Console.WriteLine("DONE");
			Console.WriteLine("-----");
			Console.ReadLine();
		}
	}
}
