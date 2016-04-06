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

        Thread[] staffThreads = new Thread[1];         // one thread per staff member on the post
        Thread patienceThread;        // the first parameter is the slot, the second is the speed


        bool running = false;                          // whether or not the day is running

        Customer NextCustomerPlease(int index)
        {
            // if there is at lesat 1 customer in the queue...
            if (theQueue.Count > 0)
            {
                // get the first customer in the queue
                Customer c = theQueue[0];
                theQueue.RemoveAt(0);

                for (int i = 0; i < theQueue.Count; i++)
                {
                    theQueue[i].shouldMoveUp++;
                }
                c.moveToServingSlot = index;

                return c;
            }

            for (int i = 0; i < theQueue.Count; i++)
            {
                theQueue[i].shouldMoveUp++;
            }

            return null;

        }

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
            Thread thr = new Thread(() => StaffThreadMethod(pos, sm.GetAttributeByIndex(0)));        // the first parameter is the slot, the second is the speed
            staffThreads[pos] = thr;

            // if the day is already running, start the thread
            if (running)
            {
                staffThreads[pos].Start();
            }

        }

        public void StaffMemberRemoved(StaffMember sm, int pos)
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
                theQueue.Insert(0, servingSlots[pos]);
                servingSlots[pos] = new Customer(null, -1, null, null);

                // move customer back into queue
                theQueue[0].transform.position = new UnityEngine.Vector3(38.5f, 9 * 0.8f, 0);

                // move queue back
                for (int i = 1; i < theQueue.Count; i++)
                {
                    theQueue[i].transform.Translate(0, -0.8f, 0);
                    theQueue[i].transform.GetComponent<SpriteRenderer>().sortingOrder++;
                }
            }

        }
        
        public void StaffThreadMethod(int index, int speed)
        {
            while (true)
            {
                
                // release the person from the holding block
                if (servingSlots[index] != null)
                {
                    // Console.WriteLine("CUSTOMER " + servingSlots[index].GetIndex() + " has been served by server " + staffList[index].GetStaffname());

                    servingSlots[index].goingToSeats = true;
                    servingSlots[index].inQueue = false;
                    
                    //servingSlots[index].doneWithQueue();
                }

                servingSlots[index] = NextCustomerPlease(index);

                // get someone else from the queue

                System.Random r = new System.Random();
                int rand = r.Next(90, 120);

                float multiplier = (float)rand / 100f;

                int sleepTime = (int)(multiplier * (4400 - (1000 * (speed - 1))));

                Thread.Sleep(sleepTime);
                
            }

        }

        public void AddCustomer(Customer c)
        {
            theQueue.Add(c);
            c.transform.GetComponent<SpriteRenderer>().sortingLayerName = "Queue";
            c.transform.GetComponent<SpriteRenderer>().sortingOrder = theQueue.Count;

            for (int i = 0; i < servingSlots.Length; i++)
            {
                if (servingSlots[i] == null && staffList[i] != null)
                {
                    servingSlots[i] = c;
                    theQueue.RemoveAt(0);
                    c.moveToServingSlot = i;
                    break;
                }
            }
        }

        void DecreasePatience()
        {
            // loop forever
            while (true)
            {
                for (int i = 0; i < theQueue.Count; i++)
                {
                    int value = 6;

                    if (i < 6)
                    {
                        value -= (5 - i);
                    }

                    theQueue[i].DecreasePatience(value);

                    // check if < 0
                    if (theQueue[i].GetPatience() < 1)
                    {
                        // walk out
                        theQueue[i].walkingAway = true;
                        for (int j = i + 1; j < theQueue.Count; j++)
                        {
                            theQueue[j].shouldMoveUp++;
                        }
                        theQueue.RemoveAt(i);
                    }
                }

                Thread.Sleep(200);

            }
        }

        public void Begin()
        {
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

        public void End()
        {
            running = false;

            for (int i = 0; i < staffThreads.Length; i++)
            {
                if (staffThreads[i] != null)
                {
                    staffThreads[i].Abort(); 
                }

                servingSlots[i] = new Customer(null, -1, null, null);

            }

            staffList = new StaffMember[staffList.Length];
            staffThreads = new Thread[staffThreads.Length];
            patienceThread = null;
        }

        public void Clear()
        {
            theQueue.Clear();
        }

        public void Upgrade()
        {
            staffList = new StaffMember[staffList.Length+1];
            staffThreads = new Thread[staffThreads.Length + 1];
            servingSlots = new Customer[servingSlots.Length + 1];
        }

        public int GetQueueSize()
        {
            return theQueue.Count;
        }

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
