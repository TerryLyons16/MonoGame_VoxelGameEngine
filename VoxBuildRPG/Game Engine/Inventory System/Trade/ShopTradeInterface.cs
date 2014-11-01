using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoxelRPGGame.GameEngine.InventorySystem.Trade
{
    public class ShopTradeInterface:TradeInterface
    {


        public ShopTradeInterface(/*ITradeInventory shop, ITradeInventory customer*/)/*:base(shop,customer)*/
        {

        }

        protected override void Trade(ITradeInventory shop, ITradeInventory customer, ITradeableItem item, int desiredQuantity, float unitPrice)
        {
            int availableQuantity = 0;

            if (shop.Items.Contains(item.InventoryItem))//Only do the trade if the item is in the shop's inventory
            {

                //1. Get the actual available qunatity
                if (item.InventoryItem.Stock >= desiredQuantity)
                {
                    availableQuantity = desiredQuantity;
                }
                else
                {
                    availableQuantity = item.InventoryItem.Stock;
                }
                //2. From the qunatity available, get the amount the buyer can carry
                int quantityBuyerCanCarry = customer.QuantityCanAdd(item.InventoryItem, availableQuantity);
          
                float totalBuyPrice = unitPrice * quantityBuyerCanCarry;

                int quantityBuyerCanAfford = 0;
                //3. From the amount the buyer can carry, get the amount the buyer can afford
                if (customer.Currency >= totalBuyPrice)
                {
                    quantityBuyerCanAfford = quantityBuyerCanCarry;
                }
                else
                {
                    quantityBuyerCanAfford = (int)(customer.Currency / unitPrice);
                }

                float actualBuyPrice = unitPrice * quantityBuyerCanAfford;

                if (customer.Currency >= actualBuyPrice && quantityBuyerCanCarry>0)
                {
                    customer.Currency -= totalBuyPrice;
                    shop.Currency += totalBuyPrice;
                    InventoryItem buyerItem = item.InventoryItem.SplitStack(quantityBuyerCanAfford);
                    customer.AddItem(buyerItem);
                }
            }

        }
        public override void Buy(ITradeInventory seller, ITradeInventory buyer, ITradeableItem item, int desiredQuantity)
        {
            //Note: Buy and Sell will both call Trade with different parameters
            //i.e:
            Trade(seller, buyer, item, desiredQuantity, item.CustomerBuyPrice);
        }
        public override void Sell(ITradeInventory seller, ITradeInventory buyer, ITradeableItem item, int desiredQuantity)
        {
            Trade(seller, buyer, item, desiredQuantity, item.CustomerSellPrice);
        }
    }
}
