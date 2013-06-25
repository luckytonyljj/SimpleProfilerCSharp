using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleProfiler
{
    class ProfilerRecording
    {
        // this class accumulates time for a single recording

        int count = 0;
        long accumulatedTime = 0;
        bool started = false;
        public string id;
        System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();

        public ProfilerRecording(string id)
        {
            this.id = id;
        }

        public void Start()
        {
            if (started) { BalanceError(); }
            count++;
            started = true;
            //startTimeMS = Time.realtimeSinceStartup; // done last
            stopWatch.Start();
        }

        public void Stop()
        {
            //float endTime = Time.realtimeSinceStartup; // done first
            if (!started) { BalanceError(); }
            started = false;
            //float elapsedTime = (endTime - startTimeMS);

            stopWatch.Stop();
            long elapsedTime = stopWatch.ElapsedMilliseconds;
            accumulatedTime += elapsedTime;
        }

        public void Reset()
        {
            accumulatedTime = 0;
            count = 0;
            started = false;
        }

        void BalanceError()
        {
            // this lets you know if you've accidentally
            // used the begin/end functions out of order
            //Debug.LogError("ProfilerRecording start/stops not balanced for '" + id + "'");
            Console.WriteLine("ProfilerRecording start/stops not balanced for '" + id + "'\n");
        }

        public long MilliSeconds
        {
            get { return accumulatedTime; }
        }

        public double Seconds
        {
            get { return accumulatedTime * 0.001; }
        }

        public int Count
        {
            get { return count; }
        }
    }
}
