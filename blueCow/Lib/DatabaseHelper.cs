using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blueCow.Lib
{
    class DatabaseHelper
    {
        public T GetData<T>(string command)
        {
            throw new NotImplementedException();
        }

        public List<string> GetCountryCodes()
        {
            using(SqlConnection conn = new SqlConnection(SysConfig.connString))
            {
                conn.Open();
                using(SqlCommand cmd = new SqlCommand("SELECT DISTINCT ida FROM Distances",conn))
                {
                    using(SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        List<string> codes = new List<string>();
                        while (rdr.Read())
                        {
                            codes.Add(rdr["ida"].ToString());
                        }
                        return codes;
                    }
                }
            }
        }

        public void LoadBids(List<string> codes, List<int> bids)
        {
            using (SqlConnection conn = new SqlConnection(SysConfig.connString))
            {
                conn.Open();
                for(var i=0;i<codes.Count;i++)
                {
                    using (SqlCommand cmd = new SqlCommand("UPDATE Bids SET Bid = " + bids[i] + " WHERE id = '" + codes[i] + "'",conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public Dictionary<string,int> GetBids()
        {
            using (SqlConnection conn = new SqlConnection(SysConfig.connString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Bids", conn))
                {
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        Dictionary<string, int> bids = new Dictionary<string, int>();
                        while (rdr.Read())
                        {
                            bids.Add(rdr["id"].ToString(),Convert.ToInt32(rdr["Bid"]));
                        }
                        return bids;
                    }
                }
            }
        } 
    }
}
