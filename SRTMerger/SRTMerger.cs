using System;
using System.IO;
using System.Linq;

namespace SRTMerger
{
    public class SRTMerger
    {
        public static void Main(string[] args)
        {
            if(args.Length!=3)
            {
                Console.WriteLine("Usage:file1 file2 mergeFileName");
                return;
            }
            using (var mergedFile= File.CreateText(args[2]))
            {
                SubtitlePacket lastPacket = WritetillLast(args[0],mergedFile);
                WritetillLast(args[1], mergedFile, packet => packet.MoveBy(lastPacket));
                mergedFile.Flush();
            }
        }

        private static SubtitlePacket WritetillLast(string filePath, StreamWriter fileStream,params Func<SubtitlePacket,SubtitlePacket>[] mapper)
        {
            SubtitlePacket lastPacket = null;
            using (var firstFile = File.OpenText(filePath))
            {
                for (SubtitlePacket subtitlePacket = SubtitlePacket.ReadFromFile(firstFile);
                     subtitlePacket != null;
                     lastPacket = subtitlePacket, subtitlePacket = SubtitlePacket.ReadFromFile(firstFile))
                {
                    mapper.Aggregate(subtitlePacket, (current, map) => map(current)).WriteToFile(fileStream); 
                }
            }
            return lastPacket;
        }

    }
}
