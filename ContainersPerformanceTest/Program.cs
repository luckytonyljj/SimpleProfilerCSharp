using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleProfiler;

namespace ContainersPerformanceTest
{
    public static class DictionaryExtensions
    {
        public static Dictionary<TKey, TValue> Shuffle<TKey, TValue>(
           this Dictionary<TKey, TValue> source)
        {
            Random r = new Random();
            return source.OrderBy(x => r.Next())
               .ToDictionary(item => item.Key, item => item.Value);
        }

        public static SortedDictionary<TKey, TValue> Shuffle<TKey, TValue>(
           this SortedDictionary<TKey, TValue> source)
        {
            Random r = new Random();

            return new SortedDictionary<TKey, TValue>(source.OrderBy(x => r.Next())
               .ToDictionary(item => item.Key, item => item.Value));
        }
    }

    class Program
    {
        enum PROFILINGTEST_TYPE {  
            ACCESS_ARRAY = 0,
            ACCESS_LIST,
            ACCESS_DICTIONARY,
            ACCESS_SORTEDDICTIONARY,
            ACCESS_HASHSET,
            SEARCH_ARRAY,
            SEARCH_LIST,
            SEARCH_DICTIONARY,
            SEARCH_SORTEDDICTIONARY,
            SEARCH_HASHSET,
            COUNT,
        }

        public class Rand
        {
            Random rand = new Random(unchecked((int)DateTime.Now.Ticks));
            public int GetRandom(int maxValue) 
            {
                //rand = new Random(unchecked((int)DateTime.Now.Ticks));
                return rand.Next((int)maxValue); 
            }
            public int GetRandom(int maxValue, params int[] exceptionValues)
            {
                if (exceptionValues.Length < 1) return GetRandom(maxValue);

                int randNum = 0;
                while (true)
                {
                    randNum = GetRandom(maxValue);
                    bool duplication = false;
                    
                    Array.ForEach( exceptionValues, p => { if (p == randNum) duplication = true; });
                    if (false == duplication) break;
                }
                return randNum;
            }
        }

        static Rand randInstance = new Rand();

        public static void Swap<T>(ref T a, ref T b) { T t = b; b = a; a = t; }
        public static void RandomShuffle(int[] a)
        {
            if (a.Length < 1) return;
            int count = a.Length;

            Array.ForEach(a, p =>
            {
                int rand = randInstance.GetRandom((int)count, p);
                Swap(ref a[rand], ref p);
            });
        }

        public static void RandomShuffle(List<int> a)
        {
            if (a.Count < 1) return;
            int count = a.Count;

            a.ForEach(p =>
            {
                int rand = randInstance.GetRandom((int)count, p);
                int t = a[rand];
                a[rand] = p;
                p = t;
            });
        }

        //public static void RandomShuffle(SortedDictionary<int, int> a)
        //{
        //    if (a.Count < 1) return;
        //    int count = a.Count;

        //    foreach (KeyValuePair<int, int> pair in a)
        //    {
        //        int rand = Rand.GetRandom((int)count, pair.Key);
        //        int t = a[rand];
        //        a[rand] = pair.Value;
        //        a[pair.Key] = t;
        //    }
        //}

        static void Main(string[] args)
        {
            uint limitsRand = int.MaxValue;
            uint limitsDataCount = 100000;
            uint limitsSearchCount = 100000;
            uint dataCounts = 0, searchCounts = 0;
            Console.WriteLine("Profile containers");
            Console.Write("Input count of data (1~" + limitsDataCount + ") : ");
            string inputText = Console.ReadLine();
            dataCounts = uint.Parse(inputText); dataCounts = dataCounts > limitsDataCount ? limitsDataCount : dataCounts;
            Console.Write("Input count of Search (1~"+ limitsSearchCount +") : ");
            string inputTextSearchCount = Console.ReadLine();
            searchCounts = uint.Parse(inputTextSearchCount); searchCounts = searchCounts > limitsSearchCount ? limitsSearchCount : searchCounts;

            PROFILINGTEST_TYPE[] testtypes = new PROFILINGTEST_TYPE[(int)(PROFILINGTEST_TYPE.COUNT)];
            int[] dataTable = new int[dataCounts];

            int[] searchIndexTable = new int[dataCounts];
            int[] searchTable = new int[dataCounts];
            
            int sequence = 0;
            
            foreach (int data in dataTable)
            {
                int current = sequence++;
                int randnum = 0;
                bool duplicated = true;

                while (duplicated)
                {
                    randnum = randInstance.GetRandom((int)limitsRand);
                    duplicated = false;
                    for (int i = 0; i < current; ++i)
                    {
                        if (randnum == dataTable[i]) { duplicated = true; break; }
                    }
                }

                dataTable[current] = randnum;
                dataTable[current] = randInstance.GetRandom((int)limitsRand);
                searchIndexTable[current] = current;
            }

            int[] bufferTable = new int[dataCounts];
            List<int> bufferList = new List<int>();
            Dictionary<int, int> bufferDic = new Dictionary<int, int>();
            SortedDictionary<int, int> bufferSortedDic = new SortedDictionary<int, int>();

            for (int i = 0; i < dataCounts; ++i)
            {
                int data = dataTable[i];
                searchTable[i] = data;
                bufferTable[i] = data;
                bufferList.Add(data);
                bufferDic[i] = data;
                bufferSortedDic[i] = data;
            }

            sequence = 0;
            RandomShuffle(searchIndexTable);
            RandomShuffle(searchTable);
            RandomShuffle(bufferTable);
            RandomShuffle(bufferList);
            bufferDic.Shuffle();
            bufferSortedDic.Shuffle();
            //RandomShuffle(bufferDic);
            //RandomShuffle(bufferSortedDic);

            int dataSearch = 0;
            Profiler profiler = new Profiler();
            profiler.TotalStart();

            #region profiling_indexsearch

            profiler.Begin(PROFILINGTEST_TYPE.ACCESS_ARRAY.ToString());
            foreach (int idx in searchIndexTable)
            {
                dataSearch = bufferTable[idx];
            }
            profiler.End(PROFILINGTEST_TYPE.ACCESS_ARRAY.ToString());

            profiler.Begin(PROFILINGTEST_TYPE.ACCESS_LIST.ToString());
            foreach (int idx in searchIndexTable)
            {
                dataSearch = bufferList[idx];
            }
            profiler.End(PROFILINGTEST_TYPE.ACCESS_LIST.ToString());

            profiler.Begin(PROFILINGTEST_TYPE.ACCESS_DICTIONARY.ToString());
            foreach (int idx in searchIndexTable)
            {
                dataSearch = bufferDic[idx];
            }
            profiler.End(PROFILINGTEST_TYPE.ACCESS_DICTIONARY.ToString());

            profiler.Begin(PROFILINGTEST_TYPE.ACCESS_SORTEDDICTIONARY.ToString());
            foreach (int idx in searchIndexTable)
            {
                dataSearch = bufferSortedDic[idx];
            }
            profiler.End(PROFILINGTEST_TYPE.ACCESS_SORTEDDICTIONARY.ToString());

            #endregion

            #region profiling_search
            

            #endregion

            profiler.TotalEndResult();

            return;
        }
    }
}
