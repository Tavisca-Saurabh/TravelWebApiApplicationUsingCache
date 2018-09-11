using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using TravelWebApi.Models;
using System.Runtime.Caching;

namespace TravelWebApi.DataAccessLayer
{
    public class AirDAL
    {
        public static bool Add(Air Flight)
        {
            try
            {
                using (var connection = new SqlConnection("Data Source=TAVDESK005;Initial Catalog=Travel;User ID=sa;Password=test123!@#"))
                {

                    connection.Open();
                    string sql = "INSERT INTO Air(Id,Name,Price,FromLocation,ToLocation,DepartureDate,ArrivalDate,IsBooked,IsSaved) VALUES(@Id,@Name,@Price,@FromLocation,@ToLocation,@DepartureDate,@ArrivalDate,@IsBooked,@IsSaved)";
                    SqlCommand cmd = new SqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@Id", Guid.NewGuid().ToString());
                    cmd.Parameters.AddWithValue("@Name", Flight.Name);
                    cmd.Parameters.AddWithValue("@Price", Flight.Price);
                    cmd.Parameters.AddWithValue("@FromLocation", Flight.From);
                    cmd.Parameters.AddWithValue("@ToLocation", Flight.To);
                    cmd.Parameters.AddWithValue("@DepartureDate", Flight.DepartureDate);
                    cmd.Parameters.AddWithValue("@ArrivalDate", Flight.ArrivalDate);
                    cmd.Parameters.AddWithValue("@IsBooked", false);
                    cmd.Parameters.AddWithValue("@IsSaved", false);
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static bool Booking(Guid flightId)
        {
            try
            {
                using (var connection = new SqlConnection("Data Source=TAVDESK005;Initial Catalog=Travel;User ID=sa;Password=test123!@#"))
                {

                    connection.Open();
                    string sql = "UPDATE Air SET IsBooked=@Booked WHERE Id = @FlightId";
                    SqlCommand cmd = new SqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@Booked", true);
                    cmd.Parameters.AddWithValue("@FlightId", flightId);
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool Saving(Guid flightId)
        {
            try
            {
                using (var connection = new SqlConnection("Data Source=TAVDESK005;Initial Catalog=Travel;User ID=sa;Password=test123!@#"))
                {

                    connection.Open();
                    string sql = "UPDATE Air SET IsSaved=@Saved WHERE Id = @HotelId";
                    SqlCommand cmd = new SqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@Saved", true);
                    cmd.Parameters.AddWithValue("@HotelId", flightId);
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static List<Air> GetAll()
        {
            string CacheKey = "AllAir";
            ObjectCache cache = MemoryCache.Default;

            if (cache.Contains(CacheKey))
            {
                System.Diagnostics.Debug.WriteLine("Cache");
                return (List<Air>)cache.Get(CacheKey);
            }
            else
            {
                try
                {
                    using (var connection = new SqlConnection("Data Source=TAVDESK005;Initial Catalog=Travel;User ID=sa;Password=test123!@#"))
                    {
                        List<Air> Flights = new List<Air>();
                        connection.Open();
                        string sql = "SELECT Id,Name,Price,FromLocation,ToLocation,DepartureDate,ArrivalDate FROM Air";
                        SqlCommand cmd = new SqlCommand(sql, connection);
                        cmd.CommandType = CommandType.Text;
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Air Flight = new Air()
                                {
                                    Id = new Guid(reader["Id"].ToString()),
                                    Name = reader["Name"].ToString(),
                                    Price = Convert.ToInt32(reader["Price"]),
                                    From = reader["FromLocation"].ToString(),
                                    To = reader["ToLocation"].ToString(),
                                    DepartureDate = reader["DepartureDate"].ToString(),
                                    ArrivalDate = reader["ArrivalDate"].ToString(),
                                };
                                Flights.Add(Flight);
                            };
                        }
                        CacheItemPolicy cacheItemPolicy = new CacheItemPolicy();
                        cacheItemPolicy.AbsoluteExpiration = DateTime.Now.AddHours(0.1);
                        cache.Add(CacheKey, Flights, cacheItemPolicy);
                        System.Diagnostics.Debug.WriteLine("Database");
                        return Flights;
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
    }
}