namespace SwmsApi.Users.Controllers
{
	public interface IPasswordHasher
	{
		/// <summary>
		/// Creates a Hash of a clear text and convert it to a Base64 String representation
		/// </summary>
		/// <param name="clearText">the clear text</param>
		/// <returns>the Hash</returns>
		string CreateStringHash(string clearText);


		/// <summary>
		/// Verifies a given clear Text against a hash
		/// </summary>
		/// <param name="clearText">The clear text</param>
		/// <param name="data">The hash</param>
		/// <returns>Is the hash equal to the clear text?</returns>
		bool Verify(string clearText, string data);
	}
}