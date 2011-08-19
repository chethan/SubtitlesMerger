using System;
using System.Collections.Generic;
using System.IO;

namespace SRTMerger
{
    public class SubtitlePacket
    {
        private const string TIME_SEPERATOR = " --> ";
        private const string TIME_FORMAT = "HH:mm:ss,fff";
        private readonly int subtitleNumber;
        private DateTime fromTime;
        private DateTime toTime;

        private readonly List<string> text;

        public SubtitlePacket(int subtitleNumber, DateTime fromTime, DateTime toTime, List<string> text)
        {
            this.subtitleNumber = subtitleNumber;
            this.fromTime = fromTime;
            this.toTime = toTime;
            this.text = text;
        }

        public void WriteToFile(TextWriter srtStream)
        {
            srtStream.WriteLine(subtitleNumber);
            srtStream.WriteLine(fromTime.ToString(TIME_FORMAT) + TIME_SEPERATOR + toTime.ToString(TIME_FORMAT));
            text.ForEach(srtStream.WriteLine);
            srtStream.WriteLine();
        }

        public SubtitlePacket MoveBy(SubtitlePacket lastPacket)
        {
            var time = lastPacket.toTime;
            var newFromTime = fromTime.AddHours(time.Hour).AddMinutes(time.Minute).AddSeconds(time.Second).AddMilliseconds(time.Millisecond);
            var newToTime = toTime.AddHours(time.Hour).AddMinutes(time.Minute).AddSeconds(time.Second).AddMilliseconds(time.Millisecond);
            return new SubtitlePacket(subtitleNumber+lastPacket.subtitleNumber,newFromTime, newToTime, text);
        }

        public static SubtitlePacket ReadFromFile(TextReader srtStream)
        {
            try
            {
                string firstLine;
                while ((firstLine=srtStream.ReadLine())==string.Empty){}
                int subTitleNumber = Convert.ToInt32(firstLine);
                string[] timeBoundries = srtStream.ReadLine().Split(TIME_SEPERATOR.ToCharArray(),StringSplitOptions.RemoveEmptyEntries);
                DateTime fromTime = DateTime.ParseExact(timeBoundries[0],TIME_FORMAT,null);
                DateTime toTime = DateTime.ParseExact(timeBoundries[1],TIME_FORMAT,null);
                var text=new List<string>();
                for (var line = srtStream.ReadLine(); !string.IsNullOrWhiteSpace(line);line=srtStream.ReadLine())
                {
                    text.Add(line);
                }
                return new SubtitlePacket(subTitleNumber, fromTime, toTime, text);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}