using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;

namespace Assets.Classes
{
    public class CustomerQueue
    {
        StaffMember[] staffList = new StaffMember[1];  // the list of staff allocated to the post
        Customer[] servingSlots = new Customer[1];              // the customers currently being served
        List<Customer> theQueue = new List<Customer>();         // the actual queue of customers
        int sortingOrderOnFinish = 11;      // the sorting order to move to after a customer is fdone with the queue
        float queuePosX = 0;        // the x coordinate of the queue start position
        float queuePosY = 0;        // the y coordinate of the queue start position 
        string queueType = "";

        Thread[] staffThreads = new Thread[1];         // one thread per staff member on the post
        Thread patienceThread;        // the first parameter is the slot, the second is the speed

        bool running = false;         // whether or not the day is running

        // CONSTRUCTOR for the class
        public CustomerQueue(int sO, float x, float y, string type)
        {
            sortingOrderOnFinish = sO;
            queuePosX = x;
            queuePosY = y;
            queueType = type;
        }

        /// <summary>
        /// Get the next customer from the queue
        /// </summary>
        /// <param name="index">The serving slot which is requesting the next customers</param>
        /// <returns>A Customer object - the head of the queue</returns>
        Customer NextCustomerPlease(int index)
        {
            // if there is at least 1 customer in the queue...
            if (theQueue.Count > 0)
            {
                // get the first customer from the queue
                Customer c = theQueue[0];
                theQueue.RemoveAt(0);

                // move the rest of the queue up
                for (int i = 0; i < theQueue.Count; i++)
                {
                    theQueue[i].shouldMoveUp++;
                }

                // set the status variables of the Customer object
                c.servingPositionX = queuePosX; 
                c.servingPositionY = queuePosY + 2;
                c.moveToServingSlot = index;

                return c;
            }
            
            // no customers in the queue
            return null;

        }
        
        /// <summary>
        /// When a staff member is dropped in one of the Slots for the post
        /// </summary>
        /// <param name="sm">The staff member to assign</param>
        /// <param name="pos">Which slot to assign to</param>
        public void StaffMemberAssigned(StaffMember sm, int pos)
        {
            // add the staff member to the staff list
            staffList[pos] = sm;
            Customer[] copy = servingSlots;

            // update the number of slots available to serve customers in
            servingSlots[pos] = null;

            // copy the original values in
            for (int i = 0; i < copy.Length; i++)
            {
                servingSlots[i] = copy[i];
            }

            // add a new thread

            int index = 0;

            if (queueType.Equals("Food"))
            {
                index = 1;
            }

            Thread thr = new Thread(() => StaffThreadMethod(pos, sm.GetAttributeByIndex(index)));        // the first parameter is the slot, the second is the speed
            staffThreads[pos] = thr;

            // if the day is already running, start the thread
            if (running)
            {
                staffThreads[pos].Start();
            }

        }

        /// <summary>
        /// When a staff member is dragged out of one of the slots for the post
        /// </summary>
        /// <param name="sm">The Staff Member to remove</param>
        /// <param name="pos">The position slot number to remove the staff member from</param>
        public void StaffMemberRemoved(StaffMember sm, int pos)
        {
            try
            {
                // clear the staff list at pos
                staffList[pos] = null;

                // clear the thread list at pos
                if (staffThreads[pos] != null && staffThreads[pos].IsAlive)
                {
                    staffThreads[pos].Abort();
                }

                staffThreads[pos] = null;

                // possibly put customer back to queue (if someone in serving slots)
                if (servingSlots[pos] != null)
                {
                    servingSlots[pos].inQueue = true;
                    theQueue.Insert(0, servingSlots[pos]);
                    servingSlots[pos] = new Customer(null, -1, null, null);

                    // move customer back into queue
                    theQueue[0].transform.position = new UnityEngine.Vector3(queuePosX, queuePosY, 0);

                    // move queue back
                    for (int i = 1; i < theQueue.Count; i++)
                    {
                        theQueue[i].transform.Translate(0, -0.8f, 0);
                        theQueue[i].transform.GetComponent<SpriteRenderer>().sortingOrder++;
                    }
                }
            }
            catch (Exception) { }
        }
        
        /// <summary>
        /// The method which is run by staff members - one thread per staff member
        /// </summary>
        /// <param name="index">The serving slot number</param>
        /// <param name="speed">The relevant speed for the Staff Member - effects the wait times</param>
        public void StaffThreadMethod(int index, int speed)
        {
            // loop forever
            while (true)
            {
                // if the person being served is not null (i.e. there is someone there)...
                if (servingSlots[index] != null)
                {
                    // sort the customers position if they are in the food queue (queue positioning issue)
                    if (queueType.Equals("Food"))
                    {
                        servingSlots[index].sortFoodQueuePos = true;
                    }

                    // update status variables
                    servingSlots[index].inQueue = false;
                    servingSlots[index].AddPatience(50);
                    
                }

                // get another customer from the queue
                servingSlots[index] = NextCustomerPlease(index);

                // generate a random value - this will effect the wait times - making the game look less 'systematic'
                System.Random r = new System.Random();
                int rand = r.Next(90, 120);
                float multiplier = (float)rand / 100f;

                int sleepTime = (int)(multiplier * (4400 - (1000 * (speed - 1))));

                // make the thread sleep for the necessary amount of time
                Thread.Sleep(sleepTime);
                
            }

        }

        /// <summary>
        /// Add a customer to the back of the queue
        /// </summary>
        /// <param name="c">The customer to be added</param>
        public void AddCustomer(Customer c)
        {
            // add the customer to the queue
            theQueue.Add(c);

            // sort the visual elements of the customer - sorting order etc
            c.transform.GetComponent<SpriteRenderer>().sortingLayerName = "Queue";
            c.transform.GetComponent<SpriteRenderer>().sortingOrder = theQueue.Count;
            c.MovementVector = new Vector2(0, 0);

            // loop through all slots
            for (int i = 0; i < servingSlots.Length; i++)
            {
                // if the serving slot is currently empty and there is a staff member assigned to that slot, immediately move the customer to that slot
                if (servingSlots[i] == null && staffList[i] != null)
                {
                    servingSlots[i] = c;
                    theQueue.RemoveAt(0);       // move the customer from the queue to the serving slot
                    c.servingPositionX = queuePosX;
                    c.servingPositionY = queuePosY + 2;
                    c.moveToServingSlot = i;
                    break;
                }
            }
        }

        /// <summary>
        /// The method which is called from the patience thread - decreaes the patience systematically
        /// </summary>
        void DecreasePatience()
        {
            // loop forever
            while (true)
            {
                // for all customers in the queue
                for (int i = 0; i < theQueue.Count; i++)
                {
                    #region Make people at front of queue get bored less easily
                    int value = 6;

                    if (i < 3)
                    {
                        value -= (5 - (2 * i));
                    }

                    if (i == 0)
                    {
                        value += 2;
                    }
                    #endregion

                    // decrease the patience values of each customer in the queue
                    theQueue[i].DecreasePatience(value);

                    // check if patience has run out
                    if (theQueue[i].GetPatience() < 1)
                    {
                        // walk out
                        theQueue[i].walkingAway = true;

                        // move customers after the current one up in the queue - to fill the gap left
                        for (int j = i + 1; j < theQueue.Count; j++)
                        {
                            theQueue[j].shouldMoveUp++;
                        }

                        // remove the customer from the queue
                        theQueue.RemoveAt(i);
                    }
                }

                // sleep the thread - delay
                Thread.Sleep(200);

            }
        }

        /// <summary>
        /// Start the staff threads and the patience thread
        /// </summary>
        public void Begin()
        {
            // status variable
            running = true;

            // start all the threads
            for (int i = 0; i < staffThreads.Length; i++)
            {
                if (staffThreads[i] != null)
                {
                    staffThreads[i].Start();
                }
            }

            patienceThread = new Thread(new ThreadStart(DecreasePatience));
            patienceThread.Start();

        }

        /// <summary>
        /// End all threads at the end of the day
        /// </summary>
        public void End()
        {
            // update the status variables
            running = false;

            // end all the staff threads
            for (int i = 0; i < staffThreads.Length; i++)
            {
                if (staffThreads[i] != null)
                {
                    staffThreads[i].Abort(); 
                }

                // clear the serving slot - dummy value
                servingSlots[i] = new Customer(null, -1, null, null);

            }

            staffList = new StaffMember[staffList.Length];
            staffThreads = new Thread[staffThreads.Length];
            patienceThread = null;
        }

        /// <summary>
        /// Clear the queue - remove all Customers
        /// </summary>
        public void Clear()
        {
            theQueue.Clear();
        }

        /// <summary>
        /// The table for the queue has been upgraded - more slots available
        /// </summary>
        public void Upgrade()
        {
            // store a copy of the existing staff who are assigned to the queue
            StaffMember[] sl = staffList;
            Thread[] st = staffThreads;

            // resize the arrays
            staffList = new StaffMember[staffList.Length + 1];
            staffThreads = new Thread[staffThreads.Length + 1];
            servingSlots = new Customer[servingSlots.Length + 1];

            // write the previously assigned staff back in to the new arrays
            for (int i = 0; i < sl.Length; i++)
            {
                staffList[i] = sl[i];
                staffThreads[i] = st[i];
            }

        }

        /// <summary>
        /// Get the number of Customers in the queue
        /// </summary>
        /// <returns>Number of Customers in the queue</returns>
        public int GetQueueSize()
        {
            return theQueue.Count;
        }

        /// <summary>
        /// Pause all threads while the application is paused
        /// </summary>
        public void Pause()
        {
            if (patienceThread != null && patienceThread.IsAlive)
            {
                patienceThread.Suspend();
            }

            for (int i = 0; i < staffThreads.Length; i++)
            {
                if (staffThreads[i] != null && staffThreads[i].IsAlive)
                {
                    staffThreads[i].Suspend();
                }
            }
        }

        /// <summary>
        /// Resume all threads when the application resumes
        /// </summary>
        public void Resume()
        {
            if (patienceThread != null)
            {
                patienceThread.Resume();
            }

            for (int i = 0; i < staffThreads.Length; i++)
            {
                if (staffThreads[i] != null)
                {
                    staffThreads[i].Resume();
                }
            }
        }

    }
}
