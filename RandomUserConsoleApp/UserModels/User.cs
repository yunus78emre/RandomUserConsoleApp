using RandomUserConsoleApp.LocationModels;
using RandomUserConsoleApp.LoginModels;


namespace RandomUserConsoleApp.UserModels
{
    public class User
    {
        public string Gender { get; set; }
        public Name Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Cell { get; set; }
        public Dob Dob { get; set; }
        public Registered Registered { get; set; }
        public Id Id { get; set; }
        public string Nat { get; set; }
        public Picture Picture { get; set; }
        public Location Location { get; set; }
        public Login Login { get; set; }
    }
}
