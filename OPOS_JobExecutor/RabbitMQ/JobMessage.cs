﻿using OPOS_Consumer.RabbitMQ;
using System.Drawing;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace OPOS_project.Scheduler
{
    public enum JobType
    {
        Blur,
        Sharpen,
        DetectEdges,
        Embossing,
    }

    
    public class JobMessage
    {
        public string Name { get; private set; }
        public JobType? JobType { get; private set; }
        public DateTime? StartDateAndTime { get; private set; }

        public DateTime? Deadline { get; private set; }
        public int? TotalExecutionTime { get; private set; }
       /* [JsonConverter(typeof(BitmapConverter))]
        public Bitmap? Image { get; set; }
       */
       public String BitmapPath { get; private set; }   
        public static String json { get; set; }

        public JobMessage(string Name, JobType? jobType, String BitmapPath,
            DateTime? startDateAndTime = null, DateTime? Deadline = null, int? TotalExecutionTime = null)
        {
            this.Name = Name;
            this.JobType = jobType;
            this.BitmapPath=BitmapPath;
            this.StartDateAndTime = startDateAndTime;
            this.Deadline = Deadline;
            this.TotalExecutionTime = TotalExecutionTime;
        }

    }
}