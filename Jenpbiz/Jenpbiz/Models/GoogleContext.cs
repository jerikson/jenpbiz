using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.OleDb;
using System.Data.SqlTypes;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace Jenpbiz.Models
{
    public class GoogleContext : DbContext
    {

        public string CmdStringSelectAll = "SELECT * FROM";
        public static OleDbConnection Connection = new OleDbConnection();

        public static string GetConnString() {
            return WebConfigurationManager.ConnectionStrings
                [@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=D:\dev\Github\Jenpbiz\Jenpbiz\GoogleCategories.mdb"].ConnectionString;
        }


        public static void Setup()
        {
            OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder();
            using (OleDbCommand cmd = new OleDbCommand(GetConnString(), Connection))
            {

                try
                {
                    Connection.Open();
                    using (OleDbDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Response.Write(cmd);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: Could not open database conneciton - " + e.Message);
                    throw;
                }
                
            }   


        }



    }
}

//string ConnString = Utils.GetConnString();
//string SqlString = "Select * From Contacts Where FirstName = ? And LastName = ?";
//        using (OleDbConnection conn = new OleDbConnection(ConnString))
//        {
//          using (OleDbCommand cmd = new OleDbCommand(SqlString, conn))
//          {
//            cmd.CommandType = CommandType.Text;
//            cmd.Parameters.AddWithValue("FirstName", txtFirstName.Text);
//            cmd.Parameters.AddWithValue("LastName", txtLastName.Text);
 
//            conn.Open();
//            using (OleDbDataReader reader = cmd.ExecuteReader())
//            {
//              while (reader.Read())
//              {
//                Response.Write(reader["FirstName"].ToString() + " " + reader["LastName"].ToString());
//              }
//            }
//          }
//        }
 