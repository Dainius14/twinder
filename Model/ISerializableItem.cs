namespace Twinder.Model
{
	/// <summary>
	/// Common item for <see cref="MatchModel"/>, <see cref="RecModel"/> and <see cref="UserModel"/> to
	/// make serialization for these items easier
	/// </summary>
	public interface ISerializableItem
	{
		string Id { get; set; }
	}

	public class PassAroundItem
	{
		public ISerializableItem Item { get; set; }
		public string DirPath { get; set; }
	}
}
