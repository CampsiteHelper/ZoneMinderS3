using System;
using MySql.Data.MySqlClient;

namespace ZMS3Sync
{
    public class ZoneMinderEvent
    {

        public static void createTableIfNotExists()
        {
            
            var sql = "select coalesce(count(*),0) as cnt from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'alarm_uploaded'";
            using (MySqlConnection conn = new MySqlConnection(U.config["MySqlConnection"]))  
            {  
                
                conn.Open();  
                MySqlCommand cmd = new MySqlCommand(sql, conn);  
  
                using (var reader = cmd.ExecuteReader())  
                {

                    reader.Read();

                    var cnt = Convert.ToInt32(reader["cnt"]);
                    reader.Close();


                        if(cnt==0)
                        {
                            U.log("Creating alarm_uploaded table ");
                            try
                            {
                                var sqlCreate = U.config["zmCreateTable"];

                            var cmdCreate = new MySqlCommand(sqlCreate, conn);
                                cmd.ExecuteNonQuery();

                            }
                            catch(Exception e)
                            {
                                U.log("Error creating table ",e);
                            }
                        }

                    }  
                }  
                

        }
        public ZoneMinderEvent()
        {
        }

        public void getEvents()
        {

            using (MySqlConnection conn = new MySqlConnection(U.config["MySqlConnection"]))  
            {  
                
                conn.Open();  
                MySqlCommand cmd = new MySqlCommand(U.config["ZMQuery"], conn);  
  
                using (var reader = cmd.ExecuteReader())  
                {  
                    while (reader.Read())  
                    {
                        U.log(reader["ID"].ToString());

                        /*list.Add(new Album()  
                        {  
                            Id = Convert.ToInt32(reader["Id"]),  
                            Name = reader["Name"].ToString(),   
                            ArtistName = reader["ArtistName"].ToString(),  
                            Price = Convert.ToInt32(reader["Price"]),  
                            Genre = reader["genre"].ToString()  
                        }); 
                        */

                    }  
                }  
                }  
                //return list;  
            }  

        }
    }

