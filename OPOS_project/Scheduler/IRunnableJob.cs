using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPOS_project.Scheduler
{
   public interface IRunnableJob
    {
        public void IsJobInterrupted();
        public void Run();

    }
}
