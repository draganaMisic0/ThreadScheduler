using Xunit;
using OPOS_project.Scheduler;
using System;
using Newtonsoft.Json;
using System.Text;
using RabbitMQ.Client;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Moq;
using System.Threading.Channels;

public class SchedulerTests
{

    private readonly Mock<IModel> mockChannel;
    private readonly Mock<IConnection> mockConnection;
    private readonly MessageQueue messageQueue;
    [Fact]
    public void Test_MaxConcurrentJobs_LimitIsRespected()
    {
        // Arrange
        var scheduler=Scheduler.getInstance();

        // Mock or create jobs with JobFactory or JobMessage.
        JobFactory jf=new JobFactory ();

        string bitmapPath1 = @"../../../Resources/city.png";
        string bitmapPath2 = @"../../../Resources/hamster.png";
        string bitmapPath3 = @"../../../Resources/nature.png";

        string fullPath1 = Path.GetFullPath(bitmapPath1);
        string fullPath2 = Path.GetFullPath(bitmapPath2);
        string fullPath3 = Path.GetFullPath(bitmapPath3);

        JobMessage jobMessage4 = new JobMessage($"Sharpen_1", JobType.Sharpen, Path.GetFullPath(bitmapPath1),
         (DateTime.Now).AddSeconds(40), (DateTime.Now).AddMinutes(1), 10);


        Random random = new Random();
        JobMessage jobMessage1 = new JobMessage($"DetectEdges" + random.NextInt64(3000), JobType.DetectEdges, fullPath2);

        JobMessage jobMessage5 = new JobMessage($"Embossing_3", JobType.Embossing, Path.GetFullPath(bitmapPath3),
             (DateTime.Now).AddMinutes(1), (DateTime.Now).AddMinutes(2), 10);


        JobMessage jobMessage2 = new JobMessage($"Blur_" + random.NextInt64(3000), JobType.Blur, fullPath1);

        /* listOfJobs.Add(new JobMessage($"Embossing_5", JobType.Embossing, bitmapPath2,
             (DateTime.Now).AddSeconds(7), (DateTime.Now).AddMinutes(1), 10));
        */

        JobMessage jobMessage3 = new JobMessage($"Sharpen_" + random.NextInt64(3000), JobType.Sharpen, fullPath3);

        var job1 = JobFactory.createJob(jobMessage1);
        var job2 = JobFactory.createJob(jobMessage2);
        var job3 = JobFactory.createJob(jobMessage3);
        var job4 = JobFactory.createJob(jobMessage4);
        var job5 = JobFactory.createJob(jobMessage5);


        // Act
        scheduler.StartJob(job1);
        scheduler.StartJob(job2);
        scheduler.StartJob(job3); // This job should be queued.
        scheduler.StartJob(job4);
        scheduler.StartJob(job5);

        // Assert
        Assert.Equal(3, scheduler.currentNumOfRunningJobs); 
       // Assert.Contains(job3, scheduler.waitingJobs); // job3 should be in the queue.
    }

   
}
