using Microsoft.Data.SqlClient;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalancedPriorityQueueAndRateLimitConsole
{
    public class BalancedPriorityQueue<T>
    {
        private string connectionString;

        public BalancedPriorityQueue(string connectionString)
        {
            this.connectionString = connectionString;
            EnsureTableExists();
        }

        private void EnsureTableExists()
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Messages') CREATE TABLE Messages (Id INT IDENTITY(1,1) PRIMARY KEY, Message NVARCHAR(MAX), Priority INT)";
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public int Count
        {
            get
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "SELECT COUNT(*) FROM Messages";
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
        }

        public void Enqueue(T item, int priority)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "INSERT INTO Messages (Message, Priority) VALUES (@Message, @Priority)";
                    cmd.Parameters.AddWithValue("@Message", item.ToString());
                    cmd.Parameters.AddWithValue("@Priority", priority);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public T Dequeue()
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "DELETE TOP(1) FROM Messages OUTPUT DELETED.Message WHERE Id = (SELECT TOP(1) Id FROM Messages ORDER BY Priority)";
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return (T)reader["Message"];
                        }
                    }
                }
            }

            throw new InvalidOperationException("Queue is empty.");
        }
    }
}