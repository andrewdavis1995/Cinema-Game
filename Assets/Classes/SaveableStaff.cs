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
        // Note: This class is needed because not everything in the StaffMember class is Serializable.
        //       So, for saving purposes, I write the staff details into a object of type SaveableStaff

        public int index;               // the staff index / id
        public int currentJob = 0;      // the job they are currently doing (0 = idle, 1 = tickets, 2 = food)
        public int dayHired;            // what day they were hired on
        public string name;             // their name
        public int transformID;         // the id of which appearance they had
        public int[] attributes = new int[4];   // their attribute ratings
        
        // CONSTRUCTOR for the class
        public SaveableStaff(StaffMember s)
        {
            this.index = s.GetIndex();
            this.currentJob = s.GetJobID();
            this.dayHired = s.GetStartDay();
            this.name = s.GetStaffname();
            this.attributes = s.GetAttributes();
            this.transformID = s.GetTransformID();
        }

    }
}
