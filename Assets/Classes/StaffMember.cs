using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    public class StaffMember
    {
        int index;
        int currentJob = 0;     // 0 = idle, 1 = tickets, 2 = food

        public StaffMember(int i)
        {
            index = i;
        }

        public void setJob(int jobID)
        {
            currentJob = jobID;
        }
        public int getJobID()
        {
            return currentJob;
        }
    }
}
