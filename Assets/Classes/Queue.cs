using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Assets.Classes
{
    public class Queue
    {
        StaffMember[] staffList = new StaffMember[1];  // the list of staff allocated to the post
        Customer[] servingSlots = new Customer[1];              // the customers currently being served
        List<Customer> theQueue = new List<Customer>();         // the actual queue of customers

        Thread[] staffThreads = new Thread[1];         // one thread per staff member on the post


        Customer NextCustomerPlease()
        {
            // if there is at lesat 1 customer in the queue...
            if (theQueue.Count > 0)
            {
                // get the first customer in the queue
                Customer c = theQueue[0];
                theQueue.RemoveAt(0);

                return c;
            }

            return null;

        }

        void StaffMemberAssigned(StaffMember sm, int pos)
        {
            // add the staff member to the staff list
            staffList[pos] = sm;
            Customer[] copy = servingSlots;

            // update the number of slots available to serve customers in
            servingSlots = new Customer[copy.Length + 1];

            // copy the original values in
            for (int i = 0; i < copy.Length; i++)
            {
                servingSlots[i] = copy[i];
            }

            // add a new thread - how to do the '2' thing ????
            Thread thr3 = new Thread(() => StaffThreadMethod(pos, sm.GetAttributeByIndex(0)));        // the first parameter is the slot, the second is the speed

        }

        void StaffThreadMethod(int index, int speed)
        {
            while (true)
            {
                // release the person from the holding block
                if (servingSlots[index] != null)
                {
                    // Console.WriteLine("CUSTOMER " + servingSlots[index].GetIndex() + " has been served by server " + staffList[index].GetStaffname());
                    servingSlots[index].inQueue = false;
                    servingSlots[index].goingToSeats = true;
                }

                servingSlots[index] = NextCustomerPlease();

                // get someone else from the queue

                Random r = new Random();
                int rand = r.Next(70, 140);
                float multiplier = (float)rand / 100f;

                int sleepTime = (int)(multiplier * (4750 - (1250 * (speed - 1))));

                Thread.Sleep(sleepTime);


            }

        }

        void AddCustomer(Customer c)
        {
            theQueue.Add(c);
        }

        void Begin()
        {
            // start all the threads
            for (int i = 0; i < staffThreads.Length; i++)
            {
                staffThreads[i].Start();
            }
        }

        void Upgrade()
        {
            staffList = new StaffMember[staffList.Length+1];
            staffThreads = new Thread[staffList.Length + 1];
        }

    }
}
