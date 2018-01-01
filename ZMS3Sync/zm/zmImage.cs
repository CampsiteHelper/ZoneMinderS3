using System;
namespace ZMS3Sync
{
    public class zmImage
    {
        
        public zmImage(ZoneMinderEvent evnt, string basePath,long eventid)
        {
            fpath = buildFilePath(evnt, basePath);
            frameId = evnt.frameID;
            eventId = eventid;

            
        }


        public string fpath { get; set; }
        public long frameId { get; set; }
        public long eventId { get; set; }

        private string buildFilePath (ZoneMinderEvent evnt, string image_base_path)
        {
            
            var dtFrame = evnt.starttime;

            var frameId = evnt.frameID;

            var monitorName = evnt.monitorName;

            /* get a two digit year for the file path this will fail in the year 100000 */
            var tYear = (dtFrame.Year % 1000).ToString().PadLeft(2,'0');
            var tMonth = dtFrame.Month.ToString().PadLeft(2, '0');
            var tDay = dtFrame.Day.ToString().PadLeft(2, '0');
            var tHour = dtFrame.Hour.ToString().PadLeft(2, '0');
            var tMin = dtFrame.Minute.ToString().PadLeft(2, '0');
            var tSec = dtFrame.Second.ToString().PadLeft(2, '0');
            var tFrameId = frameId.ToString().PadLeft(5, '0');

            if(image_base_path.Substring(image_base_path.Length-1)!="/")
            {
                image_base_path += "/";
            }
                return image_base_path + monitorName +
                    "/" + tYear + "/" + tMonth +
                    "/" + tDay + "/" + tHour + "/" + tMin +
                "/" + tSec + "/" + tFrameId + "-capture.jpg";


        }

    }
}
