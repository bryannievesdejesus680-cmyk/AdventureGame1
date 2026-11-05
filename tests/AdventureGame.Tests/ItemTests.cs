namespace AdventureGame.Tests;

public class ItemTests
{
	[Fact]
	public void TwoIndependentItemsCanShareAName()
	{
		Item first = new Item("Old Coin", "A tarnished coin.");
		Item second = new Item("Old Coin", "A tarnished coin.");

		Assert.Equal("Old Coin", first.Name);
		Assert.Equal("A tarnished coin.", first.Description);
		Assert.Equal(first.Name, second.Name);
		Assert.Equal(first.Description, second.Description);
		Assert.NotSame(first, second);
	}

	[Fact]
	public void PlaceItem_StoresDistinctReferencesAndRejectsDuplicateReference()
	{
		var (_, gallery, archive) = RoomTests.CreateDungeon();
		Item coin = new Item("Old Coin", "A tarnished coin.");
		Item ring = new Item("Copper Ring", "A copper ring.");
		gallery.PlaceItem(coin);
		gallery.PlaceItem(ring);

		Assert.Equal(2, gallery.Items.Count);
		Assert.Same(coin, gallery.Items[0]);
		Assert.Same(ring, gallery.Items[1]);
		Assert.Throws<InvalidOperationException>(() => gallery.PlaceItem(coin));
		Assert.Equal(2, gallery.Items.Count);
		Assert.Same(coin, gallery.Items[0]);
		Assert.Same(ring, gallery.Items[1]);
		Assert.Empty(archive.Items);
	}

	[Fact]
	public void ReadOnlyViews_ReflectValidatedTransfers()
	{
		var (_, gallery, _) = RoomTests.CreateDungeon();
		Item coin = new Item("Old Coin", "A tarnished coin.");
		gallery.PlaceItem(coin);
		Player player = new Player(gallery);
		IReadOnlyList<Item> roomView = gallery.Items;
		IReadOnlyList<Item> inventoryView = player.Inventory;

		Assert.Single(roomView);
		Assert.Empty(inventoryView);
		Assert.True(player.TryTake("Old Coin", out _));
		Assert.Empty(roomView);
		Assert.Same(coin, inventoryView[0]);
		Assert.True(player.TryDrop("Old Coin", out _));
		Assert.Same(coin, roomView[0]);
		Assert.Empty(inventoryView);
	}

	[Fact]
	public void ReadOnlyViews_RejectExternalAdditions()
	{
		var (_, gallery, _) = RoomTests.CreateDungeon();
		Item coin = new Item("Old Coin", "A tarnished coin.");
		gallery.PlaceItem(coin);
		Player player = new Player(gallery);

		IReadOnlyList<Item> roomView = gallery.Items;
		IReadOnlyList<Item> inventoryView = player.Inventory;
		IList<Item> roomAccess = (IList<Item>)roomView;
		IList<Item> inventoryAccess = (IList<Item>)inventoryView;

		Assert.Throws<NotSupportedException>(() =>
			roomAccess.Add(new Item("Copper Ring", "A copper ring.")));
		Assert.Throws<NotSupportedException>(() =>
			inventoryAccess.Add(new Item("Brass Token", "A brass token.")));
		Assert.Single(roomView);
		Assert.Same(coin, roomView[0]);
		Assert.Empty(inventoryView);
	}
}