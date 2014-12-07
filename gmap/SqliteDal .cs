using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gmap
{
    class SqliteDal
    {

        /// <summary> 
        /// Returns datatbale for given sql query. 
        /// </summary> 
        /// <param name="sql"></param> 
        /// <returns></returns> 
        public static DataTable getData(string sql)
        {
            SQLiteConnection cn = new SQLiteConnection(config.DataSource);
            cn.Open();

            try
            {
                SQLiteCommand cm = new SQLiteCommand(sql, cn);
                SQLiteDataReader dr = cm.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(dr);
                cn.Close();

                return dt;
            }
            catch (Exception ex)
            {
                cn.Close();
                throw ex;
            }
        }

        /// <summary> 
        /// Returns count of executed insert, update, delete statement. 
        /// </summary> 
        /// <param name="sql"></param> 
        /// <returns></returns> 
        public static int execNQ(string sql)
        {
            SQLiteConnection cn = new SQLiteConnection(config.DataSource);
            cn.Open();
            SQLiteCommand cm = new SQLiteCommand(sql, cn);
            int rows;
            rows = cm.ExecuteNonQuery();
            cn.Close();
            return rows;
        }

        /// <summary> 
        /// Returns scalar for given sql query. 
        /// </summary> 
        /// <param name="sql"></param> 
        /// <returns></returns> 
        public static object execSC(string sql)
        {
            SQLiteConnection cn = new SQLiteConnection(config.DataSource);
            cn.Open();
            SQLiteCommand cm = new SQLiteCommand(sql, cn);
            object rows;
            rows = cm.ExecuteScalar();
            cn.Close();
            if (rows == null)
            {
                rows = 0;
            }
            return rows;
        }
    }
}
