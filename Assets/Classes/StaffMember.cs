using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{

    [System.Serializable]
    public class StaffMember
    {
        int index;
        int currentJob = 0;     // 0 = idle, 1 = tickets, 2 = food
        string name;
        Transform transform;
        int upgradeLevel = 1;

        float xPos = 0;
        float yPos = 0;

        public StaffMember(int i, string n, Transform t)
        {
            index = i;
            name = n;
            transform = t;
        }

        public Vector3 GetVector() { return new Vector3(xPos, yPos, 0); }
        public void SetVector(float x, float y) { this.xPos = x; this.yPos = y; }

        public string GetStaffname()
        {
            return name;
        }

        public void Upgrade()
        {
            upgradeLevel++;
        }

        public Transform getTransform()
        {
            return transform;
        }

        public int GetUpgradeLevel()
        {
            return this.upgradeLevel;
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
