using Amazon.Rekognition.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalyzeVideos
{
    class VideoEntity
    {
        public string Name { get; set; }
        public string ID { get; set; }
        public bool IsAnalyzed { get; set; }
        public List<LabelDetection> Labels { get; set; }
    }
}
