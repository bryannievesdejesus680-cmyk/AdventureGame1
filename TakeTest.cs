using static Program;

namespace AdventureGame.Tests;

public class TakeTests
{
	private static (Room Entrance, Room Gallery, Room Archive, Player Player, Item Coin, Item Token) CreateScenario()
	{
		var (entrance, gallery, archive) = RoomTests.CreateDungeon();
		Item coin = new Item("Old Coin", "A tarnished coin.");
		Item token = new Item("Brass Token", "A brass token.");
		gallery.PlaceItem(coin);
		archive.PlaceItem(token);
		Player player = new Player(entrance);
		return (entrance, gallery, archive, player, coin, token);
	}

	[Fact]
	public void TryTake_FromCurrentRoomTransfersOriginalReference()
	{
		var (_, gallery, _, player, coin, _) = CreateScenario();

		Assert.True(player.TryMove(EAST, out _));
		bool taken = player.TryTake("Old Coin", out string message);
		Assert.True(taken);
		Assert.Equal("You take the Old Coin.", message);
		Assert.Null(gallery.Item);
		Assert.Same(coin, player.HeldItem);
		Assert.Same(gallery, player.CurrentRoom);
	}

	[Fact]
	public void TryTake_RejectsItemInAnotherRoom()
	{
		var (entrance, gallery, _, player, coin, _) = CreateScenario();

		Assert.False(player.TryTake("Old Coin", out _));
		Assert.Same(entrance, player.CurrentRoom);
		Assert.Null(player.HeldItem);
		Assert.Same(coin, gallery.Item);
	}

	[Fact]
	public void TryTake_RejectsUnknownNameWithoutChangingRoom()
	{
		var (_, gallery, _, player, coin, _) = CreateScenario();

		Assert.True(player.TryMove(EAST, out _));
		Assert.False(player.TryTake("Excalibur", out _));
		Assert.Same(coin, gallery.Item);
		Assert.Null(player.HeldItem);
	}

	[Fact]
	public void TryTake_RejectsRepeatedTakeWithoutDuplicatingItem()
	{
		var (_, gallery, _, player, coin, _) = CreateScenario();

		Assert.True(player.TryMove(EAST, out _));
		Assert.True(player.TryTake("Old Coin", out _));
		Assert.False(player.TryTake("Old Coin", out _));
		Assert.Null(gallery.Item);
		Assert.Same(coin, player.HeldItem);
	}

	[Fact]
	public void TryTake_RejectsSecondItemWhenAlreadyCarryingOne()
	{
		var (_, _, archive, player, coin, token) = CreateScenario();

		Assert.True(player.TryMove(EAST, out _));
		Assert.True(player.TryTake("Old Coin", out _));
		Assert.True(player.TryMove(NORTH, out _));
		Assert.False(player.TryTake("Brass Token", out string message));
		Assert.Equal("You are already carrying Old Coin.", message);
		Assert.Same(token, archive.Item);
		Assert.Same(coin, player.HeldItem);
	}

	[Fact]
	public void TryTake_ItemPersistsThroughMovementRoundTrip()
	{
		var (_, gallery, _, player, coin, _) = CreateScenario();

		Assert.True(player.TryMove(EAST, out _));
		Assert.True(player.TryTake("Old Coin", out _));
		Assert.True(player.TryMove(NORTH, out _));
		Assert.True(player.TryMove(SOUTH, out _));
		Assert.Same(gallery, player.CurrentRoom);
		Assert.Null(gallery.Item);
		Assert.Same(coin, player.HeldItem);
	}
}