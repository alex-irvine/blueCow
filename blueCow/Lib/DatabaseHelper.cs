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
        private int[,] _distMatrix;
        private Dictionary<string, int> _countryCodeIndexes;

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
        
        public int GetDistance(string cc1, string cc2)
        {
            using (SqlConnection conn = new SqlConnection(SysConfig.connString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT kmdist FROM Distances WHERE ida = '" + cc1 + "' AND idb = '" + cc2 + "'", conn))
                {
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            return Convert.ToInt32(rdr["kmdist"]);
                        }
                        return 0;
                    }
                }
            }
        } 

        public int[,] GetDistanceMatrix()
        {
            if(_distMatrix == null)
            {
                Dictionary<string, int> countryCodeIndex = GetCountryCodeIndexes();
                int[,] _distMatrix = new int[countryCodeIndex.Count, countryCodeIndex.Count];
                for (int i = 0; i < countryCodeIndex.Count; i++)
                {
                    //for (int j = 0; j < countryCodeIndex.Count; j++)
                    //{
                    //    if (countryCodeIndex.Keys.ElementAt(i) == countryCodeIndex.Keys.ElementAt(j))
                    //    {
                    //        _distMatrix[i, j] = 0;
                    //        continue;
                    //    }
                    //    _distMatrix[i, j] = GetDistance(countryCodeIndex.Keys.ElementAt(i), countryCodeIndex.Keys.ElementAt(j));
                    //}
                    Parallel.For(0, countryCodeIndex.Count,
                        () => new long[1],
                        (int j, ParallelLoopState state, long[] distance) => 
                        {
                            if (countryCodeIndex.Keys.ElementAt(i) == countryCodeIndex.Keys.ElementAt(j))
                            {
                                distance[0] = 0;
                            }
                            else
                            {
                                distance[0] = GetDistance(countryCodeIndex.Keys.ElementAt(i), countryCodeIndex.Keys.ElementAt(j));
                            }
                            return distance;
                        },
                        
                        (long[] distance) =>
                        {
                            lock (_distMatrix) { _distMatrix[i, j] = distance[0]; }
                        });
                }
            }
            return _distMatrix;
        }

        public Dictionary<string, int> GetCountryCodeIndexes()
        {
            if(_countryCodeIndexes == null)
            {
                _countryCodeIndexes = new Dictionary<string, int>();
                using (SqlConnection conn = new SqlConnection(SysConfig.connString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT DISTINCT ida FROM Distances", conn))
                    {
                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            int index = 0;
                            while (rdr.Read())
                            {
                                _countryCodeIndexes.Add(rdr["ida"].ToString(), index);
                                index++;
                            }
                        }
                    }
                }
            }
            return _countryCodeIndexes;
        }

        public int GetDistanceFromMatrix(string cc1, string cc2)
        {
            var distMatrix = GetDistanceMatrix();
            var countrCodeIndexes = GetCountryCodeIndexes();
            return distMatrix[_countryCodeIndexes[cc1],countrCodeIndexes[cc2]];
        }
    }
}
