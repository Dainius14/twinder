namespace Twinder.Model
{
	public sealed class AccountModel
	{
		public string AccoutName { get; set; }
		public string Name { get; set; }
		public int MatchCount { get; set; }
		public string Photo { get; set; }

		public override string ToString()
		{
			return AccoutName;
		}
	}
}
