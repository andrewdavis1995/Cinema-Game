using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Classes
{
    [Serializable]
    public class SaveableStaff
    {
        public int index;
        public int currentJob = 0;     // 0 = idle, 1 = tickets, 2 = food
        public int dayHired;
        public string name;
        public int transformID;
        public int[] attributes = new int[4];

        float xPos = 0;
        float yPos = 0;

        public SaveableStaff(StaffMember s)
        {
            this.index = s.getIndex();
            this.currentJob = s.getJobID();
            this.dayHired = s.GetStartDay();
            this.name = s.GetStaffname();
            this.attributes = s.GetAttributes();
            this.transformID = s.GetTransformID();
        }

    }
}
