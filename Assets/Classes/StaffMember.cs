using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{

    [System.Serializable]
    public class StaffMember
    {
        int index;
        int currentJob = 0;     // 0 = idle, 1 = tickets, 2 = food

        public StaffMember(int i)
        {
            index = i;
        }

        public int getIndex() { return this.index; }

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
