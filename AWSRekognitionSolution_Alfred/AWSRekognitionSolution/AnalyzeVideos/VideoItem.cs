using System.Collections.Generic;
using Amazon.Rekognition.Model;

namespace AnalyzeVideos
{
    class VideoItem
    {
        public string FileName { get; set; }
        public string JobId { get; set; }
        public bool AnalysisComplete { get; set; }
        public bool AnalysisSucceeded { get; set; }
        public List<LabelDetection> Labels { get; set; }

        public VideoItem(string fileName)
        {
            this.FileName = fileName;
        }
    }
}
