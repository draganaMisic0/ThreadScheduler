using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPOS_project.Scheduler
{
    internal interface IStatefulJob
    {
        public void checkState();
    }
}
