namespace AdventureGame.Tests;

public class ItemTests
{
	[Fact]
	public void Constructor_CreatesDistinctItemsEvenWithEqualValues()
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
	public void PlaceItem_RetainsOriginalItemReference()
	{
		var (_, gallery, _) = RoomTests.CreateDungeon();
		Item coin = new Item("Old Coin", "A tarnished coin.");

		gallery.PlaceItem(coin);

		Assert.Same(coin, gallery.Item);
	}
}