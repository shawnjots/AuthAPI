using Microsoft.AspNetCore.Identity;
using System;

namespace AuthAPI
{
	internal class User : IdentityUser, IUser
	{
		private DateTime _createDate;

		public int UserId { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Password { get; set; }
		public bool Disabled { get; set; }
		public DateTime CreateDateTime
		{
			get => _createDate;
			set
			{
				_createDate = DateTime.Now;
				_createDate = value;
			}
		}


	}
}