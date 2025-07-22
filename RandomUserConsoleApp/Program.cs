using Newtonsoft.Json;
using Npgsql;
using RandomUserConsoleApp;
using RandomUserConsoleApp.UserModels;


namespace RandomUserToPostgres
{
    class Program
    {
        private static readonly string RandomUserApiUrl = "https://randomuser.me/api/";
        private static readonly string ConnectionString = "Host=localhost;Port=5432;Database=my_database;Username=postgres;Password=1234";

        static async Task Main(string[] args)
        {
            var users = await FetchUsersAsync(10);
            foreach (var user in users)
            {
                Console.WriteLine($"User: {user.Name.Title} {user.Name.First} {user.Name.Last}, Email: {user.Email}, Phone: {user.Phone} , LocationCity: {user.Location.City} , LocationCountry: {user.Location.Country}, LoginUserName: {user.Login.Username} , LoginPassword: {user.Login.Password}");
                await InsertUserIntoDatabaseAsync(user);

            }

            Console.WriteLine("1000 kullanıcı başarıyla eklendi.");
        }

        static async Task<List<User>> FetchUsersAsync(int count)
        {
            var users = new List<User>();

            using var httpClient = new HttpClient();
            for (int i = 0; i < count / 10; i++)
            {
                var response = await httpClient.GetStringAsync($"{RandomUserApiUrl}?results=10");
                var result = JsonConvert.DeserializeObject<RandomUserResult>(response);
                users.AddRange(result.Results);

                
            }

            return users;
        }

        static async Task InsertUserIntoDatabaseAsync(User user)
        {
            await using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            var locationCmd = new NpgsqlCommand
            {
                Connection = connection,
                CommandText = @"INSERT INTO location (street_number, street_name, city, state, country, postcode, latitude, longitude, timezone_offset, timezone_description)
                                VALUES (@StreetNumber, @StreetName, @City, @State, @Country, @Postcode, @Latitude, @Longitude, @TimezoneOffset, @TimezoneDescription) 
                                RETURNING id"
            };
            locationCmd.Parameters.AddWithValue("StreetNumber", user.Location.Street.Number);
            locationCmd.Parameters.AddWithValue("StreetName", user.Location.Street.Name);
            locationCmd.Parameters.AddWithValue("City", user.Location.City);
            locationCmd.Parameters.AddWithValue("State", user.Location.State);
            locationCmd.Parameters.AddWithValue("Country", user.Location.Country);
            locationCmd.Parameters.AddWithValue("Postcode", user.Location.Postcode.ToString());
            locationCmd.Parameters.AddWithValue("Latitude", user.Location.Coordinates.Latitude);
            locationCmd.Parameters.AddWithValue("Longitude", user.Location.Coordinates.Longitude);
            locationCmd.Parameters.AddWithValue("TimezoneOffset", user.Location.Timezone.Offset);
            locationCmd.Parameters.AddWithValue("TimezoneDescription", user.Location.Timezone.Description);

            var locationId = (int)await locationCmd.ExecuteScalarAsync();

            var loginCmd = new NpgsqlCommand
            {
                Connection = connection,
                CommandText = @"INSERT INTO login (uuid, username, password, salt, md5, sha1, sha256)
                                VALUES (@Uuid, @Username, @Password, @Salt, @Md5, @Sha1, @Sha256) RETURNING id"
            };
            loginCmd.Parameters.AddWithValue("Uuid", Guid.Parse(user.Login.Uuid));
            loginCmd.Parameters.AddWithValue("Username", user.Login.Username);
            loginCmd.Parameters.AddWithValue("Password", user.Login.Password);
            loginCmd.Parameters.AddWithValue("Salt", user.Login.Salt);
            loginCmd.Parameters.AddWithValue("Md5", user.Login.Md5);
            loginCmd.Parameters.AddWithValue("Sha1", user.Login.Sha1);
            loginCmd.Parameters.AddWithValue("Sha256", user.Login.Sha256);

            var loginId = (int)await loginCmd.ExecuteScalarAsync();

            var userCmd = new NpgsqlCommand
            {
                Connection = connection,
                CommandText = @"INSERT INTO users (gender , title_name , first_name, last_name, email, phone, cell, dob_date, dob_age, registered_date, registered_age, id_name , id_value , nationality, picture_large_url, picture_medium_url, picture_thumbnail_url, location_id, login_id)
                                VALUES (@Gender, @TitleName ,@FirstName, @LastName, @Email, @Phone, @Cell, @DobDate, @DobAge, @RegisteredDate, @RegisteredAge, @IdName , @IdValue , @Nationality, @PictureLargeUrl, @PictureMediumUrl, @PictureThumbnailUrl, @LocationId, @LoginId)"
            };
            userCmd.Parameters.AddWithValue("Gender", user.Gender);
            userCmd.Parameters.AddWithValue("TitleName", user.Name.Title);
            userCmd.Parameters.AddWithValue("FirstName", user.Name.First);
            userCmd.Parameters.AddWithValue("LastName", user.Name.Last);
            userCmd.Parameters.AddWithValue("Email", user.Email);
            userCmd.Parameters.AddWithValue("Phone", user.Phone);
            userCmd.Parameters.AddWithValue("Cell", user.Cell);
            userCmd.Parameters.AddWithValue("DobDate", user.Dob.Date);
            userCmd.Parameters.AddWithValue("DobAge", user.Dob.Age);
            userCmd.Parameters.AddWithValue("RegisteredDate", user.Registered.Date);
            userCmd.Parameters.AddWithValue("RegisteredAge", user.Registered.Age);
            userCmd.Parameters.AddWithValue("IdName", (object?)user.Id.Name ?? DBNull.Value);
            userCmd.Parameters.AddWithValue("IdValue", (object?)user.Id.Value ?? DBNull.Value);
            userCmd.Parameters.AddWithValue("Nationality", user.Nat);
            userCmd.Parameters.AddWithValue("PictureLargeUrl", user.Picture.Large);
            userCmd.Parameters.AddWithValue("PictureMediumUrl", user.Picture.Medium);
            userCmd.Parameters.AddWithValue("PictureThumbnailUrl", user.Picture.Thumbnail);
            userCmd.Parameters.AddWithValue("LocationId", locationId);
            userCmd.Parameters.AddWithValue("LoginId", loginId);

            await userCmd.ExecuteNonQueryAsync();
            Console.WriteLine("User data inserted successfully.");
        }
    }

    

  
}