using System;
//pure brute force edition
namespace ConsoleApp1
{
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using System.Text;

	public class photo
	{
		public int id;
		public bool H;
		public List<string> tags;
		public bool taken = false;
	}

	public class slide
	{
		public byte order = 0;
		public int id;
		public int id1; //will be null if H == true;
		public bool H;
		public List<string> tags;
	}

	public class VerticalPairs
	{
		public int id;
		public int id1;
		public List<string> tags;
	}

	public class Program
	{
		private static Random rng = new Random();

		public static void Shuffle<T>(IList<T> list)
		{
			int n = list.Count;
			while (n > 1)
			{
				n--;
				int k = rng.Next(n + 1);
				T value = list[k];
				list[k] = list[n];
				list[n] = value;
			}
		}
		public static int getMatched(List<string> firstlist, List<string> secondlist)
		{
			return firstlist.Intersect(secondlist).ToList().Count;

		}

		public static void Main()
		{

			List<photo> photos = new List<photo>();
			List<slide> slides = new List<slide>();
			List<VerticalPairs> vpp = new List<VerticalPairs>();

			int counter = 0;
			int n = 0;
			int i = 0;
			string line;

			//Read the file and display it line by line.  
			System.IO.StreamReader file = new System.IO.StreamReader(@"input.txt");
			while ((line = file.ReadLine()) != null)
			{
				//System.Console.WriteLine(line);  
				if (counter == 0)
				{
					n = System.Convert.ToInt32(line);
				}
				else
				{
					string[] data = line.Split(new string[] { " ", " " }, StringSplitOptions.None);
					photo p = new photo();
					p.id = i;
					p.H = true;
					if (data[0] == "V")
					{
						p.H = false;
					}

					long tempN = System.Convert.ToInt32(data[1]);
					List<string> tempList = new List<string>();
					for (long j = 1; j <= tempN; j++)
					{
						tempList.Add(data[j + 1]);

					}
					p.tags = tempList;
					photos.Add(p);
					i++;
				}
				counter++;
			}
			file.Close();
			//sort vertical
			List<photo> verticallist = photos.Where(x => x.H == false).ToList();
			//Program.Shuffle<photo>(verticallist);
			foreach (var a in verticallist)
			{
				if (a.taken)
				{
					continue;
				}
				a.taken = true;
				var leastid = 0;
				var leastcommons = 100000;
				foreach (var b in verticallist.Where(x => x.taken == false).ToList())
				{
					if (!b.taken)
					{
						var asd = a;
						var firstlist = a.tags.ToList();
						var secondlist = b.tags.ToList();
						List<string> commons = firstlist.Intersect(secondlist).ToList();
						if (commons.Count < leastcommons)
						{
							leastid = b.id;
							leastcommons = commons.Count;
						}
						if (leastcommons == 0)
						{
							break;
						}
					}
				}
				//assign leastb to checked 
				verticallist.Where(x => x.id == leastid).First().taken = true;
				HashSet<string> newtags = new HashSet<string>(a.tags.ToList());
				foreach (var asd in verticallist.Where(x => x.id == leastid).First().tags)
				{
					newtags.Add(asd);
				}

				VerticalPairs pair = new VerticalPairs();
				pair.id = a.id;
				pair.id1 = leastid;
				pair.tags = newtags.ToList();
				vpp.Add(pair);
			}


			//Sort Slides
			foreach (var a in vpp)
			{
				slide newslide = new slide
				{
					id = a.id,
					id1 = a.id1,
					H = false,
					tags = a.tags
				};
				slides.Add(newslide);
			}

			foreach (var a in photos.Where(x => x.H == true))
			{
				slide newslide = new slide
				{
					id = a.id,
					id1 = -1,
					H = true,
					tags = a.tags
				};
				slides.Add(newslide);
			}

			//sort slides
			//for(int loop =0; loop < 10; loop++)
			//{
			//	slides.Sort((x, y) => x.tags.Count.CompareTo(getMatched(x.tags, y.tags)));
			//}

			//ManualSort
			List<slide> resultslides = new List<slide>();
			
			for (int loop1 = 0; loop1 < 1; loop1++)
			{
				List<slide> inputslides = new List<slide>();
				List<slide> outputSlides = new List<slide>();
				if (loop1 > 0)
				{
					inputslides = resultslides;
					inputslides.Reverse();
				}
				else
				{
					inputslides = slides.ToList();
					inputslides.Sort((x, y) => x.tags.ToList().Count.CompareTo(getMatched(x.tags.ToList(), y.tags.ToList())));

				}

				outputSlides.Add(inputslides.First());
				inputslides.Remove(inputslides.First());
				for (int loop = inputslides.Count; loop > 0; loop--)
				{
					var tempSlide = outputSlides.Last();
					if (tempSlide == null)
					{
						continue;
					}
					Console.WriteLine(loop);
					var mostid = 0;
					var mostcommons = 0;
					bool first = true;
					foreach (var b in inputslides.ToList())
					{
						if (b.id == tempSlide.id) continue;
						if (first)
						{
							mostid = b.id;
							first = false;
						}

						var firstlist = tempSlide.tags.ToList();
						var secondlist = b.tags.ToList();
						List<string> commons = firstlist.Intersect(secondlist).ToList();
						if (commons.Count == firstlist.Count)
						{
							mostid = b.id;
							break;
						}

						if (commons.Count - firstlist.Count / 2 == mostcommons)
						{
							mostid = b.id;
							mostcommons = commons.Count;

						}

						if (mostcommons == firstlist.Count)
						{
							mostid = b.id;
							break;
						}
					}

					outputSlides.Add(slides.Where(x => x.id == mostid).First());
					inputslides.Remove(slides.Where(x => x.id == mostid).First());

				}
				resultslides = outputSlides;

			}


			//slides.Sort((x, y) => x.tags.ToList().Intersect(y.tags.ToList()).ToList().Count.CompareTo(y.tags.ToList().Count));
			//slides.Sort((x, y) => x.tags.ToList().Intersect(y.tags.ToList()).ToList().Count.CompareTo(x.tags.ToList().Count));

			Console.WriteLine(photos.Count);
			Console.WriteLine(resultslides.Where(x => x.H == true).ToList().Count);
			Console.WriteLine(resultslides.Where(x => x.H == false).ToList().Count);
			Printoutput(resultslides);

		}

		public static void Printoutput(List<slide> list)
		{
			using (System.IO.StreamWriter myfile =
			new System.IO.StreamWriter(@"output.txt", true))
			{
				StringBuilder sb = new StringBuilder("");
				sb.AppendFormat("{0}\n", list.Count);

				foreach (var a in list)
				{
					if (!a.H && a.id1 != -1)
					{
						sb.AppendFormat("{0} {1}\n", a.id1, a.id);
					}
					else
					{
						sb.AppendFormat("{0}\n", a.id);
					}
				}

				myfile.Write(sb);
				Console.ReadKey();
			}

			return;
		}
	}
}
