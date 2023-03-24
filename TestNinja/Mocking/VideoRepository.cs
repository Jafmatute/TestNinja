using System.Collections.Generic;
using System.Linq;

namespace TestNinja.Mocking
{
    public interface IVideoRepository
    {
        IEnumerable<Video> GetUnprocessedVideos();
    }

    public class VideoRepository : IVideoRepository
    {
        public IEnumerable<Video> GetUnprocessedVideos()
        {
            using (var ctx = new VideoContext())
            {
                var videos = 
                    (from video in ctx.Videos
                        where !video.IsProcessed
                        select video).ToList();

                return videos;
            }
        }
    }
}