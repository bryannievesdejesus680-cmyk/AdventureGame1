using static Program;

namespace AdventureGame.Tests;

public class TakeTests
{
	private static (Room Gallery, Room Archive, Player Player, Item Coin, Item Token) CreateScenario()
	{
		var (entrance, gallery, archive) = RoomTests.CreateDungeon();
		Item coin = new Item("Old Coin", "A tarnished coin.");
		Item token = new Item("Brass Token", "A brass token.");
		gallery.PlaceItem(coin);
		archive.PlaceItem(token);
		return (gallery, archive, new Player(entrance), coin, token);
	}

	[Fact]
	public void TryTake_CarriesTwoOriginalItemsAcrossRooms()
	{
		var (gallery, archive, player, coin, token) = CreateScenario();
		Assert.True(player.TryMove(EAST, out _));
		Assert.True(player.TryTake("old coin", out _));
		Assert.True(player.TryMove(NORTH, out _));
		Assert.True(player.TryTake("BRASS TOKEN", out _));

		Assert.Empty(gallery.Items);
		Assert.Empty(archive.Items);
		Assert.Equal(2, player.Inventory.Count);
		Assert.Same(coin, player.Inventory[0]);
		Assert.Same(token, player.Inventory[1]);
	}

	[Fact]
	public void TryDrop_TransfersOriginalItemToCurrentRoomAndCanRetake()
	{
		var (_, archive, player, coin, token) = CreateScenario();
		Assert.True(player.TryMove(EAST, out _));
		Assert.True(player.TryTake("Old Coin", out _));
		Assert.True(player.TryMove(NORTH, out _));
		Assert.True(player.TryTake("Brass Token", out _));

		Assert.True(player.TryDrop("old coin", out _));
		Assert.Single(player.Inventory);
		Assert.Same(token, player.Inventory[0]);
		Assert.Single(archive.Items);
		Assert.Same(coin, archive.Items[0]);
		Assert.True(player.TryMove(SOUTH, out _));
		Assert.True(player.TryMove(NORTH, out _));
		Assert.Same(coin, archive.Items[0]);
		Assert.True(player.TryTake("Old Coin", out _));
		Assert.Empty(archive.Items);
		Assert.Same(coin, player.Inventory[1]);
	}

	[Fact]
	public void TryTake_RemovesOnlyTheSelectedRoomItem()
	{
		var (gallery, _, player, coin, _) = CreateScenario();
		Item ring = new Item("Copper Ring", "A copper ring.");
		gallery.PlaceItem(ring);
		Assert.True(player.TryMove(EAST, out _));

		Assert.True(player.TryTake("Old Coin", out _));
		Assert.Single(gallery.Items);
		Assert.Same(ring, gallery.Items[0]);
		Assert.Single(player.Inventory);
		Assert.Same(coin, player.Inventory[0]);
	}

	[Fact]
	public void RejectedTakeAndDrop_PreserveTheCurrentRoomAndCollections()
	{
		var (gallery, _, player, coin, _) = CreateScenario();
		Room start = player.CurrentRoom;
		Assert.False(player.TryTake("Old Coin", out _));
		Assert.False(player.TryDrop("Old Coin", out _));
		Assert.False(player.TryTake("  ", out _));
		Assert.False(player.TryDrop(null!, out _));

		Assert.Same(start, player.CurrentRoom);
		Assert.Same(coin, gallery.Items[0]);
		Assert.Empty(player.Inventory);
	}

	[Fact]
	public void RepeatedTakeAndDrop_PreserveOriginalReferences()
	{
		var (gallery, _, player, coin, _) = CreateScenario();
		Assert.True(player.TryMove(EAST, out _));
		Assert.True(player.TryTake("Old Coin", out _));
		Assert.False(player.TryTake("Old Coin", out _));
		Assert.Same(coin, player.Inventory[0]);
		Assert.Empty(gallery.Items);

		Assert.True(player.TryDrop("Old Coin", out _));
		Assert.False(player.TryDrop("Old Coin", out _));
		Assert.Same(coin, gallery.Items[0]);
		Assert.Empty(player.Inventory);
	}
}