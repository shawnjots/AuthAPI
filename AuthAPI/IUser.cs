using System;

namespace AuthAPI
{
	internal interface IUser
	{
		int UserId { get; set; }
		string Email { get; set; }
		string FirstName { get; set; }
		string LastName { get; set; }
		string Password { get; set; }
		bool Disabled { get; set; }
		DateTime CreateDateTime { get; set; }
	}
}