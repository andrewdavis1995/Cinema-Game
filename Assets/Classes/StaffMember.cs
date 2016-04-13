using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Classes
{

    [Serializable]
    public class StaffMember
    {
        int index;
        int currentJob = 0;     // 0 = idle, 1 = tickets, 2 = food
        int dayHired;
        string name;
        Transform transform;
        int transformID;

        int[] attributes = new int[4];

        float xPos = 0;
        float yPos = 0;

        public StaffMember(int i, string n, Transform t, int dH, int tID)
        {
            index = i;
            name = n;
            transform = t;
            dayHired = dH;
            transformID = tID;
            attributes[0] = 1;
            attributes[1] = 1;
            attributes[2] = 1;
            attributes[3] = 1;

        }

        public void SetAttributes(int[] att)
        {
            attributes = att;
        }

        public void SetTransform(Transform t)
        {
            transform = t;
        }

        public int GetTransformID()
        {
            return this.transformID;
        }

        public void Upgrade(int index)
        {
            attributes[index]++;
        }

        public int[] GetAttributes()
        {
            return attributes;
        }

        public int GetAttributeByIndex(int index)
        {
            return this.attributes[index];
        }

        public int GetStartDay()
        {
            return this.dayHired;
        }

        public Vector3 GetVector() { return new Vector3(xPos, yPos, 0); }
        public void SetVector(float x, float y) { this.xPos = x; this.yPos = y; }

        public string GetStaffname()
        {
            return name;
        }

        public void UpdateName(string newName)
        {
            this.name = newName;
        }

        public Transform GetTransform()
        {
            return transform;
        }
       
        public int GetIndex() { return this.index; }

        public void SetJob(int jobID)
        {
            currentJob = jobID;
        }
        public int GetJobID()
        {
            return currentJob;
        }
       

    }
}
