using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using blueCow.Models;
using System.Threading;
using System.Windows.Forms;

namespace blueCow.Lib
{
    class DatabaseHelper
    {
        private int[,] _distMatrix;
        private Dictionary<string, int> _countryCodeIndexes;
        private Dictionary<string, int> _bids;
        private Dictionary<List<string>, int> _distanceLookup;
        private Dictionary<string, string> _continentLookup;

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
            if(_bids == null)
            {
                _bids = new Dictionary<string, int>();
                using (SqlConnection conn = new SqlConnection(SysConfig.connString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM Bids", conn))
                    {
                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                _bids.Add(rdr["id"].ToString(), Convert.ToInt32(rdr["Bid"]));
                            }
                        }
                    }
                }
            }
            return _bids;
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

        public int[,] GetDistanceMatrix(ProgressBar progBar = null)
        {
            if(_distMatrix == null)
            {
                Dictionary<string, int> countryCodeIndex = GetCountryCodeIndexes();
                _distMatrix = new int[countryCodeIndex.Count, countryCodeIndex.Count];
                for (int i = 0; i < countryCodeIndex.Count; i++)
                {
                    for (int j = 0; j < countryCodeIndex.Count; j++)
                    {
                        if (countryCodeIndex.Keys.ElementAt(i) == countryCodeIndex.Keys.ElementAt(j))
                        {
                            _distMatrix[i, j] = 0;
                            continue;
                        }
                        _distMatrix[i, j] = GetDistance(countryCodeIndex.Keys.ElementAt(i), countryCodeIndex.Keys.ElementAt(j),true);
                    }
                    if (progBar != null)
                    {
                        progBar.Value = 203 * i + 1;
                    }
                    //var numCores = Environment.ProcessorCount;
                    //var tasks = new Task[numCores];
                    //int j = -1;
                    //for (int taskNum = 0; taskNum < numCores; taskNum++)
                    //{
                    //    tasks[taskNum] = Task.Factory.StartNew(
                    //            () =>
                    //            {
                    //                int k = Interlocked.Increment(ref j);
                    //                while (k < countryCodeIndex.Count)
                    //                {
                    //                    if (countryCodeIndex.Keys.ElementAt(i) == countryCodeIndex.Keys.ElementAt(k))
                    //                    {
                    //                        _distMatrix[i, countryCodeIndex.Values.ElementAt(k)] = 0;
                    //                    }
                    //                    else
                    //                    {
                    //                        _distMatrix[i, countryCodeIndex.Values.ElementAt(k)] =
                    //                            GetDistance(countryCodeIndex.Keys.ElementAt(i), countryCodeIndex.Keys.ElementAt(k),true);
                    //                    }
                    //                    k = Interlocked.Increment(ref j);
                    //                }
                    //            }
                    //        );
                    //}
                    //Task.WaitAll(tasks);
                    //if (progBar != null)
                    //{
                    //    progBar.Value = j*i;
                    //}
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

        public int GetDistance(string cc1, string cc2, bool notSlow)
        {
            if(_distanceLookup == null)
            {
                _distanceLookup = new Dictionary<List<string>, int>();
                using (SqlConnection conn = new SqlConnection(SysConfig.connString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT ida,idb,kmdist FROM Distances", conn))
                    {
                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                _distanceLookup.Add
                                    (new List<string>() { rdr["ida"].ToString(), rdr["idb"].ToString() }, Convert.ToInt32(rdr["kmdist"]));
                            }
                        }
                    }
                }
            }
            foreach(var kvp in _distanceLookup)
            {
                if(kvp.Key[0] == cc1 && kvp.Key[1] == cc2)
                {
                    return kvp.Value;
                }
            }
            throw new KeyNotFoundException(string.Format("There is no matching combination of keys for {0}, {1} in the dictionary", cc1, cc2));
        }

        public Dictionary<string, string> GetContinentCodes()
        {
            if (_continentLookup == null)
            {
                _continentLookup = new Dictionary<string, string>();
                using (SqlConnection conn = new SqlConnection(SysConfig.connString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT a_3,cc FROM continents", conn))
                    {
                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                if (_continentLookup.ContainsKey(rdr["a_3"].ToString()))
                                {
                                    _continentLookup.Add
                                    (rdr["a_3"].ToString()+"_D", rdr["cc"].ToString());
                                }
                                else
                                {
                                    _continentLookup.Add
                                    (rdr["a_3"].ToString(), rdr["cc"].ToString());
                                }
                            }
                        }
                    }
                }
            }
            return _continentLookup;
        }

        public string GetContinentCode(string countryCode)
        {
            var codes = GetContinentCodes();
            return codes[countryCode];
        }

        public void SortOutTheGoddamMissingCountryCodesRandomly(Random rand)
        {
            Dictionary<string, string> countries = new Dictionary<string, string>();
            using (SqlConnection conn = new SqlConnection(SysConfig.connString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("Select * from continents where cc IS NULL", conn))
                {
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            countries.Add(rdr["a_3"].ToString(), "");
                        }
                    }
                }
                foreach(var kvp in countries)
                {
                    // randomly select a continent
                    string cont = SysConfig.majorContinents[rand.Next(0, SysConfig.majorContinents.Count)];
                    using (SqlCommand cmd = new SqlCommand("UPDATE continents SET cc = '" + cont + "' WHERE a_3 = '" + kvp.Key + "'", conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
