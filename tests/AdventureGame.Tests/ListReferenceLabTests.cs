namespace AdventureGame.Tests;

public class ListReferenceLabTests
{
	[Fact]
	public void AListStoresAndRemovesReferencesToExistingItems()
	{
		Item coin = new Item("Old Coin", "A tarnished coin.");
		Item token = new Item("Brass Token", "A square brass token.");
		List<Item> samples = new List<Item>();
		samples.Add(coin);
		samples.Add(token);

		List<string> names = new List<string>();
		foreach (Item item in samples)
		{
			names.Add(item.Name);
		}

		Assert.Equal(new[] { "Old Coin", "Brass Token" }, names);

		Item? match = null;
		foreach (Item item in samples)
		{
			if (string.Equals(item.Name, "old coin", StringComparison.OrdinalIgnoreCase))
			{
				match = item;
				break;
			}
		}

		Assert.Same(coin, match);
		Assert.Equal(2, samples.Count);
		Assert.True(samples.Remove(coin));
		Assert.Single(samples);
		Assert.Same(token, samples[0]);
		Assert.Equal("Old Coin", coin.Name);
	}
}