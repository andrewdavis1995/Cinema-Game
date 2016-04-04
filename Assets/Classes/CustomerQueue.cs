using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

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
                    theQueue[i].shouldMoveUp = true;
                }
                c.moveToServingSlot = index;

                return c;
            }

            for (int i = 0; i < theQueue.Count; i++)
            {
                theQueue[i].shouldMoveUp = true;
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

        public bool addingInProgress = false;

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
                    servingSlots[index].SetTravellingTo(38.5f + (3 * servingSlots[index].servingSlot), 11 * 0.8f);
                }

                servingSlots[index] = NextCustomerPlease(index);

                    // get someone else from the queue

                Random r = new Random();
                int rand = r.Next(70, 140);
                float multiplier = (float)rand / 100f;

                int sleepTime = (int)(multiplier * (4750 - (1250 * (speed - 1))));

                Thread.Sleep(sleepTime);
                
            }

        }

        public void AddCustomer(Customer c)
        {
            //while (addingInProgress) { }

            //addingInProgress = true;
            theQueue.Add(c);

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

            //addingInProgress = false;
        }

        void DecreasePatience()
        {
            // loop forever
            while (true)
            {
                for (int i = 0; i < theQueue.Count; i++)
                {
                    theQueue[i].DecreasePatience(5);

                    // check if < 0
                    if (theQueue[i].GetPatience() < 1)
                    {
                        // walk out
                        theQueue[i].walkingAway = true;
                        theQueue.RemoveAt(i);
                        for (int j = i; j < theQueue.Count; j++)
                        {
                            theQueue[j].shouldMoveUp = true;
                        }

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

    }
}
