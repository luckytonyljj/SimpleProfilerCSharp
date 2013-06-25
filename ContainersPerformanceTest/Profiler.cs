using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace SimpleProfiler
{
    public class Profiler
    {
        int numCallCount = 0;
        Stopwatch stopwatch = new Stopwatch();
        
        static Dictionary<string, ProfilerRecording> recordings = new Dictionary<string, ProfilerRecording>();
        string displayText;

        void startTime() { stopwatch.Start(); }
        uint endTime() { stopwatch.Stop(); return (uint)stopwatch.ElapsedMilliseconds; }

        void Init()
        {
            numCallCount = 1;
            displayText = "\n\nTaking initial readings...";
        }

        void OutputProfileResult(string outputText)
        {
            //GUI.Box(displayRect, "Code Profiler");    //DELETE
            //GUI.Label(displayRect, displayText);
            Console.WriteLine(outputText);
        }

        public void Begin(string id)
        {
            // create a new recording if not present in the list
            if (!recordings.ContainsKey(id))
            {
                recordings[id] = new ProfilerRecording(id);
            }

            recordings[id].Start();
        }

        public void End(string id)
        {
            recordings[id].Stop();
        }

        public void TotalStart()
        {
            startTime();
        }

        public void TotalEndResult()
        {
            // time to display the results      

            // column width for text display
            int colWidth = 10;

            // the overall frame time and frames per second:
            displayText = "\n\n";
            float totalMS = endTime();
            float avgMS = (totalMS / numCallCount);
            float fps = (1000 / (totalMS / numCallCount));
            displayText += "Avg frame time: ";
            displayText += avgMS.ToString("0.#") + "ms, ";
            displayText += fps.ToString("0.#") + " fps \n";

            // the column titles for the individual recordings:
            displayText += "Total".PadRight(colWidth);
            displayText += "MS/frame".PadRight(colWidth);
            displayText += "Calls/fra".PadRight(colWidth);
            displayText += "MS/call".PadRight(colWidth);
            displayText += "Label";
            displayText += "\n";

            // now we loop through each individual recording
            foreach (var entry in recordings)
            {
                // Each "entry" is a key-value pair where the string ID
                // is the key, and the recording instance is the value:
                ProfilerRecording recording = entry.Value;

                // calculate the statistics for this recording:
                long recordedMS = recording.MilliSeconds;
                float percent = recordedMS / totalMS;
                float msPerFrame = recordedMS / numCallCount;
                float msPerCall = recordedMS / recording.Count;
                float timesPerFrame = recording.Count / (float)numCallCount;

                // add the stats to the display text
                displayText += (percent.ToString("0.000") + "%").PadRight(colWidth);
                displayText += (msPerFrame.ToString("0.000") + "ms").PadRight(colWidth);
                displayText += (timesPerFrame.ToString("0.000")).PadRight(colWidth);
                displayText += (msPerCall.ToString("0.0000") + "ms").PadRight(colWidth);
                displayText += (recording.id);
                displayText += "\n";

                // and reset the recording
                recording.Reset();
            }
            //Debug.Log(displayText);
            OutputProfileResult(displayText);

            // reset & schedule the next time to display results:
            numCallCount = 1;

            //startTimeMS = Time.time;  //DELETE
            //nextOutputTimeMS = Time.time + 5;

        }

        public Profiler()
        {
            Init();
        }
    }
}
