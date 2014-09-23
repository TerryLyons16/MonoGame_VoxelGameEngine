using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using VoxelRPGGame.GameEngine.Subsystems;

namespace VoxelRPGGame.GameEngine.Subsystems.Resources
{
    public abstract class AbstractResource
    {
        public delegate void ResourceAdded(AbstractResource resource,double amountGained);
        /// <summary>
        /// Event fired whenever the resource gains stock. Can be used for generating reward for agents
        /// who gain desired resources.
        /// </summary>
        public event ResourceAdded OnResourceAdded;

        public delegate void ResourceTaken(AbstractResource resource,double amountLost);
        /// <summary>
        /// Event fired whenever the resource loses stock. Can be used for generating reward for agents
        /// who consume/lose desired resources.
        /// </summary>
        public event ResourceTaken OnResourceTaken;

        protected double stock;

        protected bool hasMaxCapacity, hasMinCapacity;
        protected double maxCapacity, minCapacity;


       /// <summary>
        /// Adds quantity to resource stock. Prevents resource from going over capacity
        /// if it has a limit. Stock is rounded to 8 decimal points.
       /// </summary>
        /// <param name="amount">amount to add</param>
       /// <returns>double, amount of excess not added if resource has reached capacity</returns>
        public double Inflow(double amount)
        {

            double excessResource = 0;
            double resourceToAdd = amount;

            if (hasMaxCapacity)
            {
                if ((stock + resourceToAdd) > maxCapacity)
                {
                    resourceToAdd = maxCapacity - stock;
                    excessResource = amount - resourceToAdd;
                }
            }

            stock += resourceToAdd;
            stock = Math.Round(stock, 8);
            //Fire event if resource has gained stock
            if (resourceToAdd > 0)
            {
                if (OnResourceAdded != null)
                {
                    OnResourceAdded(this, resourceToAdd);
                }
            }


            return excessResource;
        }

        /// <summary>
        /// Takes qunatity from resource stock. If resource has a 
        /// minimum allowed amount, only takes as much resource as it can
        /// (e.g. if negative stock is not allowed).Stock is rounded to 8 decimal points.
        /// </summary>
        /// <param name="amountRequested">The amount of stock that to be removed</param>
        /// <returns>The amount of stock that allowed to be used</returns>
        public double Outflow(double amountRequested)
        {
            //NOTE: May cause issues with negative stock values
            double amountRetrieved = amountRequested;

            if (hasMinCapacity)
            {
                if ((stock - amountRequested) < minCapacity)
                {
                    amountRetrieved = stock - minCapacity;

                }
            }

            stock-=amountRetrieved;

            stock = Math.Round(stock, 8);



            if (amountRetrieved > 0)
            {
                if (OnResourceTaken != null)
                {
                    OnResourceTaken(this, amountRetrieved);
                }
            }


            return amountRetrieved;
        }
#region Properties
        /// <summary>
        /// Returns the amount of the resource
        /// </summary>
        public double Stock
        {
            get
            {
                return stock;
            }

        }


        public bool IsFull
        {
            get
            {
                bool result = false;

                if (hasMaxCapacity && stock == maxCapacity)
                {
                    result = true;
                }
                return result;
            }
        }

        public bool IsEmpty
        {
            get
            {
                bool result = false;

                if (hasMaxCapacity && stock == minCapacity)
                {
                    result = true;
                }
                return result;
            }

        }

#endregion
    }
}
