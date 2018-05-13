using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZMS3Sync
{
    
    public class RateLimiter
    {

    


        public static Dictionary<string, int> Limits = new Dictionary<string, int>();
        public static Dictionary<string, Dictionary<string, int>> Counters = new Dictionary<string, Dictionary<string, int>>();

        private int LimitPerMinute { get; set; }
        public RateLimiter()
        {
        }

        public bool checkRate(string LimitName)
        {
            addHit(LimitName);

            if(!Limits.ContainsKey(LimitName))
            {
                return true;
            }

            var limit = Limits[LimitName];
            var timeSlot = getTimeSlot();

            Dictionary<string, int> hc;

            lock (Counters)
            {
                if (!Counters.ContainsKey(LimitName))
                {
                    return true;

                }
                else
                {
                    hc = Counters[LimitName];
                }
            }

            if(!hc.ContainsKey(timeSlot))
            {
                return true;
            }
            return hc[timeSlot] <= limit;



        }
        public void addLimit(string LimitName, int countPerMinute)
        {

            lock (Limits)
            {
                if (Limits.ContainsKey(LimitName))
                {
                    Limits[LimitName] = countPerMinute;
                }
                else
                {
                    Limits.Add(LimitName, countPerMinute);
                }

            }

        }

        private string getTimeSlot()
        {
            
            //f: Sunday, June 15, 2008 9:15 PM
            return DateTime.Now.ToString("yyyy-MM-dd H:mm");

        }
        private void cleanup()
        {

            foreach(var k in Counters.Keys.ToList())
            {
                try
                {
                    var cntr = Counters[k];
                    foreach (var dtStr in cntr.Keys.ToList())
                    {
                        var dt = DateTime.Parse(dtStr);
                        if(DateTime.Now.AddMinutes(-5)>dt)
                        {
                            cntr.Remove(dtStr);
                        }

                    }
                }
                catch(Exception e)
                {
                    U.log("Error cleaning up old counters", e);
                    Counters.Remove(k);
                }



            }


        }

        private void addHit(string LimitName)
        {

            if(DateTime.Now.Ticks % 50 == 0)
            {
                Task.Factory.StartNew(() => { cleanup(); });

            }

            Dictionary<string, int> hc;

            lock (Counters)
            {
                if (!Counters.ContainsKey(LimitName))
                {
                     hc = new Dictionary<string, int>();

                    Counters.Add(LimitName, hc);

                }
                else
                {
                    hc = Counters[LimitName];
                }
            }


            var timeSlot = getTimeSlot();

            if(hc.ContainsKey(timeSlot))
            {
                hc[timeSlot]++;
            }
            else
            {
                hc.Add(timeSlot, 1);
            }
        }

    }
}
