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
        int index;      // the staff member's id number
        int currentJob = 0;     // the post where the staff member is situated (0 = unassigned, 1 = tickets, 2 = food)
        int dayHired;       // which day they were hired on
        string name;        // the staff member's name
        Transform transform;    // the visual representation of the staff member
        int transformID;    // which transform they are (0 -> 4)

        Color[] colours = new Color[3];     // the colours of hair, shirt and skin

        int[] attributes = new int[4];      // the staff member's attribute array

        // position vairables (for moving the camera to view the staff member)
        float xPos = 0;
        float yPos = 0;

        /// <summary>
        /// CONSTRUCTOR
        /// </summary>
        /// <param name="i">id for the staff member</param>
        /// <param name="n">Their name</param>
        /// <param name="t">The transform to use</param>
        /// <param name="dH">The day they were hired</param>
        /// <param name="tID">the id of the transform to use (which appearance)</param>
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

        #region Mutators

        /// <summary>
        /// Set the colours of the components
        /// </summary>
        /// <param name="col"></param>
        public void SetColours(Color[] col)
        {
            colours = col;
        }

        /// <summary>
        /// If the colour of the uniform is changed
        /// </summary>
        /// <param name="c"></param>
        public void UniformChanged(Color c)
        {
            colours[0] = c;
        }

        /// <summary>
        /// Set the attributes for the Staff Member
        /// </summary>
        /// <param name="att">The attriubte array ([0] = Tickets, [1] = Food, [2] = Friendliness, [3] = Clarity</param>
        public void SetAttributes(int[] att)
        {
            attributes = att;
        }

        /// <summary>
        /// Update the staff member's name
        /// </summary>
        /// <param name="newName">The new name for the staff member</param>
        public void UpdateName(string newName)
        {
            this.name = newName;
        }

        /// <summary>
        /// Set the tranform element
        /// </summary>
        /// <param name="t">The transform to use</param>
        public void SetTransform(Transform t)
        {
            transform = t;
        }

        /// <summary>
        /// Set the job for the staff member
        /// </summary>
        /// <param name="jobID"></param>
        public void SetJob(int jobID)
        {
            currentJob = jobID;
        }

        /// <summary>
        /// set the position vector of the staff member
        /// </summary>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        public void SetVector(float x, float y)
        {
            this.xPos = x; this.yPos = y;
        }

        #endregion

        #region Accessors
        public int GetTransformID()
        {
            return this.transformID;
        }

        public Color GetColourByIndex(int index)
        {
            return colours[index];
        }

        public Transform GetTransform()
        {
            return transform;
        }

        public int GetIndex() { return this.index; }

        public int GetJobID()
        {
            return currentJob;
        }

        public string GetStaffname()
        {
            return name;
        }
        
        public int GetAttributeByIndex(int index)
        {
            return this.attributes[index];
        }

        public int GetStartDay()
        {
            return this.dayHired;
        }

        public Vector3 GetVector() {
            return new Vector3(xPos, yPos, 0);
        }

        public int[] GetAttributes()
        {
            return attributes;
        }
        #endregion

        /// <summary>
        /// Upgrade one of the attributes
        /// </summary>
        /// <param name="index">The index of which attribute to upgrade (0 = Tickets, 1 = Food, 2 = Friendliness, 3 = Clarity)</param>
        public void Upgrade(int index)
        {
            attributes[index]++;
        }

        

    }
}
