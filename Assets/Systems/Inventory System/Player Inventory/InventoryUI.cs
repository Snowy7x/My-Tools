using System.Collections.Generic;
using UnityEngine;

namespace Snowy.Inventory
{
	public class InventoryUI : MonoBehaviour
	{
		[SerializeField] Player player;
		[SerializeField] InvItemUI itemPrefab;
		[SerializeField] List<InvItemUI> items = new List<InvItemUI>();
		
		public void Start()
		{
			player.Inventory.OnInventoryItemAdded += InventoryItemAdded;
			player.Inventory.OnInventoryItemRemoved += InventoryItemRemoved;
			player.Inventory.OnInventoryChanged += InventoryChanged;
			player.Inventory.OnCurrentSlotChanged += CurrentSlotChanged;
			
			for (var i = 0; i < player.Inventory.MaxSlots; i++)
			{
				var item = Instantiate(itemPrefab, transform);
				item.Initalize(player.Inventory.GetSlot(i), i);
				items.Add(item);
			}
		}

		private void InventoryChanged(InventorySlot slot)
		{
			items[slot.index].Update();
		}

		private void CurrentSlotChanged(int i)
		{
			foreach (var item in items)
			{
				item.SetCurrent(item.index == i);
			}
		}

		private void InventoryItemAdded(InventorySlot slot)
		{
			items[slot.index].SetItem(slot);
		}

		private void InventoryItemRemoved(InventorySlot slot)
		{
			items[slot.index].SetItem(null);
		}
	}
}