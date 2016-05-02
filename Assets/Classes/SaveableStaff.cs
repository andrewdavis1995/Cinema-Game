using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

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
        public float[,] colourArrays = new float[3, 3];     // the colours for appearance
        public int hairStyleID;         // which hair style to use
        public int extrasID;            // which extra option was selected

        // CONSTRUCTOR for the class
        public SaveableStaff(StaffMember s)
        {
            this.index = s.GetIndex();
            this.currentJob = s.GetJobID();
            this.dayHired = s.GetStartDay();
            this.name = s.GetStaffname();
            this.attributes = s.GetAttributes();
            this.transformID = s.GetTransformID();

            Color[] c = s.GetAllColours();

            // set colours
            for (int i = 0; i < 3; i++) {
                colourArrays[i,0] = c[i].r;
                colourArrays[i,1] = c[i].g;
                colourArrays[i,2] = c[i].b;
            }

            hairStyleID = s.GetHairID();
            extrasID = s.GetExtrasID();
        }

    }
}
